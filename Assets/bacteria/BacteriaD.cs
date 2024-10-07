using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class BacteriaD : MonoBehaviour
{
    public NavMeshAgent agent;

    public Bacteria_General bacGen;

    [SerializeField] Global_Data data;
    public float range; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area
    [SerializeField] Bacterial_Matrix own_matrix;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation=false;
        agent.updateUpAxis=false;
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        bacGen=this.gameObject.GetComponent<Bacteria_General>();
        

        switch(bacGen.Team)
        {
            case 1:
            foreach(GameObject elements in data.Team1)
            {
                own_matrix=elements.GetComponent<Bacterial_Matrix>(); 
            }
            break;

            case 2:
            foreach(GameObject elements in data.Team2)
            {
                own_matrix=elements.GetComponent<Bacterial_Matrix>(); 
            }
            break;

            case 3:
            foreach(GameObject elements in data.Team3)
            {
                own_matrix=elements.GetComponent<Bacterial_Matrix>(); 
            }
            break;

            default:
            break;
        }
        centrePoint=own_matrix.transform;
        UnityEngine.Debug.Log(own_matrix.name);

    }
    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
            {
                UnityEngine.Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }
        }

    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
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
