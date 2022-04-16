using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class PressedEffectPoolable : EffectPoolable
{
    public PressableButton Button;

    public override void OnActivation()
    {
        IsEffectOver = false;
        BasicMachineryObject.Machinery.AddBasicMachine(HandleEffect());
    }

    public override bool IsEffectOver { get; protected set; }

    private IEnumerable<IEnumerable<Action>> HandleEffect()
    {
        while (Button == null)
        {
            yield return TimeYields.WaitOneFrameX;
        }
        while (!IsEffectOver)
        {
            while (Button.IsActive) yield return TimeYields.WaitOneFrameX;
            IsEffectOver = true;
        }
    }
}
