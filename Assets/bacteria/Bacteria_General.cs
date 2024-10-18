using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class Bacteria_General : MonoBehaviour
{
    public BacteriaSTATS stat;
    public RTS_controll rts;

    private GameObject AttackArea;
    public GameObject Death_particle;

    [SerializeField] Global_Data data;
    [SerializeField] Health_Bar healthBar;
    public AudioSource sound_source;
    public AudioClip shield_activated;
    public AudioClip shield_break;
    public AudioClip shield_hitted;


    //[SerializeField] ParticleSystem particle;

    [SerializeField] public NavMeshAgent agent;
    [SerializeField] UnitRTS unitRTS;
    public Animator animator;
    public Animator shield_icon_animation;
    public Animator shield_damage_animation;
    private int is_opened=Animator.StringToHash("is_opened");
    private int damaged=Animator.StringToHash("damaged");
    [SerializeField]private SpriteRenderer sprite;
    public int Team;
    private bool death_coroutine_ran=false;
    public bool designated_destination=false;

        [Header("STATS")]
    [SerializeField] public float Health;
    [SerializeField] public float ATK;
    [SerializeField] public float ATK_speed;
    public bool is_attack_ready=true;
    public bool is_attack_Coroutine_ready=true;
    [SerializeField] public float ATK_CD=15f;
    [SerializeField] public float speed;
    public int damage_intake;
    public int shield;
    public bool intake_recovery_activated=false;
    Color defaultColor;

        [Header("list")]
    [SerializeField] List<GameObject> Collided_obj;
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
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();

        //particle.gameObject.SetActive(false);

        AttackArea=this.gameObject.transform.GetChild(0).gameObject;
        AttackArea.SetActive(false);

        rts=FindObjectOfType<RTS_controll>();
        if(this.gameObject.GetComponent<UnitRTS>()!=null)
        {
            unitRTS=this.gameObject.GetComponent<UnitRTS>();
        }

        agent=this.gameObject.GetComponent<NavMeshAgent>();
        agent.speed=speed;

        if(this.gameObject.GetComponent<Team1bacteria>()!=null)Team=1;
        if(this.gameObject.GetComponent<team2bacteria>()!=null)Team=2;
        if(this.gameObject.GetComponent<Team3bacteria>()!=null)Team=3;

        if(!data.Team1.Contains(this.gameObject)&&!data.Team2.Contains(this.gameObject)&&!data.Team3.Contains(this.gameObject))
        {
            switch(Team)
            {
                case 1:
                data.Team1.Add(this.gameObject);
                break;
                case 2:
                data.Team2.Add(this.gameObject);
                break;
                case 3:
                data.Team3.Add(this.gameObject);
                break;
            }
        }
        defaultColor=sprite.color;
        agent.updateRotation=false;
        agent.updateUpAxis=false;
    }

    private void Start() {
        //agent.updateRotation=false;
        //agent.updateUpAxis=false;
    }
    private void Update() {
        if(is_attack_ready==false)
        {
            ATK_CD-=Time.deltaTime;
            agent.angularSpeed=999;
            agent.acceleration=999;
            if(ATK_CD<=0)
            {
                is_attack_ready=true;
                is_attack_Coroutine_ready=true;
                agent.angularSpeed=120;
                agent.acceleration=10;
                ATK_CD=15f/ATK_speed;
            }
        }
        if(Health<=0)
        {
            //Debug.Log("I am dead");
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
            if(rts.selectedUnitRTS.Contains(unitRTS))
            {
                rts.selectedUnitRTS.Remove(unitRTS);
            }
            if(death_coroutine_ran==false)
            {
                StartCoroutine(Die());
            }           
        }
        //BacteriaB animation
        if(shield>0&&shield_icon_animation!=null)
        {
            if(shield_icon_animation.GetBool(is_opened)==false)
            {
                sound_source.PlayOneShot(shield_activated);
            }
            shield_icon_animation.SetBool(is_opened,true);
        }
        else if(shield<=0&&shield_icon_animation!=null)
        {
            if(shield_icon_animation.GetBool(is_opened)==true)
            {
                sound_source.PlayOneShot(shield_break);
            }
            shield_icon_animation.SetBool(is_opened,false);
        }
        //damaging
        if(Collided_obj.Any())
        {
            AttackArea.SetActive(is_attack_ready);
            if(is_attack_ready==true&&is_attack_Coroutine_ready==true)
            StartCoroutine(Attack_time());
        }
        //rts thing

        // Check if we've reached the destination
        if (!agent.pathPending&&this.gameObject.GetComponent<UnitRTS>()!=null)
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!((data.Team1.Contains(this.gameObject)&&data.Team1.Contains(other.gameObject))||(data.Team2.Contains(this.gameObject)&&data.Team2.Contains(other.gameObject))||(data.Team3.Contains(this.gameObject)&&data.Team3.Contains(other.gameObject))))
        {
            if(!other.gameObject.CompareTag("resource"))
            {
                Collided_obj.Add(other.gameObject);
                AttackArea.SetActive(is_attack_ready);            
            }
            
        }
        
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(Collided_obj.Contains(other.gameObject))
        Collided_obj.Remove(other.gameObject);
    }

    IEnumerator Attack_time()
    {
        is_attack_Coroutine_ready=false;
        yield return new WaitForSeconds(0.25f);
        is_attack_ready=false;
    }
    public void Damage(int damage)
    {
        if(shield>0)
        {
            shield-=1;
            shield_damage_animation.SetTrigger(damaged);
            sound_source.PlayOneShot(shield_hitted);
        }
        else{
            Health-=damage;
            
            //Debug.Log(damage);
            healthBar.Change(-damage);

            sound_source.PlayOneShot(sound_source.clip);

            damage_intake+=damage;
            //bacteria D recovery effect
            if(damage_intake>=4)
            {
                for(int i=0;i<damage_intake/4;i++)
                {
                    if(intake_recovery_activated==true)
                    {
                        Health++;
                        healthBar.Change(1);
                    }
                }
            }

            StartCoroutine(damaged_blink());
        }
        
    }

    IEnumerator damaged_blink()
    {
        sprite.color=Color.red;

        yield return new WaitForSeconds(0.05f);
        sprite.color=defaultColor;
    }

    IEnumerator Die()
    {
        death_coroutine_ran=true;
        //particle.gameObject.SetActive(true);
        Instantiate(Death_particle,transform.position,Quaternion.identity);
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
