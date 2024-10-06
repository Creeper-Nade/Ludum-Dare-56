using UnityEngine;
using System.Collections;

public class BacteriaC : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 1f; // 细菌移动速度
    public float wanderRadius = 5f; // 随机游走的半径

    [Header("资源携带")]
    public int maxTotalResources = 10; // 最大携带资源总量
    public int carryingX = 0; // 携带的X资源量
    public int carryingY = 0; // 携带的Y资源量
    public int carryingZ = 0; // 携带的Z资源量

    [Header("采集设置")]
    public float collectionCooldown = 3f; // 采集冷却时间
    public float findResourceDelayMin = 0.1f; // 寻找资源的最小延迟时间
    public float findResourceDelayMax = 1f; // 寻找资源的最大延迟时间

    private GameObject targetResource; // 当前目标资源
    private bool isPlayerControlled = false; // 是否被玩家控制
    private float lastCollectionTime; // 上次采集时间
    private Vector3 wanderTarget; // 随机游走的目标点

    // 细菌的状态枚举
    private enum State
    {
        SeekingResource, // 寻找资源
        CollectingResource, // 收集资源
        Wandering // 随机游走
    }

    private State currentState = State.SeekingResource; // 当前状态

    private void Start()
    {
        lastCollectionTime = -collectionCooldown; // 确保游戏开始时可以立即采集
        StartCoroutine(BehaviorRoutine());
    }

    // 主要行为协程
    private IEnumerator BehaviorRoutine()
    {
        while (true)
        {
            if (!isPlayerControlled)
            {
                switch (currentState)
                {
                    case State.SeekingResource:
                        yield return StartCoroutine(FindNearestResourceWithDelay());
                        break;
                    case State.CollectingResource:
                        if (targetResource != null && GetTotalResources() < maxTotalResources)
                        {
                            MoveTowardsTarget();
                        }
                        else
                        {
                            currentState = State.Wandering;
                            SetNewWanderTarget();
                        }
                        break;
                    case State.Wandering:
                        Wander();
                        break;
                }
            }
            else
            {
                // 预留给玩家控制的逻辑
            }

            yield return null;
        }
    }

    // 带延迟的寻找最近资源
    private IEnumerator FindNearestResourceWithDelay()
    {
        // 随机延迟，避免所有细菌同时寻找资源
        float delay = Random.Range(findResourceDelayMin, findResourceDelayMax);
        yield return new WaitForSeconds(delay);

        FindNearestResource();
        if (targetResource != null)
        {
            currentState = State.CollectingResource;
        }
        else
        {
            currentState = State.Wandering;
            SetNewWanderTarget();
        }
    }

    // 寻找最近的资源
    private void FindNearestResource()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("resource");
        float nearestDistance = Mathf.Infinity;
        GameObject nearestResource = null;

        foreach (GameObject resource in resources)
        {
            if (resource.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, resource.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestResource = resource;
                }
            }
        }

        targetResource = nearestResource;

        if (targetResource == null)
        {
            Debug.LogWarning("没有找到带有'resource'标签的可用游戏对象");
        }
    }

    // 向目标资源移动
    private void MoveTowardsTarget()
    {
        if (targetResource != null && targetResource.activeSelf)
        {
            Vector3 direction = (targetResource.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 如果到达目标附近，尝试收集资源
            if (Vector3.Distance(transform.position, targetResource.transform.position) < 0.1f)
            {
                TryCollectResource();
            }
        }
        else
        {
            currentState = State.SeekingResource;
        }
    }

    // 尝试收集资源（考虑冷却时间）
    private void TryCollectResource()
    {
        if (Time.time - lastCollectionTime >= collectionCooldown)
        {
            CollectResource();
            lastCollectionTime = Time.time;
        }
    }

    // 收集资源
    private void CollectResource()
    {
        if (targetResource != null && targetResource.activeSelf)
        {
            Resource resourceScript = targetResource.GetComponent<Resource>();
            if (resourceScript != null)
            {
                string collectedResourceType;
                if (resourceScript.TryCollect(out collectedResourceType))
                {
                    resourceScript.ConfirmCollection(collectedResourceType);
                    AddResource(collectedResourceType);
                    
                    Debug.Log($"BacteriaC collected {collectedResourceType} from {targetResource.name}. " +
                              $"Current: X={carryingX}, Y={carryingY}, Z={carryingZ}");

                    // 如果资源已满，切换到游走状态
                    if (GetTotalResources() >= maxTotalResources)
                    {
                        Debug.Log("BacteriaC reached maximum capacity. Switching to wandering state.");
                        currentState = State.Wandering;
                        SetNewWanderTarget();
                    }
                }
                else
                {
                    currentState = State.SeekingResource;
                }
            }
        }
        else
        {
            currentState = State.SeekingResource;
        }
    }

    // 添加收集到的资源
    private void AddResource(string resourceType)
    {
        switch (resourceType)
        {
            case "X":
                carryingX++;
                break;
            case "Y":
                carryingY++;
                break;
            case "Z":
                carryingZ++;
                break;
        }
    }

    // 获取当前携带的总资源量
    private int GetTotalResources()
    {
        return carryingX + carryingY + carryingZ;
    }

    // 设置新的随机游走目标
    private void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        wanderTarget = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * wanderRadius;
    }

    // 执行随机游走
    private void Wander()
    {
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, moveSpeed * Time.deltaTime);

        // 如果到达游走目标，设置新的目标
        if (Vector3.Distance(transform.position, wanderTarget) < 0.1f)
        {
            SetNewWanderTarget();
        }
    }

    // 设置是否由玩家控制（预留给外部控制）
    public void SetPlayerControlled(bool controlled)
    {
        isPlayerControlled = controlled;
    }
}
