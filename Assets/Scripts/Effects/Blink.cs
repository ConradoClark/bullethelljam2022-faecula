using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public float BlinkDuration;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(BlinkEffect());
    }

    private IEnumerable<IEnumerable<Action>> BlinkEffect()
    {
        while (isActiveAndEnabled)
        {
            yield return TimeYields.WaitSeconds(TimerRef.Timer, BlinkDuration);
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
        }

        SpriteRenderer.enabled = true;
    }
}
