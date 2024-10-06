using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class bar_extension
{
    public static void SetWidth(this RectTransform t,float width)
    {
        t.sizeDelta=new Vector2(width,t.rect.height);
    }
}
