using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_matrix : MonoBehaviour
{
    public card_manager cmanager;
    private GameObject instantiating_prefab; 

    public Global_Data data;  
    public void InstantiatePrefab()
    {
        instantiating_prefab=cmanager.selected_element.GetComponent<CardSettings>().card_stat.prefab;
        data.Team1.Add(instantiating_prefab);
        var Cloned_bacteria=Instantiate(instantiating_prefab,transform.position,Quaternion.Euler(Vector3.forward*Random.Range(0.0f, 360.0f)));
    }
}
