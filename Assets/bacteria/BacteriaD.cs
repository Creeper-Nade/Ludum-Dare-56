using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BacteriaD : MonoBehaviour
{
    public NavMeshAgent agent;

    public Bacteria_General bacGen;

    [SerializeField] Global_Data data;
    public PatrolRange patrolRange;
    public float range; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area
    public Collider2D matrix_collider;
    [SerializeField] Bacterial_Matrix own_matrix;
        [Header("setup")]
    private float distance;
    private float nearestDistance=10000;

    Vector3 point;

    [SerializeField] private GameObject nearestFoe;
    [SerializeField] List<GameObject> Collided_allies;
    private bool RanPosResetDelayCoroutine=false;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation=false;
        agent.updateUpAxis=false;
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        bacGen=this.gameObject.GetComponent<Bacteria_General>();
        

    }

    private void Start() {
        //UnityEngine.Debug.Log(bacGen.Team);
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
        patrolRange=own_matrix.gameObject.GetComponentInChildren<PatrolRange>();

        matrix_collider=own_matrix.gameObject.GetComponent<Collider2D>(); 
        centrePoint=own_matrix.transform;
        //UnityEngine.Debug.Log(own_matrix.name);
    }
    void Update()
    {
        if(agent.remainingDistance <= 1&&FindTarget()==false&&bacGen.designated_destination==false) //done with path
        {
                    
                if (RandomPoint(centrePoint.position, range, out point)&&!matrix_collider.bounds.Contains(point)) //pass in our centre point and radius of area
                {
                    Findpoint();
                }
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Findpoint();
                    
                }    
            
        }
        if(Collided_allies.Any()&&RanPosResetDelayCoroutine==false)
        {
            StartCoroutine(DelayReset());
            RanPosResetDelayCoroutine=true;
        }

    }
    IEnumerator DelayReset()
    {
        yield return new WaitForSeconds(3);
        if(FindTarget()==false&&bacGen.designated_destination==false)
        Findpoint();
        RanPosResetDelayCoroutine=false;
    }
    public void Findpoint()
    {
        if (RandomPoint(centrePoint.position, range, out point)&&!matrix_collider.bounds.Contains(point)) //pass in our centre point and radius of area
        {
                        //UnityEngine.Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                        agent.SetDestination(point);
        }
    }
    public void ForceFind()
    {
        if (ForceBackRandomPoint(centrePoint.position, range, out point)&&!matrix_collider.bounds.Contains(point)) //pass in our centre point and radius of area
        {
                        //UnityEngine.Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                        agent.SetDestination(point);
        }
    }
    private void FixedUpdate() {
        if(bacGen.designated_destination==false&&FindTarget()==true)
        {
            //rotate as foe
            Vector3 targetDirection= nearestFoe.transform.position-this.transform.position;
            float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
            transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
        }
        else if(bacGen.designated_destination==false)
        {
            //rotate to patrol destination
            Vector3 targetDirection= point-this.transform.position;
            float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
            transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
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

    bool ForceBackRandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, Mathf.Infinity, NavMesh.AllAreas)&&!matrix_collider.bounds.Contains(randomPoint)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


        bool FindTarget()
    {    
        if(patrolRange.entered_object.Any())
        {
            foreach(GameObject bacteria in patrolRange.entered_object)
            {
                distance=Vector3.Distance(this.transform.position,bacteria.transform.position);
                if(distance<nearestDistance)
                {
                    nearestFoe=bacteria;
                    nearestDistance=distance;
                }
            }
            nearestDistance=100000;
            if(bacGen.designated_destination==false)
            {
                agent.SetDestination(nearestFoe.transform.position);
                return true;
            }
            else return false;
        }
        else{
            return false;
        }    
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.GetComponent<Bacteria_General>()!=null&&other.gameObject.GetComponent<Bacteria_General>().Team==bacGen.Team)
        {
            if(FindTarget()==false&&bacGen.designated_destination==false)
            {
                Collided_allies.Add(other.gameObject);
                Findpoint();

            }
        }
        
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(Collided_allies.Contains(other.gameObject))
        {
            Collided_allies.Remove(other.gameObject);
        }
    }
}
