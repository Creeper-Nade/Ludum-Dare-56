using System.Collections;
using System.Collections.Generic;
using System.Data;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;
public class BacteriaA : MonoBehaviour
{
    [SerializeField] BacteriaSTATS stat;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [Header("AI")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] List<GameObject> foe;
    private float distance;
    private float nearestDistance=10000;

    [SerializeField] private GameObject nearestFoe;
    public Global_Data data;
    
    private void Awake()
    {
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
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

    void FindTarget()
    {
        if(gameObject.GetComponent<Team1bacteria>()!=null)
        {
            Debug.Log("in team 1");
            foreach(GameObject bacteria in data.Team2)
            {
                foe.Add(bacteria);
            }
            foreach(GameObject bacteria in data.Team3)
            {
                foe.Add(bacteria);
            }
        }
        if(gameObject.GetComponent<team2bacteria>()!=null)
        {
            foreach(GameObject bacteria in data.Team1)
            {
                foe.Add(bacteria);
            }
            foreach(GameObject bacteria in data.Team3)
            {
                foe.Add(bacteria);
            }
        }
        if(gameObject.GetComponent<Team3bacteria>()!=null)
        {
            foreach(GameObject bacteria in data.Team1)
            {
                foe.Add(bacteria);
            }
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
