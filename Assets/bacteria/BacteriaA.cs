using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;
public class BacteriaA : MonoBehaviour
{
    [SerializeField] BacteriaSTATS stat;
    public Global_Data data;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [Header("AI")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] List<GameObject> foe;
    private float distance;
    private float nearestDistance=10000;

    [SerializeField] private GameObject nearestFoe;
    [SerializeField] Rigidbody2D rb;
    

    private void Awake()
    {
        //initialize components
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
        rb=gameObject.GetComponent<Rigidbody2D>();
        agent=GetComponent<NavMeshAgent>();



       
    }
    void Start()
    {
        _spriteRenderer.sprite=stat.sprite;
        agent.updateRotation=false;
        agent.updateUpAxis=false;

    }

    private void Update()
    {
        FindTarget();
        
        
    }
    private void FixedUpdate() {
        Vector3 targetDirection= nearestFoe.transform.position-this.transform.position;
        Quaternion targetRotation= Quaternion.LookRotation(targetDirection);
        rb.MoveRotation(targetRotation);
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
        agent.SetDestination(nearestFoe.transform.position);
    }
}
