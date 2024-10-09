using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacC_Enemy_Detection : MonoBehaviour
{
        public Bacteria_General bacGen;

    [SerializeField] Global_Data data;
    public List<GameObject> entered_object;

    private void Awake() {
        bacGen=this.GetComponentInParent<Bacteria_General>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Bacteria_General>()!=null&&other.GetComponent<Bacteria_General>().Team!=bacGen.Team)
        {
            entered_object.Add(other.gameObject);
        }                

        }
    private void OnTriggerExit2D(Collider2D other) {

            entered_object.Remove(other.gameObject);

    }
}
