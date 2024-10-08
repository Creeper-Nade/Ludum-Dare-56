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
    private void Awake() {
        production_x=0;
        production_y=0;
        production_z=0;
        Health=50;
    }

    private void Update()
    {
        // Debug.Log(gameObject.name + production_x);
        // Debug.Log(gameObject.name + production_y);
        // Debug.Log(gameObject.name + production_z);
    }
    
    public void AddResources(int carryingX, int carryingY, int carryingZ)
    {
        production_x += carryingX;
        production_y += carryingY;
        production_z += carryingZ;
        
    }
}
