using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class StraightMovingBullet : ScreenBullet
{
    private void OnEnable()
    {
        BasicMachineryObject.Machinery.AddBasicMachine(Move());
    }

    IEnumerable<IEnumerable<Action>> Move()
    {
        while (isActiveAndEnabled && !IsEffectOver)
        {
            transform.Translate((float)TimerRef.Timer.Multiplier * Speed * Direction);
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
