using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class HeartCounter : MonoBehaviour
{
    public PrefabPool HeartsPool;
    public PrefabPool HeartEffectPool;
    public FaeStats Stats;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public float Offset;

    private Heart[] _hearts = new Heart[5];

    private void OnEnable()
    {
        this.ObserveEvent<FaeStats.FaeEvents, FaeStats.FaeHitPointsEventHandler>(FaeStats.FaeEvents.OnTakeDamage, OnEvent);
        MachineryRef.Machinery.AddBasicMachine(ShowHearts());
    }

    private void OnEvent(FaeStats.FaeHitPointsEventHandler obj)
    {
        if (obj.CurrentHitPoints < 0) return; 
        _hearts[obj.CurrentHitPoints].SetEmpty();
    }

    private IEnumerable<IEnumerable<Action>> ShowHearts()
    {
        for (var i = 0; i < Stats.MaxHitPoints; i++)
        {
            yield return TimeYields.WaitMilliseconds(TimerRef.Timer, 500);
            if (!HeartsPool.TryGetFromPool(out var obj) || !(obj is Heart heart)) continue;

            var position = transform.position + new Vector3(i * Offset, 0, 0);
            if (HeartEffectPool.TryGetFromPool(out var ef) && ef is EffectPoolable heartEffect)
            {
                heartEffect.transform.position = position;
            }

            heart.transform.position = position;
            _hearts[i] = heart;
        }
    }
}
