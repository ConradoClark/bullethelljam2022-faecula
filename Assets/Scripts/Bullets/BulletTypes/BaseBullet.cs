using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class BaseBullet : EffectPoolable
{
    public TimerScriptable TimerRef;
    public Collider2D Collider;
    public ActivableEffect[] Effects;

    public override void OnActivation()
    {
        foreach (var effect in Effects)
        {
            effect.Activate();
        }
    }

    public override bool IsEffectOver { get; protected set; }
}