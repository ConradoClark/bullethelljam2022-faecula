using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class FadeWave : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer SpriteRenderer;

    public float MinAlpha;
    public float MaxAlpha;
    public float Frequency;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(Wave());
    }

    IEnumerable<IEnumerable<Action>> Wave()
    {
        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(MaxAlpha)
            .Over(Frequency * 2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();

        while (isActiveAndEnabled)
        {
            yield return SpriteRenderer.GetAccessor()
                .Color.A
                .SetTarget(MinAlpha)
                .Over(Frequency)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(TimerRef.Timer)
                .Build();

            yield return SpriteRenderer.GetAccessor()
                .Color.A
                .SetTarget(MaxAlpha)
                .Over(Frequency)
                .Easing(EasingYields.EasingFunction.BounceEaseIn)
                .UsingTimer(TimerRef.Timer)
                .Build();
        }

        SpriteRenderer.color = new Color(SpriteRenderer.color.r,
            SpriteRenderer.color.g,
            SpriteRenderer.color.b,
            MaxAlpha);
    }
}
