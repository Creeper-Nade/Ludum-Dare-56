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
    [SerializeField]private SpriteRenderer sprite;
    private int Team;
    private bool death_coroutine_ran=false;
    public bool designated_destination=false;

        [Header("STATS")]
    [SerializeField] public float Health;
    [SerializeField] public float ATK;
    [SerializeField] public float ATK_speed;
    public bool is_attack_ready=true;
    [SerializeField] public float ATK_CD=15f;
    [SerializeField] public float speed;

    public int shield;
    Color defaultColor;
    private void Awake()
    {
                //initialize bacteria
        Health=stat.health;
        ATK=stat.attack;
        ATK_speed=stat.atk_speed;
        ATK_CD/=ATK_speed;
        speed=stat.speed;
        shield=0;

        sprite=gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        particle.gameObject.SetActive(false);

        AttackArea=this.gameObject.transform.GetChild(0).gameObject;
        AttackArea.SetActive(false);

        rts=FindObjectOfType<RTS_controll>();

        agent=this.gameObject.GetComponent<NavMeshAgent>();

        if(this.gameObject.GetComponent<Team1bacteria>()!=null)Team=1;
        if(this.gameObject.GetComponent<team2bacteria>()!=null)Team=2;
        if(this.gameObject.GetComponent<Team3bacteria>()!=null)Team=3;
        defaultColor=sprite.color;
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
    private void OnCollisionStay2D(Collision2D other)
    {
        if(!((data.Team1.Contains(this.gameObject)&&data.Team1.Contains(other.gameObject))||(data.Team2.Contains(this.gameObject)&&data.Team2.Contains(other.gameObject))||(data.Team3.Contains(this.gameObject)&&data.Team3.Contains(other.gameObject))))
        {
            //!((this.gameObject.GetComponent<Team1bacteria>()!=null&&(other.gameObject.GetComponent<Team1bacteria>()!=null||other.gameObject.GetComponentInParent<Team1bacteria>()!=null))||(this.gameObject.GetComponent<team2bacteria>()!=null&&(other.gameObject.GetComponent<team2bacteria>()!=null||other.gameObject.GetComponentInParent<team2bacteria>()!=null))||(this.gameObject.GetComponent<Team3bacteria>()!=null&&(other.gameObject.GetComponent<Team3bacteria>()!=null||other.gameObject.GetComponentInParent<Team3bacteria>()!=null)))
            Debug.Log("attack");
            AttackArea.SetActive(is_attack_ready);
            if(is_attack_ready==true)
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
        if(shield>0)
        {
            shield-=1;
        }
        else{
            Health-=damage;
            Debug.Log(damage);
            healthBar.Change(-damage);
            StartCoroutine("damaged_blink");
            Debug.Log("ouch");
        }
        
    }

    IEnumerator damaged_blink()
    {
        sprite.color=Color.red;

        yield return new WaitForSeconds(0.05f);
        Debug.Log("color back to original");
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
