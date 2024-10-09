using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_matrix : MonoBehaviour
{
    public card_manager cmanager;
    [SerializeField] Animator animator;
    private GameObject instantiating_prefab; 

    public Global_Data data;

    void Awake()
    {
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        data.Team1.Add(this.gameObject);
        animator=this.gameObject.GetComponentInChildren<Animator>();
    }  
    public void InstantiatePrefab()
    {
        animator.SetTrigger("is_producing");
        instantiating_prefab=cmanager.selected_element.GetComponent<CardSettings>().card_stat.prefab;
        //data.Team1.Add(instantiating_prefab);
        var Cloned_bacteria=Instantiate(instantiating_prefab,transform.position,Quaternion.Euler(Vector3.forward*Random.Range(0.0f, 360.0f)));
        data.Team1.Add(Cloned_bacteria);
    }

    
}
