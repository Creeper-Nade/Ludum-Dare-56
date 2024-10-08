using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRange : MonoBehaviour
{
    [SerializeField] private Bacterial_Matrix matrix;
    public List<GameObject> entered_object;

    private void Awake() {
        matrix=this.GetComponentInParent<Bacterial_Matrix>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Bacteria_General>()!=null&&other.GetComponent<Bacteria_General>().Team!=matrix.Team)
        {
            entered_object.Add(other.gameObject);
        }                

    }
    private void OnTriggerExit2D(Collider2D other) {

            entered_object.Remove(other.gameObject);
            if(other.GetComponent<BacteriaD>()!=null)
            {
                Debug.Log("force go home");
                other.GetComponent<BacteriaD>().ForceFind();
            }  

    }
}
