using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BacteriaB : MonoBehaviour
{
    [Header("reference")]
    [SerializeField] BacteriaSTATS stat;
    public Global_Data data;
    [SerializeField] Bacteria_General bacGen;
    [SerializeField] SpriteRenderer _spriteRenderer;
    public float range;
     Vector3 point;
    public Transform centrePoint; //centre of the area the agent wants to move around in
    [Header("AI")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] List<GameObject> foe;
    [SerializeField] Rigidbody2D rb;
    public Collider2D matrix_collider;
    [SerializeField] Bacterial_Matrix own_matrix;
    private void Awake()
    {
        //initialize components
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
        rb=gameObject.GetComponent<Rigidbody2D>();
        agent=GetComponent<NavMeshAgent>();
        bacGen=GetComponent<Bacteria_General>();
        
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        agent.speed=stat.speed;
        range=6;

       
    }
        void Start()
    {
        agent.updateRotation=false;
        agent.updateUpAxis=false;
        switch(bacGen.Team)
        {
            case 1:
            foreach(GameObject elements in data.Team1)
            {   
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>();
            }
            break;

            case 2:
            foreach(GameObject elements in data.Team2)
            {
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>(); 
            }
            break;

            case 3:
            foreach(GameObject elements in data.Team3)
            {
                if(elements.GetComponent<Bacterial_Matrix>()!=null)
                own_matrix=elements.GetComponent<Bacterial_Matrix>();  
            }
            break;

            default:
            break;
        }

        matrix_collider=own_matrix.gameObject.GetComponent<Collider2D>(); 
        centrePoint=own_matrix.transform;

        //detect
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Findpoint();
                    
            }    
            

    }
        public void Findpoint()
    {
        if (RandomPoint(centrePoint.position, range, out point)&&!matrix_collider.bounds.Contains(point)) //pass in our centre point and radius of area
        {
                        //UnityEngine.Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                        agent.SetDestination(point);
        }
        else{
            Findpoint();
        }
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)&&!matrix_collider.bounds.Contains(randomPoint)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

}
