using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class MagicCounter : MonoBehaviour
{
    public PrefabPool MagicPool;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public FaeStats Stats;

    private List<IPoolableComponent> _magicBars = new List<IPoolableComponent>();

    private void OnEnable()
    {
        _magicBars = new List<IPoolableComponent>();
        this.ObserveEvent<FaeStats.FaeEvents, FaeStats.FaeMagicEventHandler>(FaeStats.FaeEvents.OnGraze, OnEvent);

        FillBars();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<FaeStats.FaeEvents, FaeStats.FaeMagicEventHandler>(FaeStats.FaeEvents.OnGraze, OnEvent);
    }

    private void OnEvent(FaeStats.FaeMagicEventHandler obj)
    {
        FillBars();
    }

    private void FillBars()
    {
        var scaledMagic = Stats.Magic / 2;
        if (scaledMagic == _magicBars.Count) return;
        if (scaledMagic > _magicBars.Count)
        {
            if (MagicPool.TryGetManyFromPool(scaledMagic - _magicBars.Count, out var objects))
            {
                _magicBars.AddRange(objects);
            }
        }
        else if (scaledMagic < _magicBars.Count)
        {
            foreach (var _ in Enumerable.Range(0, _magicBars.Count - scaledMagic))
            {
                var bar = _magicBars.Last();
                MagicPool.Release(bar);
                _magicBars.Remove(bar);
            }
        }

        for (var i = 0; i < _magicBars.Count; i++)
        {
            _magicBars[i].Component.transform.position = transform.position + new Vector3(i * 0.075f, 0);
        }
    }
}
