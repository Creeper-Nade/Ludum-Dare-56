using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class card_manager : MonoBehaviour
{
    [Header("visual")]

    private Animator animator;
    public GameObject canvas;
    public GameObject selected_element;
    public Image img;
    private UnityEngine.Color originalcolor;

    [Header("raycast")]
    GraphicRaycaster ui_raycaster;
    PointerEventData hovered_data;
    List<RaycastResult> casted_results;
    [SerializeField] List<GameObject> children;


    [Header("misc")]

    private float RectmaximumY=-390;

    private player_matrix _player_matrix;
    public AudioSource CameraSource;
    public AudioClip swoosh;
    public AudioClip UnableSummon;
    public AudioClip EnableSummon;
    public AudioClip Hover;
    private bool BlinkOccuring=false;

    void Awake()
    {
        animator= gameObject.GetComponent<Animator>();
        ui_raycaster=canvas.GetComponent<GraphicRaycaster>();
        CameraSource=GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        hovered_data=new PointerEventData(EventSystem.current);
        casted_results=new List<RaycastResult>();

        //addressing player matrix to find it
        _player_matrix=FindObjectOfType<player_matrix>();
        //Find children in this
        foreach(Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        originalcolor=img.color;
    }
    void Update()
    {
        //detect hover
        if(animator.GetBool("visible")==true)
        {
            RaycastUI(); 
             
        }
        foreach(GameObject card in children)
        {
            if(CalculateCostConstanly(card))
            {
                if(card.GetComponent<Image>().color!=originalcolor&&BlinkOccuring==false)
                {
                    card.GetComponent<Image>().color=originalcolor;
                }
            }
            else
            {
                if(card.GetComponent<Image>().color!=UnityEngine.Color.gray&&BlinkOccuring==false)
                {

                    card.GetComponent<Image>().color=UnityEngine.Color.gray;
                }
                
            }
        }

    }
    void RaycastUI()
    {
        hovered_data.position = Input.mousePosition;
        casted_results.Clear();
 
        ui_raycaster.Raycast(hovered_data, casted_results);
 
        foreach(RaycastResult result in casted_results)
        {
            //confusing code of hovering effect
            GameObject ui_element = result.gameObject;
            Vector2 anchoredPos=ui_element.GetComponent<RectTransform>().anchoredPosition;
            if(children.Contains(ui_element)&&ui_element.GetComponent<RectTransform>().anchoredPosition.y<=RectmaximumY)
            {
                if(ui_element.transform.GetComponent<RectTransform>().anchoredPosition.y==-403)
                CameraSource.PlayOneShot(Hover);
                anchoredPos.y+=2;                
                ui_element.GetComponent<RectTransform>().anchoredPosition=anchoredPos;
                
            }
            
            if(Input.GetMouseButtonDown(0))
            {
                //more confusing codes of blinking effect when card is clicked
                selected_element=ui_element;
                if(CalculateCost()==true)
                {
                    StartCoroutine(SuccessfulFlash());
                }
                else
                {
                    StartCoroutine(FailingFlash());
                }
                
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

    public IEnumerator SuccessfulFlash()
    {
        BlinkOccuring=true;
        CameraSource.PlayOneShot(EnableSummon);
        selected_element.GetComponent<Image>().color= UnityEngine.Color.cyan;
        yield return new WaitForSeconds(0.1f);
        selected_element.GetComponent<Image>().color = originalcolor;
        BlinkOccuring=false;
        StopCoroutine(SuccessfulFlash());
    }

        public IEnumerator FailingFlash()
    {
        BlinkOccuring=true;
        CameraSource.PlayOneShot(UnableSummon);
        selected_element.GetComponent<Image>().color= UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        selected_element.GetComponent<Image>().color = UnityEngine.Color.gray;
        BlinkOccuring=false;
        StopCoroutine(FailingFlash());
    }
    public void matrix_clicked()
    {
        if(animator.GetBool("visible")==false)
        {
            CameraSource.PlayOneShot(swoosh);
        }        
        animator.SetBool("visible",true);
    }
    public void matrix_quit()
    {
        if(animator.GetBool("visible"))
        {
            CameraSource.PlayOneShot(swoosh);
        }   
        animator.SetBool("visible",false);
        foreach(GameObject child in children)
            {
                child.GetComponent<RectTransform>().anchoredPosition= new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x,-403);
            }
    }

    private bool CalculateCost()
    {
        if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_x-selected_element.GetComponent<CardSettings>().card_stat.X_consume>=0)
        {
            if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_y-selected_element.GetComponent<CardSettings>().card_stat.Y_consume>=0)
            {
                if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_z-selected_element.GetComponent<CardSettings>().card_stat.Z_consume>=0)
                {
                    _player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_x-=selected_element.GetComponent<CardSettings>().card_stat.X_consume;
                    _player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_y-=selected_element.GetComponent<CardSettings>().card_stat.Y_consume;
                    _player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_z-=selected_element.GetComponent<CardSettings>().card_stat.Z_consume;
                    Debug.Log("summoning success");
                    _player_matrix.InstantiatePrefab();
                    return true;
                    //instantiate corresponding prefab
                }
                else return false;
            }
            else return false;
        }
        return false;
    }
    private bool CalculateCostConstanly(GameObject TargetCard)
    {
        if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_x-TargetCard.GetComponent<CardSettings>().card_stat.X_consume>=0)
        {
            if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_y-TargetCard.GetComponent<CardSettings>().card_stat.Y_consume>=0)
            {
                if(_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_z-TargetCard.GetComponent<CardSettings>().card_stat.Z_consume>=0)
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        return false;
    }
}
