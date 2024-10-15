using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public static float tickTime=0.05f;
    public static float tickTime02=0.2f;
    private float _tickerTimer;
    private float _tickerTimer02;
    public delegate void TickAction();
    public static event TickAction OnTickAction;
    public delegate void TickAction02();
    public static event TickAction OnTickAction02;
    private void Update()
    {
        _tickerTimer02+=Time.deltaTime;
        _tickerTimer+=Time.deltaTime;
        if(_tickerTimer>=tickTime)
        {
            _tickerTimer=0;
            TickEvent();
        }
        if(_tickerTimer02>=tickTime02)
        {
            _tickerTimer02=0;
            TickEvent02();
        }
    }
    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }
    private void TickEvent02()
    {
        OnTickAction02?.Invoke();
    }
}
