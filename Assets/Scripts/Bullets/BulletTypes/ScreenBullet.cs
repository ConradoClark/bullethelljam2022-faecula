using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class ScreenBullet : BaseBullet
{
    public Vector2 Direction;
    public float Speed;
    public BoundsValue Bounds;

    private float? _originalSpeed;

    public override void OnActivation()
    {
        base.OnActivation();
        _originalSpeed ??= Speed;
        Speed = _originalSpeed ?? Speed;
        transform.rotation = Quaternion.identity;
        BasicMachineryObject.Machinery.AddBasicMachine(CheckOutOfBounds());
    }

    private IEnumerable<IEnumerable<Action>> CheckOutOfBounds()
    {
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        while (!IsEffectOver && isActiveAndEnabled)
        {
            if (!Bounds.Value.Contains(transform.position))
            {
                IsEffectOver = true;
            }

            // no need to check every frame
            for (var i = 0; i < 10; i++)
            {
                if (IsEffectOver || !isActiveAndEnabled)
                {
                    IsEffectOver = true;
                    yield break;
                }
                yield return TimeYields.WaitOneFrameX;
            }
        }
        IsEffectOver = true;
    }
}