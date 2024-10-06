using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bacteria_General : MonoBehaviour
{
    public BacteriaSTATS stat;

    private GameObject AttackArea;

    [SerializeField] Global_Data data;
    private int Team;

        [Header("STATS")]
    [SerializeField] float Health;
    [SerializeField] float ATK;
    [SerializeField] float ATK_speed;
    private bool is_attack_ready=true;
    [SerializeField] float ATK_CD=15f;
    [SerializeField] float speed;
    private void Awake()
    {
                //initialize bacteria
        Health=stat.health;
        ATK=stat.attack;
        ATK_speed=stat.atk_speed;
        ATK_CD/=ATK_speed;
        speed=stat.speed;

        AttackArea=this.gameObject.transform.GetChild(0).gameObject;
        AttackArea.SetActive(false);

        if(this.gameObject.GetComponent<Team1bacteria>()!=null)Team=1;
        if(this.gameObject.GetComponent<team2bacteria>()!=null)Team=2;
        if(this.gameObject.GetComponent<Team3bacteria>()!=null)Team=3;
    }

    private void Update() {
        if(is_attack_ready==false)
        {
            ATK_CD-=Time.deltaTime;
            if(ATK_CD<=0)
            {
                is_attack_ready=true;
                ATK_CD=15f/ATK_speed;
            }
        }
        if(Health<=0)
        {
            Debug.Log("I am dead");
            switch(Team)
            {
                case 1:
                data.Team1.Remove(this.gameObject);
                break;
                
                case 2:
                data.Team2.Remove(this.gameObject);
                break;

                case 3:
                data.Team3.Remove(this.gameObject);
                break;
            }
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(!((this.gameObject.GetComponent<Team1bacteria>()!=null&&other.GetComponent<Team1bacteria>()!=null)||(this.gameObject.GetComponent<team2bacteria>()!=null&&other.GetComponent<team2bacteria>()!=null)||(this.gameObject.GetComponent<Team3bacteria>()!=null&&other.GetComponent<Team3bacteria>()!=null)))
        {
            AttackArea.SetActive(is_attack_ready);
            StartCoroutine("Attack_time");
        }
        
    }

    IEnumerator Attack_time()
    {
        yield return new WaitForSeconds(0.25f);
        is_attack_ready=false;
    }
    public void Damage(int damage)
    {
        Health-=damage;
        Debug.Log("ouch");
    }
}
