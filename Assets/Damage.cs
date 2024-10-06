using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Damage : MonoBehaviour
{
    [SerializeField] Bacteria_General Bacteria_stats;
    [SerializeField] Bacteria_General Foe_stats;
    private int damage;
    private void Awake() {
        Bacteria_stats=transform.GetComponentInParent<Bacteria_General>();
    }
    private void OnTriggerEnter2D(Collider2D other) {

        damage=Bacteria_stats.stat.attack;
        if(other.GetComponent<Bacteria_General>()!=null)
        {
            Foe_stats=other.GetComponent<Bacteria_General>();
            Foe_stats.Damage(damage);
        }
        else if (other.GetComponent<Bacterial_Matrix>())
        {
            //write condition of that it has to attack with bacteria C, but bacterial A would still target matrix
        }
    }
}
