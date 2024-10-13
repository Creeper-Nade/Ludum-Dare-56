using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy2matrix : MonoBehaviour
{
    [SerializeField] Global_Data data;
    [SerializeField] Animator animator;
    private GameObject instantiating_prefab; 
    [SerializeField] GameObject selected_product;
    [SerializeField] Bacterial_Matrix matrix;
    [SerializeField] PatrolRange patrolRange;
    private bool instantiate_cd_setted=false;
    public List<WeightedValue> weightedValues;
    [SerializeField] AudioSource audioSource;
    public AudioClip Produced_unit;
    void Awake()
    {
        patrolRange=this.gameObject.GetComponentInChildren<PatrolRange>();
        animator=this.gameObject.GetComponentInChildren<Animator>();
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        matrix=this.gameObject.GetComponent<Bacterial_Matrix>();
        audioSource=GetComponent<AudioSource>();
        data.Team3.Add(this.gameObject);
        GetRandomValue(weightedValues);
    }   
 
    void Update()
    {
        if(CalculateCost()==true&&instantiate_cd_setted==false)
        {
            instantiate_cd_setted=true;
            StartCoroutine(instantiate_cd());
        }

    }
    IEnumerator instantiate_cd()
    {
        yield return new WaitForSeconds(1);
        matrix.production_x-=selected_product.GetComponent<CardSettings>().card_stat.X_consume;
        matrix.production_y-=selected_product.GetComponent<CardSettings>().card_stat.Y_consume;
        matrix.production_z-=selected_product.GetComponent<CardSettings>().card_stat.Z_consume;
        InstantiatePrefab();
        instantiate_cd_setted=false;
        GetRandomValue(weightedValues);
    }
    public void InstantiatePrefab()
    {
        animator.SetTrigger("is_producing");
        audioSource.PlayOneShot(Produced_unit);
        instantiating_prefab=selected_product.GetComponent<CardSettings>().card_stat.prefab;
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position,out hit, 10.0f,1);
        var Cloned_bacteria=Instantiate(instantiating_prefab,hit.position,Quaternion.Euler(Vector3.forward*Random.Range(0.0f, 360.0f)));
        data.Team3.Add(Cloned_bacteria);
    }
    void GetRandomValue(List<WeightedValue> weightedValueList)
    {
        GameObject output = null;
 
        //Getting a random weight value
        var totalWeight = 0;
        foreach (var entry in weightedValueList)
        {
            totalWeight += entry.weight;
        }
        var rndWeightValue = Random.Range(1, totalWeight + 1);
 
        //Checking where random weight value falls
        var processedWeight = 0;
        foreach (var entry in weightedValueList)
        {
            processedWeight += entry.weight;
            if(rndWeightValue <= processedWeight)
            {
                output = entry.bacteria;
                break;
            }
        }
        if(patrolRange.BacteriaD.Count>=6)
        {
            GetRandomValue(weightedValues);
        }
        else selected_product=output;
    }
    private bool CalculateCost()
    {
        if(this.gameObject.GetComponent<Bacterial_Matrix>().production_x-selected_product.GetComponent<CardSettings>().card_stat.X_consume>=0)
        {
            if(matrix.production_y-selected_product.GetComponent<CardSettings>().card_stat.Y_consume>=0)
            {
                if(matrix.production_z-selected_product.GetComponent<CardSettings>().card_stat.Z_consume>=0)
                {
                    
                    return true;
                    //instantiate corresponding prefab
                }
                else return false;
            }
            else return false;
        }
        return false;
    }
}
