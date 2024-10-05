using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_matrix : MonoBehaviour
{
    public card_manager cmanager;
    private GameObject instantiating_prefab;   
    public void InstantiatePrefab()
    {
        instantiating_prefab=cmanager.selected_element.GetComponent<CardSettings>().card_stat.prefab;
        var Cloned_bacteria=Instantiate(instantiating_prefab,transform.position,Quaternion.Euler(Vector3.forward*Random.Range(0.0f, 360.0f)));
        //float randomDirectionModifier=Random.Range(-180f,180f);
        //Cloned_bacteria.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f+randomDirectionModifier,1f));
    }
}
