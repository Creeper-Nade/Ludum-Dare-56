using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class matrix_manager : MonoBehaviour
{
    public NavMeshSurface player_navMesh;
    public NavMeshSurface enemy_navMesh;
        // 4个公开可编辑的资源点引用
    public Bacterial_Matrix matrix1;
    public Bacterial_Matrix matrix2;
    public Bacterial_Matrix matrix3;

    // 资源点列表
    private List<Bacterial_Matrix> matrixList = new List<Bacterial_Matrix>();

    // 当前活跃的资源点数量
    private int activeMatrixCount;

    // 触发重新初始化的资源点数量
    public int reinitializeThreshold = 1;

    // 资源点之间的最小距离
    public float minDistanceBetweenMatrix = 200f;

    // 资源点生成范围
    public float minRange = -80f;
    public float maxRange = 80f;

    //stuff

    [SerializeField]public List<GameObject> obstacle_list;

    private void Start()
    {
        InitializeAllResources();
    }

    // 初始化所有资源点
    private void InitializeAllResources()
    {
        matrixList.Clear();
        AddResourceIfValid(matrix1);
        AddResourceIfValid(matrix2);
        AddResourceIfValid(matrix3);


        List<Vector2> positions = GenerateUniquePositions(matrixList.Count);

        for (int i = 0; i < matrixList.Count; i++)
        {
            InitializeResource(matrixList[i], positions[i]);
        }

        activeMatrixCount = matrixList.Count;

        // 确保所有资源都被激活
        foreach (var matrixList in matrixList)
        {
            matrixList.gameObject.SetActive(true);
        }
        player_navMesh.BuildNavMesh();
        enemy_navMesh.BuildNavMesh();
    }

    // 添加有效的资源到列表
    private void AddResourceIfValid(Bacterial_Matrix matrix)
    {
        if (matrix != null)
        {
            matrixList.Add(matrix);
        }
    }

    // 初始化单个资源点
    private void InitializeResource(Bacterial_Matrix matrix, Vector2 position)
    {
        if (matrix != null)
        {
            matrix.gameObject.SetActive(true); // 确保资源对象被激活
            matrix.transform.position = position;
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
            } while (IsOverlappingAny(position, positions) && attempts < maxAttempts|| IsOverlappingObstacle(position));

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


    // 检查是否与任何其他位置重叠
    private bool IsOverlappingAny(Vector2 position, List<Vector2> existingPositions)
    {
        foreach (var existingPosition in existingPositions)
        {
            if (Vector2.Distance(existingPosition, position) < minDistanceBetweenMatrix)
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
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.GetAreaFromName("Enemy")))
            {
                return true;
            }
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
    //public void RefreshNavMesh()
    //{
    //    Debug.Log("calledToDestroy");
    //    player_navMesh.UpdateNavMesh(player_navMesh.navMeshData);
    //    enemy_navMesh.UpdateNavMesh(enemy_navMesh.navMeshData);
    //}
}
