using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Creat_Enemy_Num
{
    public int production_x;
    public float production_y;
    public float production_z;
    public bool canPerhaps;
}


public enum EnemyType
{
    enemyA,
    enemyB,
    enemyC,
    enemyD
}

public class Creat_Enemy_Controller : MonoBehaviour
{

    Bacterial_Matrix bacterial;

    public Creat_Enemy_Num creatA_Enemy_Nums;
    public Creat_Enemy_Num creatB_Enemy_Nums;
    public Creat_Enemy_Num creatC_Enemy_Nums;
    public Creat_Enemy_Num creatD_Enemy_Nums;

    [SerializeField] Global_Data data;
    public GameObject[] enemys;
    public TeamType team;
    public TeamData teamData;

    public int[] restrict;
    public int productionCycleTime_1 = 2 * 60;
    public int productionCycleTime_2 = 7 * 60;
    private float startTime;
    private int bacteriaACount = 0;
    private int bacteriaBCount = 0;
    private int bacteriaCCount = 0;
    private int bacteriaDCount = 0;
    void Start()
    {
        bacterial = GetComponent<Bacterial_Matrix>();
        data.TeamDatas.Add(team,teamData);
        Global_Data.Instance.TeamDatas[team].EnemyData.Add(EnemyType.enemyA, 0);
        Global_Data.Instance.TeamDatas[team].EnemyData.Add(EnemyType.enemyB, 0);
        Global_Data.Instance.TeamDatas[team].EnemyData.Add(EnemyType.enemyC, 0);
        Global_Data.Instance.TeamDatas[team].EnemyData.Add(EnemyType.enemyD, 0);
    }

 

    void Update()
    {
        if (Time.time - startTime < productionCycleTime_1|| bacteriaBCount<3)
        {
            // 在前2分钟内按照特定规则生产  
            ProduceBasedOnPriority_1();
        }
        else if (Time.time - startTime < productionCycleTime_2)
        {
            ProduceBasedOnPriority_2();
        }
        else
        {
            ProduceBasedOnPriority_3();
        }
    }

    void ProduceBasedOnPriority_1()
    {
        if (bacteriaCCount < 2)
        {
            if(CreatEnemyC())
            bacteriaCCount +=1;
        }
        else if (bacteriaCCount == 2&& bacteriaACount!= 2)
        {
            if (CreatEnemyA())
            {
                if (bacteriaACount < 2)
                {
                    bacteriaCCount = 0;
                    bacteriaACount += 1;
                }
            }
        }
        else if (bacteriaACount==2)
        {
            if (CreatEnemyB())
            {
                bacteriaBCount += 1;
                bacteriaCCount = 0;
                bacteriaACount = 0;
            }
        }
       
    }

    void ProduceBasedOnPriority_2()
    {
        if (Global_Data.Instance.TeamDatas[team].EnemyData[EnemyType.enemyC]<=6)
        {
            if (bacteriaCCount < 2)
            {
                if (CreatEnemyC())
                {
                    bacteriaCCount += 1;

                }
            }
            else if (bacteriaCCount == 2 && bacteriaACount != 2)
            {
                if (CreatEnemyA())
                {
                        bacteriaCCount = 0;
                        bacteriaACount += 1;
                }
            }
        }
        else
        {
            if (bacteriaACount < 2)
            {
                if (CreatEnemyA())
                {
                    bacteriaACount += 1;
                }
            }
            else if (bacteriaACount == 2&& bacteriaBCount!=4)
            {
                if (CreatEnemyB())
                {
                    bacteriaACount = 0;
                    bacteriaBCount += 1;
                }
            }
            else if (bacteriaBCount==4 && Global_Data.Instance.TeamDatas[team].EnemyData[EnemyType.enemyD] < 6 )
            {
                if (CreatEnemyD())
                {
                    bacteriaDCount += 1;
                    bacteriaBCount = 0;
                }
            }else if (Global_Data.Instance.TeamDatas[team].EnemyData[EnemyType.enemyD] > 6)
            {
                bacteriaACount = 0;
                bacteriaBCount = 0;
            }
        }
    }

    void ProduceBasedOnPriority_3()
    {
        if (Global_Data.Instance.TeamDatas[team].EnemyData[EnemyType.enemyC] <= 12)
        {
            if (bacteriaCCount < 2)
            {
                if (CreatEnemyC())
                {
                    bacteriaCCount += 1;

                }
            }
            else if (bacteriaCCount == 2 && bacteriaACount != 2)
            {
                if (CreatEnemyA())
                {
                    bacteriaCCount = 0;
                    bacteriaACount += 1;
                }
            }
            else if (bacteriaACount == 2)
            {
                if (CreatEnemyB())
                {
                    bacteriaBCount += 1;
                    bacteriaCCount = 0;
                    bacteriaACount = 0;
                }
            }
        }
        else
        {
            if (bacteriaACount < 3)
            {
                if (CreatEnemyA())
                {
                    bacteriaACount += 1;
                }
            }
            else if (bacteriaACount == 3 && bacteriaBCount != 3)
            {
                if (CreatEnemyB())
                {
                    bacteriaACount = 0;
                    bacteriaBCount += 1;
                }
            }
            else if (bacteriaBCount == 3&& bacteriaDCount<6)
            {
                if (CreatEnemyD())
                {
                    bacteriaDCount += 1;
                }
            }
        }
    }

    bool CreatEnemyA()
    {
        if (creatA_Enemy_Nums.canPerhaps)
        {
            if (bacterial.production_x > creatA_Enemy_Nums.production_x || bacterial.production_y > creatA_Enemy_Nums.production_y )
            {
                GameObject enemya = GameObject.Instantiate(enemys[0]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyA);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bacterial.production_x > creatA_Enemy_Nums.production_x && bacterial.production_y > creatA_Enemy_Nums.production_y && bacterial.production_z > creatA_Enemy_Nums.production_z)
            {
                bacterial.production_x -= creatA_Enemy_Nums.production_x;
                bacterial.production_y -= creatA_Enemy_Nums.production_y;
                bacterial.production_z -= creatA_Enemy_Nums.production_z;
                GameObject enemya = GameObject.Instantiate(enemys[0]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyA);
                Debug.LogError(Global_Data.Instance.TeamDatas[team].EnemyData[EnemyType.enemyA]);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    bool CreatEnemyB()
    {
        if (creatB_Enemy_Nums.canPerhaps)
        {
            if (bacterial.production_x > creatB_Enemy_Nums.production_x || bacterial.production_y > creatB_Enemy_Nums.production_y)
            {
                GameObject enemyb = GameObject.Instantiate(enemys[1]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyB);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bacterial.production_x > creatB_Enemy_Nums.production_x && bacterial.production_y > creatB_Enemy_Nums.production_y && bacterial.production_z > creatB_Enemy_Nums.production_z)
            {
                bacterial.production_x -= creatB_Enemy_Nums.production_x;
                bacterial.production_y -= creatB_Enemy_Nums.production_y;
                bacterial.production_z -= creatB_Enemy_Nums.production_z;
                GameObject enemyb = GameObject.Instantiate(enemys[1]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyB);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    bool CreatEnemyC()
    {
        if (creatC_Enemy_Nums.canPerhaps)
        {
            if (bacterial.production_x > creatC_Enemy_Nums.production_x  )
            {
                bacterial.production_x -= creatC_Enemy_Nums.production_x;
                GameObject enemyb = GameObject.Instantiate(enemys[2]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyC);
                return true;

            }
            else if (bacterial.production_y > creatC_Enemy_Nums.production_y)
            {
                bacterial.production_y -= creatC_Enemy_Nums.production_y;
                GameObject enemyb = GameObject.Instantiate(enemys[2]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyC);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bacterial.production_x > creatC_Enemy_Nums.production_x && bacterial.production_y > creatC_Enemy_Nums.production_y && bacterial.production_z > creatC_Enemy_Nums.production_z)
            {
                bacterial.production_x -= creatC_Enemy_Nums.production_x;
                bacterial.production_y -= creatC_Enemy_Nums.production_y;
                bacterial.production_z -= creatC_Enemy_Nums.production_z;
                GameObject enemyb = GameObject.Instantiate(enemys[2]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyC);
                return true;
            }
            else
            {
                return false;

            }
        }
    }

    bool CreatEnemyD()
    {
        if (creatD_Enemy_Nums.canPerhaps)
        {
            if (bacterial.production_x > creatD_Enemy_Nums.production_x || bacterial.production_y > creatD_Enemy_Nums.production_y)
            {
                GameObject enemyd = GameObject.Instantiate(enemys[3]);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bacterial.production_x > creatD_Enemy_Nums.production_x && bacterial.production_y > creatD_Enemy_Nums.production_y && bacterial.production_z > creatD_Enemy_Nums.production_z)
            {
                bacterial.production_x -= creatD_Enemy_Nums.production_x;
                bacterial.production_y -= creatD_Enemy_Nums.production_y;
                bacterial.production_z -= creatD_Enemy_Nums.production_z;
                GameObject enemyb = GameObject.Instantiate(enemys[3]);
                Global_Data.Instance.TeamDatas[team].AddEnemy(EnemyType.enemyD);
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}
