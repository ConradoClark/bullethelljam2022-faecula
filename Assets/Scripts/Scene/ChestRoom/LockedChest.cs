using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class LockedChest : MonoBehaviour
{
    public SpriteRenderer FlashSprite;
    public SpriteRenderer ShadowSprite;
    public TimerScriptable TimerRef;

    public IEnumerable<IEnumerable<Action>> Knock()
    {
        yield return Flash().AsCoroutine()
            .Combine(Jump().AsCoroutine())
            .Combine(Scale().AsCoroutine())
            .Combine(ScaleShadow().AsCoroutine())
            .Combine(RandomRotate().AsCoroutine());
    }

    private IEnumerable<IEnumerable<Action>> Flash()
    {
        yield return FlashSprite.GetAccessor()
            .Color.A
            .SetTarget(0.5f)
            .Over(0.4f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return FlashSprite.GetAccessor()
            .Color.A
            .SetTarget(0f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> Jump()
    {
        yield return transform.GetAccessor().Position
            .Y.Increase(0.65f)
            .Over(0.35f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return transform.GetAccessor().Position
            .Y.Decrease(0.65f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> Scale()
    {
        yield return transform.GetAccessor().UniformScale()
            .Increase(0.25f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return transform.GetAccessor().UniformScale()
            .Decrease(0.25f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> ScaleShadow()
    {
        yield return ShadowSprite.transform.GetAccessor().LocalScale.X
            .Increase(0.25f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return ShadowSprite.transform.GetAccessor().LocalScale.X
            .Decrease(0.15f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> RandomRotate()
    {
        var direction = Random.value >= 0.5f ? 1f : -1f;
        var intensity = (0.1f + Random.value * 0.05f) * 0.1f;
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.3f, (ms) =>
        {
            transform.Rotate(0, 0, intensity * (float)ms * direction);
        });

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.3f, (ms) =>
        {
            transform.Rotate(0, 0, intensity * (float)ms * -direction);
        });

        transform.rotation = Quaternion.identity;
    }
}
