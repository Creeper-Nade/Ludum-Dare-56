using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_Data : MonoBehaviour
{
    public float[] production_rate={3,3,2};
    public float[] production_x=new float[999];
    public float[] production_y=new float[999];
    public float[] production_z=new float[999];
    public static float productionCD=3;
    [SerializeField] float elapsedTime;
    void Start()
    {
        StartCoroutine(AddProduction());
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
            for(int i=0;i<=2;i++)
            {
                production_x[i]+=production_rate[0];
                production_y[i]+=production_rate[1];
                production_z[i]+=production_rate[2];
                Debug.Log("x"+i+production_x[i]);
                Debug.Log("y"+i+production_y[i]);
                Debug.Log("z"+i+production_z[i]);
            }
            
        }
    }

}
