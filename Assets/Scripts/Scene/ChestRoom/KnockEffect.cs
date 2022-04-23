using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class KnockEffect : MonoBehaviour
{
    public SpriteRenderer FlashSprite;
    public SpriteRenderer ShadowSprite;
    public TimerScriptable TimerRef;
    public PrefabPool StarEffectPool;

    private PlayerInput _input;
    private Camera _camera;

    public bool Jumps;
    public bool Rotates;


    void OnEnable()
    {
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));
    }

    public IEnumerable<IEnumerable<Action>> Knock()
    {
        if (StarEffectPool.TryGetFromPool(out var obj) && obj is EffectPoolable effect)
        {
            var pos = GetMousePosInWorld();
            effect.transform.position = new Vector3(pos.x, pos.y, 0);
        }

        yield return Flash().AsCoroutine()
            .Combine(Jumps ? Jump().AsCoroutine() : Enumerable.Empty<Action>())
            .Combine(Scale().AsCoroutine())
            .Combine(Jumps ? ScaleShadow().AsCoroutine() : Enumerable.Empty<Action>())
            .Combine(Rotates ? RandomRotate().AsCoroutine() : Enumerable.Empty<Action>());
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
        yield return transform.GetAccessor().LocalScale.Y
            .Increase(0.25f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return transform.GetAccessor().LocalScale.Y
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
            .Decrease(0.25f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> RandomRotate()
    {
        var direction = Random.value >= 0.5f ? 1f : -1f;
        var intensity = (0.05f + Random.value * 0.02f) * 0.1f;
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

    private Vector3 GetMousePosInWorld()
    {
        var mousePosition = _input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = _camera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }
}
