using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CombinationRange : MonoBehaviour
{
    [SerializeField] List<Bacteria_General> same_within_range;
    [SerializeField] Bacteria_General bacGen; 

    public ParticleSystem Upgraded_particle1;
    public ParticleSystem Upgraded_particle2;
    [SerializeField] private Global_Data data;
    private int BacteriaType;

    private void Awake() {
        same_within_range= new List<Bacteria_General>();

        bacGen=this.gameObject.GetComponentInParent<Bacteria_General>();
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        Upgraded_particle1.gameObject.SetActive(false);
        if(Upgraded_particle2!=null)
        Upgraded_particle2.gameObject.SetActive(false);

        same_within_range.Add(bacGen);
        if(this.gameObject.GetComponentInParent<BacteriaA>()!=null)BacteriaType=1;
        if(this.gameObject.GetComponentInParent<BacteriaB>()!=null)BacteriaType=2;
        if(this.gameObject.GetComponentInParent<BacteriaC>()!=null)BacteriaType=3;
        if(this.gameObject.GetComponentInParent<BacteriaD>()!=null)BacteriaType=4;
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if((data.Team1.Contains(this.transform.parent.gameObject)&&data.Team1.Contains(other.gameObject))||(data.Team2.Contains(this.transform.parent.gameObject)&&data.Team2.Contains(other.gameObject))||(data.Team3.Contains(this.transform.parent.gameObject)&&data.Team3.Contains(other.gameObject)))
        {
            if((other.GetComponent<BacteriaA>()!=null&&BacteriaType==1)||(other.GetComponent<BacteriaB>()!=null&&BacteriaType==2)||(other.GetComponent<BacteriaC>()!=null&&BacteriaType==3)||(other.GetComponent<BacteriaD>()!=null&&BacteriaType==4))
            {
                if(!same_within_range.Contains(other.GetComponent<Bacteria_General>()))
                {
                    same_within_range.Add(other.GetComponent<Bacteria_General>());
                }
            }
            
        }
                
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(same_within_range.Contains(other.GetComponent<Bacteria_General>()))
        {
            same_within_range.Remove(other.GetComponent<Bacteria_General>());
        }
        
    }
    private void Update() {
        //1st upgrade
        if(same_within_range.Count>=bacGen.stat.number1_for_combination)
        {
            Upgraded_particle1.gameObject.SetActive(true);
            switch(BacteriaType)
            {
                case 1:
                bacGen.ATK=7;
                break;

                case 2:
                //bacteriaB shield effect
                break;

                case 3:
                //insert bacteriaC combination effect1
                break;

                case 4:
                //insert bacteria D recovering effect;
                break;
                default:
                break;
            }

            //second upgrade
            if(same_within_range.Count>=bacGen.stat.number2_for_combination_upgrade)
            {
                Upgraded_particle2.gameObject.SetActive(true);
                if(BacteriaType==3)
                {
                    //set bacteriaC 2nd effect
                }
            }
            else
            {
                //reset bacteriaC 2nd effect;
                Upgraded_particle2.gameObject.SetActive(false);
            }
        }
        else{
            switch(BacteriaType)
            {
                case 1:
                bacGen.ATK=5;
                break;

                case 2:
                //bacteriaB shield effect
                break;

                case 3:
                //insert bacteriaC combination effect1
                break;

                case 4:
                //insert bacteria D recovering effect;
                break;
                default:
                break;
            }
            Upgraded_particle1.gameObject.SetActive(false);

            
        }
        
    }
}
