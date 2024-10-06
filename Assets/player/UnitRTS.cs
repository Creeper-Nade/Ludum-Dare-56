using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRTS : MonoBehaviour
{
    [SerializeField]private GameObject selectedGameObject;
    private void Awake()
    {
        selectedGameObject=transform.Find("selected").gameObject;
        Debug.Log(selectedGameObject.name);
        SetSelectedVisible(false);
    }
    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }
}
