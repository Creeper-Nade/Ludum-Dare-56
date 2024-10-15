using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private bool damaged=false;
    private int damage;
    public Bacteria_General Foe_stats;
    [SerializeField] List<GameObject> Collided_obj;
    private void Awake() {
        damage=2;
    }
    private void Update() {
        if(Collided_obj.Any()&&damaged==false)
        {
            foreach(GameObject collided_bac in Collided_obj)
            {
                Foe_stats=collided_bac.GetComponent<Bacteria_General>();
                StartCoroutine(damageCD());
                damaged=true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Bacteria_General>()!=null)
        {
            Collided_obj.Add(other.gameObject);
                        
        }
                
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(Collided_obj.Contains(other.gameObject))
        {
            Collided_obj.Remove(other.gameObject);
                        
        }
                
    }
    IEnumerator damageCD()
    {
        Foe_stats.Damage(damage);
        yield return new WaitForSeconds(0.5f);
        damaged=false;
    }
}
