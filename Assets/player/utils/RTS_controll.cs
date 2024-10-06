using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class RTS_controll : MonoBehaviour
{
    private Vector3 startPos;
    private List<UnitRTS> selectedUnitRTS;
    
    private void Awake() {
        selectedUnitRTS=new List<UnitRTS>();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos=UtilsClass.GetMouseWorldPosition();
        }

        if(Input.GetMouseButtonUp(0))
        {
            Collider2D[] collider2DArray=Physics2D.OverlapAreaAll(startPos,UtilsClass.GetMouseWorldPosition());
            //deselect
            foreach(UnitRTS unitRTS in selectedUnitRTS)
            {
                unitRTS.SetSelectedVisible(false);
            }
            selectedUnitRTS.Clear();
            //select
            foreach(Collider2D collider2D in collider2DArray)
            {
                UnitRTS unitRTS=collider2D.GetComponent<UnitRTS>();
                if(unitRTS!=null)
                {
                    unitRTS.SetSelectedVisible(true);
                    selectedUnitRTS.Add(unitRTS);
                }
            }
            Debug.Log(selectedUnitRTS.Count);
        }
    }
}
