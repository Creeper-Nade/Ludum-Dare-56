using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CombinationRange : MonoBehaviour
{
    [SerializeField] List<Bacteria_General> same_within_range;
    [SerializeField] Bacteria_General bacGen; 
    [SerializeField] BacteriaC BacC; 

    public CircleCollider2D patrolCollider;

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

        if(BacteriaType==3)
        {
            BacC=this.gameObject.GetComponentInParent<BacteriaC>();
        }
        
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
                bacGen.shield+=2;
                //bacteriaB shield effect
                break;

                case 3:
                BacC.maxTotalResources=12;
                //insert bacteriaC combination effect1
                break;

                case 4:
                bacGen.intake_recovery_activated=true;
                patrolCollider=this.gameObject.GetComponentInParent<BacteriaD>().patrolRange.gameObject.GetComponent<CircleCollider2D>();
                patrolCollider.radius=2;
                bacGen.gameObject.GetComponentInParent<BacteriaD>().range=12;
                //insert bacteria D recovering effect;
                break;
                default:
                break;
            }

            //second upgrade
            if(same_within_range.Count>=bacGen.stat.number2_for_combination_upgrade)
            {
                if(Upgraded_particle2!=null)Upgraded_particle2.gameObject.SetActive(true);
                if(BacteriaType==3)
                {
                    BacC.maxTotalResources=15;//set bacteriaC 2nd effect
                    bacGen.speed=10;
                    bacGen.agent.speed=bacGen.speed;
                }
            }
            else
            {
                //reset bacteriaC 2nd effect;
                if(BacteriaType==3)
                {
                    BacC.maxTotalResources=12;//set bacteriaC 2nd effect
                    bacGen.speed=8;
                    bacGen.agent.speed=bacGen.speed;
                }
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
                bacGen.shield=0;
                //bacteriaB shield effect
                break;

                case 3:
                BacC.maxTotalResources=10;//set bacteriaC 2nd effect
                bacGen.speed=8;
                bacGen.agent.speed=bacGen.speed;
                //insert bacteriaC combination effect1
                break;

                case 4:
                bacGen.intake_recovery_activated=false;
                patrolCollider=this.gameObject.GetComponentInParent<BacteriaD>().patrolRange.gameObject.GetComponent<CircleCollider2D>();
                patrolCollider.radius=1.5f;
                bacGen.gameObject.GetComponentInParent<BacteriaD>().range=8;
                //insert bacteria D recovering effect;
                break;
                default:
                break;
            }
            Upgraded_particle1.gameObject.SetActive(false);

            
        }
        
    }
}
