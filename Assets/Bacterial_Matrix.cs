using System.Collections;
using System.Collections.Generic;
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
    private bool death_coroutine_ran=false;
    private void Awake() {
        production_x=0;
        production_y=0;
        production_z=0;
        Health=50;
    }

    private void Update()
    {
        Debug.Log(gameObject.name+production_x);
        Debug.Log(gameObject.name + production_y);
        Debug.Log(gameObject.name + production_z);
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
            StartCoroutine("damaged_blink");
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
}
