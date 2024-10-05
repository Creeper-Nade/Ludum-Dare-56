using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UiHandler : MonoBehaviour
{
    public Global_Data data;
    public TextMeshProUGUI X_display;
    public TextMeshProUGUI Y_display;
    public TextMeshProUGUI Z_display;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        X_display.text=string.Format("X:{0}",data.production_x[0]);
        Y_display.text=string.Format("Y:{0}",data.production_y[0]);
        Z_display.text=string.Format("Z:{0}",data.production_z[0]);
    }
}
