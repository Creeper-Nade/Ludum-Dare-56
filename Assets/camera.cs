using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class camera : MonoBehaviour
{
    [Header("Camera settings")]
    private Camera _mainCamera;
    public CinemachineVirtualCamera cinemachine;
   private bool dragPanMoveActive;
   private Vector2 lastMousePosition;
   private Vector3 inputDir=new Vector3(0,0,0);
   [SerializeField ]private float maximumFOV=20;
   [SerializeField ]private float minimumFOV=5;
   private float targetFOV=10;
    [Header("gameobj and stuffs")]
   private card_manager cards;
   private player_matrix matrix;

   private void Awake()
   {
    _mainCamera=Camera.main;
    matrix=FindObjectOfType<player_matrix>();
    cards=FindObjectOfType<card_manager>();
   }
    void Update()
    {
        
        PanCamera();
        HandleCameraZoom();
        if(Input.GetMouseButtonDown(0)&&EventSystem.current.IsPointerOverGameObject()==false)
        {
            Left_clicked();
        }
        if(transform.position!=matrix.gameObject.transform.position)
        {
            cards.matrix_quit();
        }
    }
    void Left_clicked()
    {
        GameObject hittedObject;
        var rayHit=Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Input.mousePosition));
        
        if(!rayHit.collider)return;
        Debug.Log(rayHit.collider.gameObject.name);
        hittedObject=rayHit.collider.gameObject;
        transform.position=hittedObject.transform.position;
        targetFOV=8;

        if(hittedObject==matrix.gameObject)
        {
            cards.matrix_clicked();
        }
    }
    void PanCamera()
    {
        if(Input.GetMouseButtonDown(1))
        {
            dragPanMoveActive=true;
            lastMousePosition=Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(1))
        {
            dragPanMoveActive=false;
        }

        if(dragPanMoveActive)
        {
            Vector2 mouseMovementDelta=(Vector2)Input.mousePosition-lastMousePosition;
            float dragPanSpeed=2f;
            inputDir.x=mouseMovementDelta.x*dragPanSpeed;
            inputDir.y=mouseMovementDelta.y*dragPanSpeed;
            lastMousePosition=Input.mousePosition;

            Vector3 moveDir=transform.up*inputDir.y+transform.right*inputDir.x;
        float movespeed=10f;
        transform.position+= -moveDir*movespeed*Time.deltaTime;
        }

        
    }

    private void HandleCameraZoom()
    {
        if(Input.mouseScrollDelta.y>0)
        {
            targetFOV-=2;
        }
        if(Input.mouseScrollDelta.y<0)
        {
            targetFOV+=2;
        }
        targetFOV=Mathf.Clamp(targetFOV,minimumFOV,maximumFOV);
        float zoomSpeed=10f;
        cinemachine.m_Lens.OrthographicSize=Mathf.Lerp(cinemachine.m_Lens.OrthographicSize,targetFOV,Time.deltaTime*zoomSpeed);
    }
}
