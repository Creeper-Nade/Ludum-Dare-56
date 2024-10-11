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
    private bool death_coroutine_ran=false;

    public int Team;
    [Header("other references")]
    public Animator ui_animator;
    private void Awake() {
        production_x=0;
        production_y=0;
        production_z=0;
        Health=50;

        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        if(this.gameObject.GetComponent<player_matrix>())Team=1;
        if(this.gameObject.GetComponent<enemy1matrix>())Team=2;
        if(this.gameObject.GetComponent<enemy2matrix>())Team=3;
        defaultColor=sprite.color;
    }

    private void Update()
    {
        //Debug.Log(gameObject.name+production_x);
        //Debug.Log(gameObject.name + production_y);
        //Debug.Log(gameObject.name + production_z);

        if(Health<=0)
        {
            Debug.Log("I am dead");
                data.Team1.Remove(this.gameObject);
                data.Team2.Remove(this.gameObject);
                data.Team3.Remove(this.gameObject);
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
            Health-=damage;
            Debug.Log(damage);
            if(this.gameObject.GetComponent<player_matrix>()!=null)
            {
                Cinemachine_shake.Instance.ShakeCamera(8f,.2f);
                ui_animator.SetTrigger("is_damaged");
            }
            else{
            Cinemachine_shake.Instance.ShakeCamera(4f,.1f);}
            StartCoroutine(damaged_blink());
            Debug.Log("ouch");
        
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
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
        death_coroutine_ran=false;
    }
    private void OnDestroy() {
        //manager.RefreshNavMesh();
        
    }
}
