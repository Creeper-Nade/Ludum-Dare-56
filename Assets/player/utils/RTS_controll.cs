using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class RTS_controll : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    public Vector3 startPos;
    [SerializeField]public List<UnitRTS> selectedUnitRTS;
    
    private void Awake() {
        selectedUnitRTS=new List<UnitRTS>();
        selectionAreaTransform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionAreaTransform.gameObject.SetActive(true);
            startPos=UtilsClass.GetMouseWorldPosition();
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 currentMousePostition=UtilsClass.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startPos.x,currentMousePostition.x),
                Mathf.Min(startPos.y,currentMousePostition.y)
            );
            Vector3 upperRight=new Vector3(
                Mathf.Max(startPos.x,currentMousePostition.x),
                Mathf.Max(startPos.y,currentMousePostition.y)
            );
            selectionAreaTransform.position=lowerLeft;
            selectionAreaTransform.localScale=upperRight-lowerLeft;
        }

        if(Input.GetMouseButtonUp(0))
        {
            selectionAreaTransform.gameObject.SetActive(false);
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
