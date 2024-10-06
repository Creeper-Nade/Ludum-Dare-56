using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1matrix : MonoBehaviour
{
    [SerializeField] Global_Data data;
    void Awake()
    {
        data=GameObject.FindWithTag("data").GetComponent<Global_Data>();
        data.Team2.Add(this.gameObject);
    }
}
