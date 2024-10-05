using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaA : MonoBehaviour
{
    [SerializeField] BacteriaSTATS stat;
    [SerializeField] SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        _spriteRenderer.sprite=stat.sprite;
    }
}
