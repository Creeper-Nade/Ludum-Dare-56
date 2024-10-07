using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Damage : MonoBehaviour
{
    [SerializeField] Bacteria_General Bacteria_stats;
    [SerializeField] Bacteria_General Foe_stats;
    [SerializeField] Bacterial_Matrix Foe_matrix;
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
            Bacteria_stats.is_attack_ready=false;
            this.gameObject.SetActive(false);
        }
        else if (other.GetComponent<Bacterial_Matrix>()!=null)
        {
            Foe_matrix=other.GetComponent<Bacterial_Matrix>();
            if(!((data.Team1.Contains(Bacteria_stats.gameObject)&&data.Team1.Contains(Foe_matrix.gameObject))||(data.Team2.Contains(Bacteria_stats.gameObject)&&data.Team2.Contains(Foe_matrix.gameObject))||(data.Team3.Contains(Bacteria_stats.gameObject)&&data.Team3.Contains(Foe_matrix.gameObject))))
            {
                Foe_matrix.Damage(damage);
                Bacteria_stats.is_attack_ready=false;
                this.gameObject.SetActive(false);
            }
        }

    }
}
