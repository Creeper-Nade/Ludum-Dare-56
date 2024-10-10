using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;

public class ResourceManager : MonoBehaviour
{
    // 4个公开可编辑的资源点引用
    public Resource resource1;
    public Resource resource2;
    public Resource resource3;
    public Resource resource4;

    // 资源点列表
    private List<Resource> resources = new List<Resource>();

    // 当前活跃的资源点数量
    private int activeResourceCount;

    // 触发重新初始化的资源点数量
    public int reinitializeThreshold = 1;

    // 资源点之间的最小距离
    public float minDistanceBetweenResources = 10f;

    // 资源点生成范围
    public float minRange = -525f;
    public float maxRange = 525f;

    //stuff
    [SerializeField]List<Bacterial_Matrix> matrix_list;
    [SerializeField]public List<GameObject> obstacle_list;

    private void Awake() {
        matrix_list.AddRange(FindObjectsOfType<Bacterial_Matrix>());
    }
    private void Start()
    {
        InitializeAllResources();
    }

    // 初始化所有资源点
    private void InitializeAllResources()
    {
        resources.Clear();
        AddResourceIfValid(resource1);
        AddResourceIfValid(resource2);
        AddResourceIfValid(resource3);
        AddResourceIfValid(resource4);

        if (resources.Count < 4)
        {
            Debug.LogWarning($"未设置所有资源点。当前设置了 {resources.Count} 个资源点，应该设置 4 个。");
        }

        List<Vector2> positions = GenerateUniquePositions(resources.Count);

        for (int i = 0; i < resources.Count; i++)
        {
            InitializeResource(resources[i], positions[i]);
        }

        activeResourceCount = resources.Count;

        // 确保所有资源都被激活
        foreach (var resource in resources)
        {
            resource.gameObject.SetActive(true);
        }

        Debug.Log($"已初始化并激活 {activeResourceCount} 个资源点。");
    }

    // 添加有效的资源到列表
    private void AddResourceIfValid(Resource resource)
    {
        if (resource != null)
        {
            resources.Add(resource);
        }
    }

    // 初始化单个资源点
    private void InitializeResource(Resource resource, Vector2 position)
    {
        if (resource != null)
        {
            resource.gameObject.SetActive(true); // 确保资源对象被激活
            resource.SetManagerReference(this);
            resource.transform.position = position;
            resource.Initialize(position);
        }
        else
        {
            Debug.LogError("尝试初始化空的资源点引用。");
        }
    }

    // 生成不重叠的唯一位置
    private List<Vector2> GenerateUniquePositions(int count)
    {
        List<Vector2> positions = new List<Vector2>();
        int attempts;
        const int maxAttempts = 100;

        for (int i = 0; i < count; i++)
        {
            Vector2 position;
            attempts = 0;
            do
            {
                position = GetRandomPosition();
                attempts++;
            } while (IsOverlappingAny(position, positions) && attempts < maxAttempts && IsOverlappingMatrix(position)&& IsOverlappingObstacle(position));

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"无法为资源点 {i + 1} 找到不重叠的位置，使用最后一次尝试的位置。");
            }

            positions.Add(position);
        }

        return positions;
    }

    // 获取随机位置
    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(minRange, maxRange);
        float y = Random.Range(minRange, maxRange);
        return new Vector2(x, y);
    }

    // 资源点耗尽计数器
    public void ResourceDepletedCounter()
    {
        activeResourceCount--;
        Debug.Log($"资源点被耗尽，当前剩余活跃资源点：{activeResourceCount}");
        if (activeResourceCount <= reinitializeThreshold)
        {
            Debug.Log("触发资源重新初始化");
            InitializeAllResources();
        }
    }

    // 检查是否与任何其他位置重叠
    private bool IsOverlappingAny(Vector2 position, List<Vector2> existingPositions)
    {
        foreach (var existingPosition in existingPositions)
        {
            if (Vector2.Distance(existingPosition, position) < minDistanceBetweenResources)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOverlappingMatrix(Vector2 position)
    {
        
        foreach(Bacterial_Matrix matrix in matrix_list)
        {
                    if (position.x > (matrix.gameObject.transform.position.x - 5)
                && position.x < (matrix.gameObject.transform.position.x + 5)
                && position.y > (matrix.gameObject.transform.position.y - 5)
                && position.y < (matrix.gameObject.transform.position.y + 5)
            )
            {
                return true;
            }
        }


        return false;
    }
    private bool IsOverlappingObstacle(Vector2 position)
    {
        
        foreach(GameObject obstacle in obstacle_list)
        {
            Collider2D obstacle_collider;
            obstacle_collider=obstacle.gameObject.GetComponent<Collider2D>(); 
            //NavMeshHit hit;
            //if (NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.GetAreaFromName("Enemy")))
            //{
            //    not_overlapping=true;
            //}
            if (obstacle_collider.bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }

    private object FindObjectByType<T>()
    {
        throw new System.NotImplementedException();
    }
}
