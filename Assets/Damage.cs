using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Damage : MonoBehaviour
{
    [SerializeField] Bacteria_General Bacteria_stats;
    [SerializeField] Bacteria_General Foe_stats;
    [SerializeField] Global_Data data;
    private int damage;
    private void Awake() {
        Bacteria_stats=transform.GetComponentInParent<Bacteria_General>();
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
    }
    private void OnTriggerEnter2D(Collider2D other) {

        damage=Bacteria_stats.stat.attack;
        Bacteria_stats.animator.SetTrigger("is_attacking");
        if(other.GetComponent<Bacteria_General>()!=null)
        {
            Foe_stats=other.GetComponent<Bacteria_General>();
            if(!((data.Team1.Contains(Bacteria_stats.gameObject)&&data.Team1.Contains(Foe_stats.gameObject))||(data.Team2.Contains(Bacteria_stats.gameObject)&&data.Team2.Contains(Foe_stats.gameObject))||(data.Team3.Contains(Bacteria_stats.gameObject)&&data.Team3.Contains(Foe_stats.gameObject))))
            Foe_stats.Damage(damage);
        }
        else if (other.GetComponent<Bacterial_Matrix>())
        {
            //write condition of that it has to attack with bacteria C, but bacterial A would still target matrix
        }

    }
}
