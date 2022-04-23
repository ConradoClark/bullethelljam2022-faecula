using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class Fade : ActivableEffect
{
    public float Delay;
    public float Duration;

    public SpriteRenderer SpriteRenderer;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    private Color? _originalColor;

    void OnEnable()
    {
        _originalColor ??= SpriteRenderer.color;
    }

    public IEnumerable<IEnumerable<Action>> Run()
    {
        IsActive = true;
        SpriteRenderer.color = _originalColor ?? SpriteRenderer.color;
        yield return TimeYields.WaitSeconds(TimerRef.Timer, Delay);

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .Over(Duration)
            .SetTarget(0f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .BreakIf(() => !isActiveAndEnabled)
            .UsingTimer(TimerRef.Timer)
            .Build();
        IsActive = false;
    }

    public override bool Activate()
    {
        if (IsActive) return false;
        MachineryRef.Machinery.AddBasicMachine(Run());
        return true;
    }
}
