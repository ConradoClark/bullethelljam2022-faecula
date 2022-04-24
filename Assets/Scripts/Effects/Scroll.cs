using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public float HorizontalScrollSpeed = 0.25f;
    public float VerticalScrollSpeed = 0.25f;
    public bool Scrolling;

    public SpriteRenderer SpriteRenderer;

    public TimerScriptable TimerRef;
    private double _multiplier;

    public void Update()
    {
        if (!Scrolling) return;
        var verticalOffset = (float)TimerRef.Timer.UpdatedTimeInMilliseconds * HorizontalScrollSpeed;
        var horizontalOffset = (float)TimerRef.Timer.UpdatedTimeInMilliseconds * VerticalScrollSpeed;
        if (!(Math.Abs(TimerRef.Timer.Multiplier - _multiplier) > 0.001f)) return;
        
        SpriteRenderer.material.SetVector("_ScrollSpeed", new Vector4(horizontalOffset, verticalOffset));
        _multiplier = TimerRef.Timer.Multiplier;
    }
}