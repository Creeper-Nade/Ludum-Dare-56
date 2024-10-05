using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class card_manager : MonoBehaviour
{
    private Animator animator;
    public GameObject canvas;
    private GameObject blinking_element;
    public Image img;
    private UnityEngine.Color originalcolor;
    GraphicRaycaster ui_raycaster;
    PointerEventData hovered_data;
    List<RaycastResult> casted_results;

    public List<GameObject> children;

    private float RectmaximumY=-390;

    void Awake()
    {
        animator= gameObject.GetComponent<Animator>();
        ui_raycaster=canvas.GetComponent<GraphicRaycaster>();
        hovered_data=new PointerEventData(EventSystem.current);
        casted_results=new List<RaycastResult>();

        originalcolor=img.color;
    }
    void Update()
    {
        //detect hover
        if(animator.GetBool("visible")==true)
        {
            RaycastUI(); 
             
        }

    }
    void RaycastUI()
    {
        hovered_data.position = Input.mousePosition;
        casted_results.Clear();
 
        ui_raycaster.Raycast(hovered_data, casted_results);
 
        foreach(RaycastResult result in casted_results)
        {
            //hovering effect
            GameObject ui_element = result.gameObject;
            Vector2 anchoredPos=ui_element.GetComponent<RectTransform>().anchoredPosition;
            if(children.Contains(ui_element)&&ui_element.GetComponent<RectTransform>().anchoredPosition.y<=RectmaximumY)
            {
                anchoredPos.y+=2;
                ui_element.GetComponent<RectTransform>().anchoredPosition=anchoredPos;
            }
            
            if(Input.GetMouseButtonDown(0))
            {
                blinking_element=ui_element;
                StartCoroutine("CardFlash");
                Debug.Log(ui_element.name);
            }

            foreach(GameObject child in children)
            {
                if(child.gameObject!=ui_element)
                {
                    child.GetComponent<RectTransform>().anchoredPosition= new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x,-403);
                }
            }
        }

        
    }

    public IEnumerator CardFlash()
    {
        blinking_element.GetComponent<Image>().color= UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        blinking_element.GetComponent<Image>().color = originalcolor;
        StopCoroutine("CardFlash");
    }
    public void matrix_clicked()
    {
        Debug.Log("haa");
        animator.SetBool("visible",true);
    }
    public void matrix_quit()
    {
        Debug.Log("quit");
        animator.SetBool("visible",false);
        foreach(GameObject child in children)
            {
                child.GetComponent<RectTransform>().anchoredPosition= new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x,-403);
            }
    }
}
