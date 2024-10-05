using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_Data : MonoBehaviour
{

    public float[] production_rate={3,3,2};
    public static float productionCD=3;
    [SerializeField] float elapsedTime;
    void Awake()
    {
        //initialize data of scriptable objects
        foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_x=0;
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_y=0;
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_z=0;
            }
    }
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
            foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                //add resources to every bacterial matrix
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_x+=production_rate[0];
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_y+=production_rate[1];
                matrix.GetComponent<Bacterial_Matrix>().matrixdata.production_z+=production_rate[2];
            }
            
        }
    }

}
