using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;

public class BacteriaC : MonoBehaviour
{
    [Header("my stuff")]
    public Bacteria_General bacGen;

    [SerializeField] Global_Data data;
    public NavMeshAgent agent;
    [SerializeField] Bacterial_Matrix own_matrix;
    [SerializeField] BacC_Enemy_Detection enemy_Detection;
    public bool Had_collected=false;
        private float distance;
    private float nearestDistance=10000;
    [SerializeField] private GameObject nearestFoe;
    public AudioClip Retrieve;
    public AudioClip Transfer;
    public AudioSource audioSource;
    [Header("基础属性")]
    public int collectionAmount = 1; // 采集量


    [Header("资源携带")]
    public int maxTotalResources = 10; // 最大携带资源总量
    public int carryingX = 0; // 携带的X资源量
    public int carryingY = 0; // 携带的Y资源量
    public int carryingZ = 0; // 携带的Z资源量

    [Header("采集设置")]
    public int collectionCooldown = 1; // 采集冷却时间（基于攻击速度）
    public float findResourceDelayMin = 0.1f; // 寻找资源的最小延迟时间
    public float findResourceDelayMax = 0.5f; // 寻找资源的最大延迟时间

    [Header("战斗设置")]
    public GameObject targetResource; // 当前目标资源
    private GameObject matrixGo; // 母巢对象

    // 细菌的状态枚举
    public enum State
    {
        SeekingResource, // 寻找资源
        CollectingResource, // 收集资源
        Wandering, // 随机游走
        ReturnHome, // 返回母巢
        Fighting, // 战斗中
        Dead // 被击杀
    }

    public State currentState = State.SeekingResource; // 当前状态

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        bacGen=this.gameObject.GetComponent<Bacteria_General>();
        Had_collected=false;
        enemy_Detection=this.gameObject.GetComponentInChildren<BacC_Enemy_Detection>();
        audioSource=bacGen.sound_source;
        //initialize stats
        collectionAmount = 1;
        Had_collected=false;
        findResourceDelayMin=0.1f;
        findResourceDelayMax=0.5f;
    }
    private void Start()
    {
        switch(bacGen.Team)
        {
            case 1:
            foreach(GameObject elements in data.Team1)
            {   
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>();
            }
            break;

            case 2:
            foreach(GameObject elements in data.Team2)
            {
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>(); 
            }
            break;

            case 3:
            foreach(GameObject elements in data.Team3)
            {
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>();  
            }
            break;

            default:
            break;
        }
        matrixGo = own_matrix.gameObject;
        StartCoroutine(BehaviorRoutine());
    }

    // 主要行为协程
    private IEnumerator BehaviorRoutine()
    {
        while (true)
        {
            if (!bacGen.designated_destination&&CheckForEnemies()==false)
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
            currentState = State.CollectingResource;
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
            //Debug.LogWarning("没有找到带有'resource'标签的可用游戏对象");
        }
    }

    // 向目标资源移动
    private void MoveTowardsTarget()
    {
        if (targetResource != null && targetResource.activeSelf)
        {
            agent.SetDestination(targetResource.transform.position);
        }
        else
        {
            currentState = State.SeekingResource;
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("resource"))
        {            
            if(Had_collected==false)StartCoroutine(CollectResourceEnum());
            Had_collected=true;
            
        }
        if(other.gameObject==matrixGo)
        {
            TransferResourcesToMatrix();
            currentState = State.SeekingResource;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D other) {
        
        if(other.gameObject==matrixGo)
        {
            audioSource.PlayOneShot(Transfer);
        }
    }

    IEnumerator CollectResourceEnum()
    {
        CollectResource();
        yield return new WaitForSeconds(collectionCooldown);       
        Had_collected=false;
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
                    int confirmCount = resourceScript.ConfirmCollection(collectedResourceType, collectionAmount);
                    AddResource(collectedResourceType, confirmCount);
                    
                    //Debug.Log($"BacteriaC collected {collectionAmount} {collectedResourceType} from {targetResource.name}. " +
                              //$"Current: X={carryingX}, Y={carryingY}, Z={carryingZ}");

                    // 如果资源已满，切换到返回母巢状态
                    if (GetTotalResources() >= maxTotalResources)
                    {
                        //Debug.Log("BacteriaC reached maximum capacity. Returning home.");
                        currentState = State.ReturnHome;
                    }

                    // 如果资源点被采集完毕，通知所有细菌返回母巢
                    if (!resourceScript.HasRemainingResources())
                    {
                        //Debug.Log("Resource depleted. All bacteria returning home.");
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
        audioSource.PlayOneShot(Retrieve);
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

    // 返回母巢
    private void ReturnToMatrix()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(matrixGo.transform.position,out hit, 10.0f,1);
        agent.SetDestination(hit.position);
    }

    // 将资源转移给母巢
    private void TransferResourcesToMatrix()
    {
        Bacterial_Matrix bm = matrixGo.GetComponent<Bacterial_Matrix>();
        if (bm != null)
        {
            bm.AddResources(carryingX, carryingY, carryingZ);
            
            //Debug.Log($"Transferred resources to matrix: X={carryingX}, Y={carryingY}, Z={carryingZ}");
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

    // 检查附近的敌人
    private bool CheckForEnemies()
    {
        if(enemy_Detection.entered_object.Any())
        {
            foreach(GameObject bacteria in enemy_Detection.entered_object)
            {
                distance=Vector3.Distance(this.transform.position,bacteria.transform.position);
                if(distance<nearestDistance)
                {
                    nearestFoe=bacteria;
                    nearestDistance=distance;
                }
            }
            nearestDistance=100000;
            if(bacGen.designated_destination==false)
            {
                agent.SetDestination(nearestFoe.transform.position);
                return true;
            }
            else return false;
        }
        else{
            return false;
        }  
    }
        private void FixedUpdate() {
        if(bacGen.designated_destination==false&&CheckForEnemies()==true)
        {
            //rotate as foe
            Vector3 targetDirection= nearestFoe.transform.position-this.transform.position;
            float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
            transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
        }
        else if(bacGen.designated_destination==false)
        {
            if(currentState!=State.ReturnHome&&currentState==State.CollectingResource)
            {
                //rotate to resourec destination
                Vector3 targetDirection= targetResource.transform.position-this.transform.position;
                float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
                transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
            }
            if(currentState==State.ReturnHome)
            {
                Vector3 targetDirection= matrixGo.transform.position-this.transform.position;
                float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
                transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
            }
            
        }
    }
}
