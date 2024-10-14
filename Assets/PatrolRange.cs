using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolRange : MonoBehaviour
{
    [SerializeField] private Bacterial_Matrix matrix;
    public List<GameObject> entered_object;
    public List<GameObject> BacteriaD;
    public PatrolRange foe1Patrol;
    public PatrolRange foe2Patrol;
    public AudioSource DrumSource;
    public static bool is_increasing_Drum_sound=false;

    private void Awake() {
        matrix=this.GetComponentInParent<Bacterial_Matrix>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Bacteria_General>()!=null&&other.GetComponent<Bacteria_General>().Team!=matrix.Team)
        {
            entered_object.Add(other.gameObject);
        }
        if(other.GetComponent<BacteriaD>()!=null&&other.GetComponent<Bacteria_General>().Team==matrix.Team)
        {
            BacteriaD.Add(other.gameObject);
        }                

    }
    private void OnTriggerExit2D(Collider2D other) {

            entered_object.Remove(other.gameObject);
            if(other.GetComponent<BacteriaD>()!=null)
            {
                Debug.Log("force go home");
                other.GetComponent<BacteriaD>().ForceFind();
            }

            if(other.GetComponent<BacteriaD>()!=null&&other.GetComponent<Bacteria_General>().Team==matrix.Team)
            {
                BacteriaD.Remove(other.gameObject);
            }     

    }
    private void Update() {
        if(entered_object.Any())
        {
            if(DrumSource.volume<1)
            {
                is_increasing_Drum_sound=true;
                DrumSource.volume+=Time.deltaTime;
            }
            else
            {
                is_increasing_Drum_sound=false;
            }            
        }
        else if(!entered_object.Any()||entered_object.Count==0)
        {
            if(DrumSource.volume>0&&is_increasing_Drum_sound==false)
            DrumSource.volume-=Time.deltaTime;
        }
        if(entered_object.Count==0&&foe1Patrol.entered_object.Count==0&&foe2Patrol.entered_object.Count==0)
        {
            is_increasing_Drum_sound=false;
        }
    }
    //private void OnTriggerStay2D(Collider2D other) {
        
    //}
}
