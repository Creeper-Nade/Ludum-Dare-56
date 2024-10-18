using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance {get;private set;}
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    public RectTransform canvasRectTransform;
    private System.Func<string> getTooltipTextFunc;
    private void Awake() {
        Instance=this;
        backgroundRectTransform=transform.Find("background").GetComponent<RectTransform>();
        textMeshPro=transform.Find("info").GetComponent<TextMeshProUGUI>();
        rectTransform=transform.GetComponent<RectTransform>();
        HideTooltip();
    }
    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize=textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize=new Vector2(16,16);
        backgroundRectTransform.sizeDelta=textSize+paddingSize;
    }
    private void Update() {
        SetText(getTooltipTextFunc());
        Vector2 anchoredPosition=Input.mousePosition/canvasRectTransform.localScale.x;
        if(anchoredPosition.x+backgroundRectTransform.rect.width>canvasRectTransform.rect.width)
        {
            //tip on right
            anchoredPosition.x=canvasRectTransform.rect.width-backgroundRectTransform.rect.width;
        }
        if(anchoredPosition.y+backgroundRectTransform.rect.height>canvasRectTransform.rect.height)
        {
            //tip on top
            anchoredPosition.y=canvasRectTransform.rect.height-backgroundRectTransform.rect.height;
        }
        rectTransform.anchoredPosition=anchoredPosition;
    }
    private void ShowTooltip(string tooltipText)
    {
        gameObject.SetActive(true);
        SetText(tooltipText);
    }
    private void ShowTooltip(System.Func<string> getTooltipTextFunc)
    {
        this.getTooltipTextFunc=getTooltipTextFunc;
        gameObject.SetActive(true);
        SetText(getTooltipTextFunc());
    }
    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    public static void ShowTooltip_Static(string tooltipText)
    {
        Instance.ShowTooltip(tooltipText);
    }
    public static void ShowTooltip_Static(System.Func<string> getTooltipTextFunc)
    {
        Instance.ShowTooltip(getTooltipTextFunc);
    }
    public static void HideTooltip_Static()
    {
        Debug.Log("attempted to hide");
        Instance.HideTooltip();
    }
}
