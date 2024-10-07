using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bacteria_General : MonoBehaviour
{
    public BacteriaSTATS stat;
    public RTS_controll rts;

    private GameObject AttackArea;

    [SerializeField] Global_Data data;
    [SerializeField] Health_Bar healthBar;

    [SerializeField] ParticleSystem particle;

    [SerializeField] NavMeshAgent agent;
    public Animator animator;
    private SpriteRenderer sprite;
    private int Team;
    private bool death_coroutine_ran=false;
    public bool designated_destination=false;

        [Header("STATS")]
    [SerializeField] public float Health;
    [SerializeField] public float ATK;
    [SerializeField] public float ATK_speed;
    private bool is_attack_ready=true;
    [SerializeField] public float ATK_CD=15f;
    [SerializeField] public float speed;
    private void Awake()
    {
                //initialize bacteria
        Health=stat.health;
        ATK=stat.attack;
        ATK_speed=stat.atk_speed;
        ATK_CD/=ATK_speed;
        speed=stat.speed;

        sprite=gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        particle.gameObject.SetActive(false);

        AttackArea=this.gameObject.transform.GetChild(0).gameObject;
        AttackArea.SetActive(false);

        rts=FindObjectOfType<RTS_controll>();

        agent=this.gameObject.GetComponent<NavMeshAgent>();

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
                default:
                break;
            }
            if(death_coroutine_ran==false)
            {
                StartCoroutine(Die());
            }           
        }


        //rts thing

        // Check if we've reached the destination
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    designated_destination=false;
                    
                }
            }
}
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(!((this.gameObject.GetComponent<Team1bacteria>()!=null&&other.GetComponent<Team1bacteria>()!=null)||(this.gameObject.GetComponent<team2bacteria>()!=null&&other.GetComponent<team2bacteria>()!=null)||(this.gameObject.GetComponent<Team3bacteria>()!=null&&other.GetComponent<Team3bacteria>()!=null)))
        {
            AttackArea.SetActive(is_attack_ready);
            StartCoroutine(Attack_time());
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
        healthBar.Change(-damage);
        StartCoroutine("damaged_blink");
        Debug.Log("ouch");
    }

    IEnumerator damaged_blink()
    {
        Color defaultColor=sprite.color;
        sprite.color=Color.red;

        yield return new WaitForSeconds(0.05f);
        sprite.color=defaultColor;
    }

    IEnumerator Die()
    {
        death_coroutine_ran=true;
        particle.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
        death_coroutine_ran=false;
    }

    
    public void FaceDir()
    {
        Vector3 targetDirection= rts.startPos-this.transform.position;
        float angle=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;
        transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
    }
}
