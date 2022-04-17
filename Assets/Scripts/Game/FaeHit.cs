using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class FaeHit : MonoBehaviour
{
    public Collider2D Collider;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer FaeWings;
    public SpriteRenderer FaeBody;
    public FaeStats FaeStats;

    public Color DamagedColor;
    public Color DangerColor;

    public bool CanBeHit { get; private set; }

    private void OnEnable()
    {
        CanBeHit = true;
    }

    public void Hit()
    {
        CanBeHit = false;
        MachineryRef.Machinery.AddBasicMachine(TakeHit());
    }

    private IEnumerable<IEnumerable<Action>> TakeHit()
    {
        FaeStats.TakeDamage();
        ChangeColorOnDamage();
        yield return Blink().AsCoroutine();
        CanBeHit = true;
    }

    private void ChangeColorOnDamage()
    {
        if (FaeStats.MaxHitPoints == 0) return;
        if (FaeStats.HitPoints / (float)FaeStats.MaxHitPoints >= 0.7f)
        {
            FaeBody.color = FaeWings.color = Color.white;
        }
        else if (FaeStats.HitPoints / (float)FaeStats.MaxHitPoints >= 0.4f)
        {
            FaeBody.color = FaeWings.color = DamagedColor;
        }
        else
        {
            FaeBody.color = FaeWings.color = DangerColor;
        }
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        for (var i = 0; i < 5; i++)
        {
            FaeBody.enabled = FaeWings.enabled = false;
            yield return TimeYields.WaitMilliseconds(TimerRef.Timer, 100);
            FaeBody.enabled = FaeWings.enabled = true;
            yield return TimeYields.WaitMilliseconds(TimerRef.Timer, 100);
        }
    }
}
