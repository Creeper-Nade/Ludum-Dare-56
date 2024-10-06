using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Bacteria",menuName ="custom/Bacteria")]
public class BacteriaSTATS : ScriptableObject
{
    public int attack;
    public float health;
    public int max_inventory_space;

    public int[] inventory;
    public float atk_speed;
    public float speed;
    public Sprite sprite;
    public int Team;
}
