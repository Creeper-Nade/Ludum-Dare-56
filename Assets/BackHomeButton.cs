using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackHomeButton : MonoBehaviour
{
    public Button button;
    public Color HoveredColor;
    private Color Original;
    private ColorBlock cb;
    void Start()
    {
        cb=button.colors;
        Original=cb.normalColor;
    }
    public void ChangeWhenHover()
    {
        cb.selectedColor=HoveredColor;
        button.colors=cb;
        this.gameObject.GetComponent<RectTransform>().localScale=new Vector2(1.2f,1.2f);
    }
    public void ChangeOnExit()
    {
        cb.selectedColor=Original;
        button.colors=cb;
        this.gameObject.GetComponent<RectTransform>().localScale=new Vector2(1f,1f);
    }
}
