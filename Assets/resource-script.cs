using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{
    // 定义资源类型枚举
    public enum ResourceType
    {
        TypeX, // 50X + 20Y + 10Z
        TypeY, // 20X + 50Y + 10Z
        TypeZ  // 20X + 20Y + 40Z
    }

    // 当前资源点的类型
    public ResourceType currentType;

    // 当前资源量
    [SerializeField] private int currentX, currentY, currentZ;

    // 对ResourceManager的引用
    private ResourceManager managerRef;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D resourceCollider;

    // 采集冷却时间
    private float collectCooldown = 3f;
    private float lastCollectTime;

    // 当前重叠的Matrix实例
    private GameObject currentMatrixInstance;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        resourceCollider = GetComponent<CircleCollider2D>();
        if (resourceCollider == null)
        {
            resourceCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        resourceCollider.isTrigger = true;
        resourceCollider.radius = 1f; // 设置适当的半径
    }

    // 设置ResourceManager引用的方法
    public void SetManagerReference(ResourceManager manager)
    {
        managerRef = manager;
    }

    // 初始化资源点
    public void Initialize(Vector2 position)
    {
        transform.position = position;
        InitializeResources();
        SetVisibility(true);
    }

    // 初始化资源量
    private void InitializeResources()
    {
        switch (currentType)
        {
            case ResourceType.TypeX:
                currentX = 50;
                currentY = 20;
                currentZ = 10;
                break;
            case ResourceType.TypeY:
                currentX = 20;
                currentY = 50;
                currentZ = 10;
                break;
            case ResourceType.TypeZ:
                currentX = 20;
                currentY = 20;
                currentZ = 40;
                break;
        }
    }

    // 尝试采集资源
    public bool TryCollect(out string collectedResourceType)
    {
        collectedResourceType = string.Empty;

        if (currentX > 0 || currentY > 0 || currentZ > 0)
        {
            float totalResources = currentX + currentY + currentZ;
            float randomValue = Random.Range(0, totalResources);

            if (randomValue < currentX)
            {
                collectedResourceType = "X";
                return true;
            }
            else if (randomValue < currentX + currentY)
            {
                collectedResourceType = "Y";
                return true;
            }
            else
            {
                collectedResourceType = "Z";
                return true;
            }
        }

        return false;
    }

    public bool HasRemainingResources()
    {
        if (currentX > 0 || currentY > 0 || currentZ > 0)
        {
            return true;
        }

        return false;
    }

    // 确认采集并减少资源
    public void ConfirmCollection(string resourceType)
    {
        switch (resourceType)
        {
            case "X":
                currentX--;
                break;
            case "Y":
                currentY--;
                break;
            case "Z":
                currentZ--;
                break;
        }

        CheckResourceDepletion();
    }
    
    // 确认采集并减少资源
    public int ConfirmCollection(string resourceType, int collectionAmount)
    {
        int confirmCount = 0;
        switch (resourceType)
        {
            case "X":
                if (currentX >= collectionAmount)
                {
                    confirmCount = collectionAmount;
                }
                else
                {
                    confirmCount = currentX;
                }
                currentX -= confirmCount;
                break;
            case "Y":
                if (currentY >= collectionAmount)
                {
                    confirmCount = collectionAmount;
                }
                else
                {
                    confirmCount = currentY;
                }
                currentY -= confirmCount;
                break;
            case "Z":
                if (currentZ >= collectionAmount)
                {
                    confirmCount = collectionAmount;
                }
                else
                {
                    confirmCount = currentZ;
                }
                currentZ -= confirmCount;
                break;
        }

        CheckResourceDepletion();
        return confirmCount;
    }

    // 检查资源是否耗尽
    private void CheckResourceDepletion()
    {
        if (currentX == 0 && currentY == 0 && currentZ == 0)
        {
            if (managerRef != null)
            {
                managerRef.ResourceDepletedCounter();
            }
            else
            {
                Debug.LogError("ResourceManager引用丢失。无法报告资源耗尽。");
            }
            gameObject.SetActive(false);
        }
    }

    // 设置资源点可见性
    private void SetVisibility(bool isVisible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = isVisible;
        if (resourceCollider != null) resourceCollider.enabled = isVisible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BacteriaC"))
        {
            Debug.Log($"{other.gameObject.name} 进入了资源点 {gameObject.name} 的范围！准备开始采集资源。");
            currentMatrixInstance = other.gameObject;
            StartCoroutine(CollectResourceRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BacteriaC"))
        {
            Debug.Log($"{other.gameObject.name} 离开了资源点 {gameObject.name} 的范围！");
            currentMatrixInstance = null;
            StopAllCoroutines();
        }
    }

    private IEnumerator CollectResourceRoutine()
    {
        while (currentMatrixInstance != null)
        {
            if (Time.time - lastCollectTime >= collectCooldown)
            {
                lastCollectTime = Time.time;

                string collectedResourceType;
                if (TryCollect(out collectedResourceType))
                {
                    if (currentMatrixInstance != null)
                    {
                        currentMatrixInstance.SendMessage("TryCollectResource", collectedResourceType, SendMessageOptions.DontRequireReceiver);
                        ConfirmCollection(collectedResourceType);
                        Debug.Log($"Resource collected: {collectedResourceType}");
                    }
                }
            }
            yield return null;
        }
    }
}
