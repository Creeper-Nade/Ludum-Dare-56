using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TeamData
{
    public Dictionary<EnemyType, int> EnemyData=new Dictionary<EnemyType, int>();
    public void AddEnemy(EnemyType enemyType)
    {
        if (!EnemyData.ContainsKey(enemyType))
        {
            EnemyData.Add(enemyType,0);
        }
      
        EnemyData[enemyType] += 1;
    }

    public void RemoveEnemy(EnemyType enemyType)
    {
        if (!EnemyData.ContainsKey(enemyType))
        {
            EnemyData.Add(enemyType, 0);
        }
        Debug.LogError("xxxxxxxxxx");
        EnemyData[enemyType] -= 1;
    }
}

public enum TeamType
{
    Team1,
    Team2,
    Team3
}
public class Global_Data : MonoBehaviour
{
    [Header("production")]
    public float[] production_rate={3,3,2};
    public static float productionCD=3;
    [SerializeField] float elapsedTime;

    [Header("list of objects")]
    public List<GameObject> Team1;
    public List<GameObject> Team2;
    public List<GameObject> Team3;
    public Dictionary<TeamType, TeamData> TeamDatas = new Dictionary<TeamType, TeamData>();

    public static Global_Data Instance;
    void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        //initialize data of scriptable objects
        foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                matrix.GetComponent<Bacterial_Matrix>().production_x=0;
                matrix.GetComponent<Bacterial_Matrix>().production_y=0;
                matrix.GetComponent<Bacterial_Matrix>().production_z=0;
            }
    }

    void Start()
    {
        StartCoroutine(AddProduction());
    }

    void InitDic()
    {
        
    }

    void Update()
    {
        elapsedTime+=Time.deltaTime;
        int minutes=Mathf.FloorToInt(elapsedTime/60);
        if(minutes>=3)
        {
            if(minutes>=6)
            {
                production_rate[0]=6;
                production_rate[1]=6;
                production_rate[2]=3;
                productionCD=8;
            }
            else{
                production_rate[0]=4;
            production_rate[1]=4;
            production_rate[2]=2;
            }
            
        }
    }

    IEnumerator AddProduction()
    {
        while(true)
        {
            yield return new WaitForSeconds(productionCD);
            foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                //add resources to every bacterial matrix
                matrix.GetComponent<Bacterial_Matrix>().production_x+=production_rate[0];
                matrix.GetComponent<Bacterial_Matrix>().production_y+=production_rate[1];
                matrix.GetComponent<Bacterial_Matrix>().production_z+=production_rate[2];
            }
            
        }
    }

}
