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
}
