using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class DurationBullet : BaseBullet
{
    public float Duration;

    public override void OnActivation()
    {
        base.OnActivation();
        transform.rotation = Quaternion.identity;
        BasicMachineryObject.Machinery.AddBasicMachine(CheckDuration());
    }

    private IEnumerable<IEnumerable<Action>> CheckDuration()
    {
        yield return TimeYields.WaitSeconds(TimerRef.Timer, Duration);
        IsEffectOver = true;
    }
}