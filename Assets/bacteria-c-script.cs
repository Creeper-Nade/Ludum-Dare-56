using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BacteriaC : MonoBehaviour
{
    [Header("基础属性")]
    public int maxHealth = 8; // 最大血量
    private int currentHealth; // 当前血量
    public int damage = 2; // 攻击力
    public float moveSpeed = 8f; // 移动速度
    public int collectionAmount = 10; // 采集量
    public float attackSpeed = 6f; // 攻击速度（每秒攻击次数）

    [Header("移动设置")]
    public float wanderRadius = 5f; // 随机游走的半径

    [Header("资源携带")]
    public int maxTotalResources = 10; // 最大携带资源总量
    public int carryingX = 0; // 携带的X资源量
    public int carryingY = 0; // 携带的Y资源量
    public int carryingZ = 0; // 携带的Z资源量

    [Header("采集设置")]
    public float collectionCooldown = 1f / 6f; // 采集冷却时间（基于攻击速度）
    public float findResourceDelayMin = 0.1f; // 寻找资源的最小延迟时间
    public float findResourceDelayMax = 1f; // 寻找资源的最大延迟时间

    [Header("集群效果")]
    public float groupingRadius = 2f; // 集群检测半径
    public int extraCollectionAmount3 = 5; // 3个细菌集合时的额外采集量
    public int extraCollectionAmount6 = 10; // 6个细菌集合时的额外采集量
    public float extraSpeed6 = 2f; // 6个细菌集合时的额外速度

    [Header("战斗设置")]
    public float attackRange = 1f; // 攻击范围

    private GameObject targetResource; // 当前目标资源
    private bool isPlayerControlled = false; // 是否被玩家控制
    private float lastCollectionTime; // 上次采集时间
    private Vector3 wanderTarget; // 随机游走的目标点
    private GameObject matrixGo; // 母巢对象
    private float lastAttackTime; // 上次攻击时间

    // 细菌的状态枚举
    private enum State
    {
        SeekingResource, // 寻找资源
        CollectingResource, // 收集资源
        Wandering, // 随机游走
        ReturnHome, // 返回母巢
        Fighting, // 战斗中
        Dead // 被击杀
    }

    private State currentState = State.SeekingResource; // 当前状态

    private void Start()
    {
        currentHealth = maxHealth; // 初始化当前血量为最大血量
        var playerMatrix = FindFirstObjectByType<player_matrix>();
        matrixGo = playerMatrix.gameObject;
        lastCollectionTime = -collectionCooldown; // 确保游戏开始时可以立即采集
        lastAttackTime = -1f / attackSpeed; // 确保游戏开始时可以立即攻击
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
                            currentState = State.ReturnHome;
                        }
                        break;
                    case State.ReturnHome:
                        ReturnToMatrix();
                        break;
                    case State.Wandering:
                        Wander();
                        break;
                    case State.Fighting:
                        Fight();
                        break;
                    case State.Dead:
                        if (carryingX > 0 || carryingY > 0 || carryingZ > 0)
                        {
                            SpawnResources();
                        }
                        Destroy(gameObject);
                        yield break;
                }
            }
            else
            {
                // 预留给玩家控制的逻辑
            }

            CheckForEnemies(); // 检查附近的敌人
            ApplyGroupingEffect(); // 应用集群效果

            yield return null;
        }
    }
    
    // 被击杀，掉落资源
    private void SpawnResources()
    {
        // TODO: 实现资源掉落逻辑
        Debug.Log($"掉落资源: X={carryingX}, Y={carryingY}, Z={carryingZ}");
        carryingX = 0;
        carryingY = 0;
        carryingZ = 0;
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
                    resourceScript.ConfirmCollection(collectedResourceType, collectionAmount);
                    AddResource(collectedResourceType, collectionAmount);
                    
                    Debug.Log($"BacteriaC collected {collectionAmount} {collectedResourceType} from {targetResource.name}. " +
                              $"Current: X={carryingX}, Y={carryingY}, Z={carryingZ}");

                    // 如果资源已满，切换到返回母巢状态
                    if (GetTotalResources() >= maxTotalResources)
                    {
                        Debug.Log("BacteriaC reached maximum capacity. Returning home.");
                        currentState = State.ReturnHome;
                    }

                    // 如果资源点被采集完毕，通知所有细菌返回母巢
                    if (!resourceScript.HasRemainingResources())
                    {
                        Debug.Log("Resource depleted. All bacteria returning home.");
                        NotifyAllBacteriaToReturnHome();
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
    private void AddResource(string resourceType, int amount)
    {
        switch (resourceType)
        {
            case "X":
                carryingX += amount;
                break;
            case "Y":
                carryingY += amount;
                break;
            case "Z":
                carryingZ += amount;
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

    // 返回母巢
    private void ReturnToMatrix()
    {
        Vector3 direction = (matrixGo.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 如果靠近母巢，转移资源并切换到寻找资源状态
        if (Vector3.Distance(transform.position, matrixGo.transform.position) < 2f)
        {
            TransferResourcesToMatrix();
            currentState = State.SeekingResource;
        }
    }

    // 将资源转移给母巢
    private void TransferResourcesToMatrix()
    {
        Bacterial_Matrix bm = matrixGo.GetComponent<Bacterial_Matrix>();
        if (bm != null)
        {
            bm.AddResources(carryingX, carryingY, carryingZ);
            Debug.Log($"Transferred resources to matrix: X={carryingX}, Y={carryingY}, Z={carryingZ}");
            carryingX = 0;
            carryingY = 0;
            carryingZ = 0;
        }
    }

    // 通知所有细菌返回母巢
    private void NotifyAllBacteriaToReturnHome()
    {
        BacteriaC[] allBacteria = FindObjectsOfType<BacteriaC>();
        foreach (BacteriaC bacteria in allBacteria)
        {
            bacteria.currentState = State.ReturnHome;
        }
    }

    // 应用集群效果
    private void ApplyGroupingEffect()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, groupingRadius);
        int nearbyBacteriaCount = 0;

        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject != gameObject && collider.GetComponent<BacteriaC>() != null)
            {
                nearbyBacteriaCount++;
            }
        }

        if (nearbyBacteriaCount >= 5) // 6个细菌（包括自己）
        {
            collectionAmount = 10 + extraCollectionAmount3 + extraCollectionAmount6;
            moveSpeed = 8f + extraSpeed6;
        }
        else if (nearbyBacteriaCount >= 2) // 3个细菌（包括自己）
        {
            collectionAmount = 10 + extraCollectionAmount3;
            moveSpeed = 8f;
        }
        else
        {
            collectionAmount = 10;
            moveSpeed = 8f;
        }
    }

    // 检查附近的敌人
    private void CheckForEnemies()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("EnemyBacteria"))
            {
                currentState = State.Fighting;
                return;
            }
        }

        if (currentState == State.Fighting)
        {
            currentState = State.SeekingResource;
        }
    }

    // 战斗逻辑
    private void Fight()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("EnemyBacteria"))
            {
                if (Time.time - lastAttackTime >= 1f / attackSpeed)
                {
                    AttackEnemy(collider.gameObject);
                    lastAttackTime = Time.time;
                }
                return;
            }
        }

        currentState = State.SeekingResource;
    }

    // 攻击敌人
    private void AttackEnemy(GameObject enemy)
    {
        BacteriaC enemyBacteria = enemy.GetComponent<BacteriaC>();
        if (enemyBacteria != null)
        {
            enemyBacteria.TakeDamage(damage);
            Debug.Log($"攻击敌人: {enemy.name}，造成 {damage} 点伤害");
        }
    }

    // 受到伤害
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"受到 {damageAmount} 点伤害，剩余生命值: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentState = State.Dead;
        }
    }

    // 设置是否由玩家控制（预留给外部控制）
    public void SetPlayerControlled(bool controlled)
    {
        isPlayerControlled = controlled;
    }
}
