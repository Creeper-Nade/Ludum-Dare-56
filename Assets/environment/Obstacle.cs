using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private bool damaged=false;
    private int damage;
    public Bacteria_General Foe_stats;

    private void Awake() {
        damage=2;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.GetComponent<Bacteria_General>()!=null)
        {
            Foe_stats=other.GetComponent<Bacteria_General>();
            if(damaged==false)
            {
                StartCoroutine(damageCD());
                damaged=true;
            }
            
        }
                
    }
    IEnumerator damageCD()
    {
        Foe_stats.Damage(damage);
        yield return new WaitForSeconds(0.5f);
        damaged=false;
    }
}
