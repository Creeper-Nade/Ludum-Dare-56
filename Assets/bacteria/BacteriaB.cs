using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BacteriaB : MonoBehaviour
{
    [Header("reference")]
    [SerializeField] BacteriaSTATS stat;
    public Global_Data data;
    [SerializeField] Bacteria_General Bacgen;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [Header("AI")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] List<GameObject> foe;
    [SerializeField] Rigidbody2D rb;
    private void Awake()
    {
        //initialize components
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
        rb=gameObject.GetComponent<Rigidbody2D>();
        agent=GetComponent<NavMeshAgent>();
        Bacgen=GetComponent<Bacteria_General>();
        
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        agent.speed=stat.speed;

       
    }
        void Start()
    {
        agent.updateRotation=false;
        agent.updateUpAxis=false;

    }

}
