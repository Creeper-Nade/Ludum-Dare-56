using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class player_matrix : MonoBehaviour
{
    public card_manager cmanager;
    [SerializeField] Animator animator;
    private GameObject instantiating_prefab; 

    public Global_Data data;
    public AudioSource audioSource;
    public AudioSource produce_sound;

    void Awake()
    {
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        data.Team1.Add(this.gameObject);
        animator=this.gameObject.GetComponentInChildren<Animator>();
        audioSource=GetComponent<AudioSource>();
    }  
    public void InstantiatePrefab()
    {
        animator.SetTrigger("is_producing");
        produce_sound.PlayOneShot(produce_sound.clip);
        instantiating_prefab=cmanager.selected_element.GetComponent<CardSettings>().card_stat.prefab;
        //data.Team1.Add(instantiating_prefab);
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position,out hit, 10.0f,1);
        var Cloned_bacteria=Instantiate(instantiating_prefab,hit.position,Quaternion.Euler(Vector3.forward*Random.Range(0.0f, 360.0f)));
        data.Team1.Add(Cloned_bacteria);
    }

    
}
