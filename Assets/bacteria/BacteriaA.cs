using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;
public class BacteriaA : MonoBehaviour
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
    [SerializeField] Animator animator;

    [Header("setup")]
    private float distance;
    private float nearestDistance=10000;

    [SerializeField] private GameObject nearestFoe;
    
    

    private void Awake()
    {
        //initialize components
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
        rb=gameObject.GetComponent<Rigidbody2D>();
        agent=GetComponent<NavMeshAgent>();
        Bacgen=GetComponent<Bacteria_General>();
        animator=this.gameObject.GetComponent<Animator>();
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        agent.speed=stat.speed;

       
    }
    void Start()
    {
        agent.updateRotation=false;
        agent.updateUpAxis=false;

    }

    private void Update()
    {
        FindTarget();
        
        
    }
    private void FixedUpdate() {
        if(Bacgen.designated_destination==false)
        {
            //rotate as foe
            Vector3 targetDirection= nearestFoe.transform.position-this.transform.position;
            float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
            transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
        }
    }

    void FindTarget()
    {
        if(gameObject.GetComponent<Team1bacteria>()!=null)
        {
            if(data.Team2.Any())
            foreach(GameObject bacteria in data.Team2)
            {
                foe.Add(bacteria);
            }
            if(data.Team3.Any())
            foreach(GameObject bacteria in data.Team3)
            {
                foe.Add(bacteria);
            }
        }
        if(gameObject.GetComponent<team2bacteria>()!=null)
        {
            if(data.Team1.Any())
            foreach(GameObject bacteria in data.Team1)
            {
                foe.Add(bacteria);
            }
            if(data.Team3.Any())
            foreach(GameObject bacteria in data.Team3)
            {
                foe.Add(bacteria);
            }
        }
        if(gameObject.GetComponent<Team3bacteria>()!=null)
        {
            if(data.Team1.Any())
            foreach(GameObject bacteria in data.Team1)
            {
                foe.Add(bacteria);
            }
            if(data.Team2.Any())
            foreach(GameObject bacteria in data.Team2)
            {
                foe.Add(bacteria);
            }
        }
        foreach(GameObject bacteria in foe)
        {
            distance=Vector3.Distance(this.transform.position,bacteria.transform.position);
            if(distance<nearestDistance)
            {
                nearestFoe=bacteria;
                nearestDistance=distance;
            }
        }
        foe.Clear();
        nearestDistance=100000;
        if(this.gameObject.GetComponent<Bacteria_General>().designated_destination==false)
        agent.SetDestination(nearestFoe.transform.position);
    }
}
