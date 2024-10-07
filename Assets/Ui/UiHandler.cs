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
    private player_matrix _player_matrix;
    
    private void Awake()
    {
        _player_matrix=FindObjectOfType<player_matrix>();
    }

    void Update()
    {
        X_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_x);
        Y_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_y);
        Z_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_z);
    }
}
