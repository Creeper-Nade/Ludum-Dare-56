using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy2matrix : MonoBehaviour
{
    [SerializeField] Global_Data data;
    void Awake()
    {
        data=GameObject.FindWithTag("GBdata").GetComponent<Global_Data>();
        data.Team3.Add(this.gameObject);
    }
}
