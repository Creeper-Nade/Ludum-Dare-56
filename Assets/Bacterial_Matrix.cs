using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using Unity.Collections;
using UnityEngine;

public class Bacterial_Matrix : MonoBehaviour
{
    public float production_x;
    public float production_y;
    public float production_z;
    public float Health;
    Color defaultColor;
    public SpriteRenderer sprite;
    public Global_Data data;
    public matrix_manager manager;
    public GameObject Dying_particle;
    public GameObject Dead_Burst;
    public camera CameraFocus;
    public AudioSource audioSource;
    public AudioSource cameraSource;
    public AudioClip Player_damaged;
    private bool death_coroutine_ran=false;

    public int Team;
    [Header("other references")]
    public Animator ui_animator;
    public Animator Matrix_animator;
    private void Awake() {
        production_x=0;
        production_y=0;
        production_z=0;
        Health=50;

        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        CameraFocus=GameObject.FindFirstObjectByType<camera>();
        Matrix_animator=this.gameObject.GetComponentInChildren<Animator>();
        audioSource=GetComponent<AudioSource>();
        if(this.gameObject.GetComponent<player_matrix>())Team=1;
        if(this.gameObject.GetComponent<enemy1matrix>())Team=2;
        if(this.gameObject.GetComponent<enemy2matrix>())Team=3;
        defaultColor=sprite.color;

        Dying_particle.SetActive(false);
    }

    private void Update()
    {
        //Debug.Log(gameObject.name+production_x);
        //Debug.Log(gameObject.name + production_y);
        //Debug.Log(gameObject.name + production_z);

        if(Health<=0)
        {
            Debug.Log("I am dead");
                
                Matrix_animator.SetTrigger("is_producing");
            //ui_animator
            if(death_coroutine_ran==false)
            {
                StartCoroutine(Die());
            }           
        }
    }
    
    public void AddResources(int carryingX, int carryingY, int carryingZ)
    {
        production_x += carryingX;
        production_y += carryingY;
        production_z += carryingZ;
        
    }

    public void Damage(int damage)
    {
        if(Health>0)
        {
            Health-=damage;
            audioSource.PlayOneShot(audioSource.clip);
            Debug.Log(damage);
            if(this.gameObject.GetComponent<player_matrix>()!=null)
            {
                Cinemachine_shake.Instance.ShakeCamera(8f,.2f);
                ui_animator.SetTrigger("is_damaged");
                cameraSource.PlayOneShot(Player_damaged);
            }
            else{
            Cinemachine_shake.Instance.ShakeCamera(4f,.1f);}
            StartCoroutine(damaged_blink());
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
        Dying_particle.SetActive(true);
        CameraFocus.transform.position=this.transform.position;
        Cinemachine_shake.Instance.ShakeCamera(8f,3);        
        yield return new WaitForSeconds(3);
        Instantiate(Dead_Burst,transform.position,Quaternion.identity);
        Cinemachine_shake.Instance.ShakeCamera(8f,.1f);   
        data.Team1.Remove(this.gameObject);
                data.Team2.Remove(this.gameObject);
                data.Team3.Remove(this.gameObject);
        Destroy(gameObject);
        death_coroutine_ran=false;
    }
    private void OnDestroy() {
        //manager.RefreshNavMesh();
        
    }
}
