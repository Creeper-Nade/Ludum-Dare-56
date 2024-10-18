using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New card",menuName ="custom/card")]
public class CardScriptable : ScriptableObject
{
    public float X_consume;
    public float Y_consume;
    public float Z_consume;
    public GameObject prefab;
    public string Role;
    public string CombinationEffect1;
    public string CombinationEffect2;
}
