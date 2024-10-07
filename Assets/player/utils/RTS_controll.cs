using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AI;
public class RTS_controll : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    public Vector3 startPos;
    [SerializeField]public List<UnitRTS> selectedUnitRTS;
    public Texture2D original_cursor;
    public Texture2D designating_cursor;
    
    private void Awake() {
        selectedUnitRTS=new List<UnitRTS>();
        selectionAreaTransform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!selectedUnitRTS.Any())
            selectionAreaTransform.gameObject.SetActive(true);
            startPos=UtilsClass.GetMouseWorldPosition();

            List<Vector3> targetPositionList=GetPositionListAround(startPos,new float[]{1f,2f,3f},new int[] {5,10,20});//get various destination

            int targetPositionListIndex=0;

            foreach(UnitRTS unitRTS in selectedUnitRTS)
            {
                unitRTS.gameObject.GetComponent<Bacteria_General>().FaceDir();
                //set destination for each
                unitRTS.gameObject.GetComponent<Bacteria_General>().designated_destination=true;
                unitRTS.gameObject.GetComponent<NavMeshAgent>().SetDestination(targetPositionList[targetPositionListIndex]);
                targetPositionListIndex=(targetPositionListIndex+1)%targetPositionList.Count;
            }
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

        if(selectedUnitRTS.Any())
        {
            Vector2 cursorHotspot=new Vector2(designating_cursor.width/2,designating_cursor.height/2);
            Cursor.SetCursor(designating_cursor,cursorHotspot,CursorMode.Auto);
        }
        else{
            Vector2 orignalCursor_hotspot=new Vector2(512,462);
            Cursor.SetCursor(original_cursor,orignalCursor_hotspot,CursorMode.Auto);
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPos,float[] ringDistanceArray,int[] ringPostionCountArray)
    {
        List<Vector3> positionList=new List<Vector3>();
        positionList.Add(startPos);
        for(int i=0;i<ringDistanceArray.Length;i++)
        {
            positionList.AddRange(GetPositionListAround(startPos,ringDistanceArray[i],ringPostionCountArray[i]));
        }
        return positionList;
    }
    private List<Vector3> GetPositionListAround(Vector3 startPos,float distance,int positionCount)
    {
        List<Vector3> positionList=new List<Vector3>();
        for(int i=0;i<positionCount;i++)
        {
            float angle=i*(360f/positionCount);
            Vector3 dir=ApplyRotationToVector(new Vector3(1,0),angle);
            Vector3 position=startPos+dir*distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec,float angle)
    {
        return Quaternion.Euler(0,0,angle)*vec;
    }
}
