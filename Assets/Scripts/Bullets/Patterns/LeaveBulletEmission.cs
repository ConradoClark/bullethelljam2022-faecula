using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Licht.Impl.Orchestration;
using UnityEngine;

public class LeaveBulletEmission : EmissionFunction
{
    public override IEnumerable<IEnumerable<Func<StepResult>>> EmitBullets(EmissionReference emissionRef, EmissionParameters emissionParams)
    {
        if (!emissionRef.BulletPool.TryGetFromPool(out var m) || !(m is StraightMovingBullet main)) yield break;
        main.transform.SetPositionAndRotation(
            new Vector3(-16f * emissionParams.Direction.x, 4 - 6 * emissionParams.Direction.y)
            + (Vector3)(emissionParams.Offset - emissionParams.Direction),
            Quaternion.FromToRotation(Vector2.right, emissionParams.Direction)
        );
        main.Direction = emissionParams.Direction;
        main.Speed = emissionParams.SpeedOverride > 0 ? emissionParams.SpeedOverride : main.Speed;

        var sub = emissionRef.SpecialEffects.FirstOrDefault(fx => fx.Key == "Sub");

        foreach (var value in Enumerable.Range(1, (int) emissionParams.Intensity))
        {
            if (!sub.EffectPool.TryGetFromPool(out var b) || !(b is DurationBullet bullet)) continue;
            bullet.transform.position = main.transform.position;

            yield return Enumerable.Repeat<Func<StepResult>>(() => new StepResult
            {
                HasSpawnedBullet = true,
                ShouldSkipFrame = false
            }, 1);

            foreach (var _ in TimeYields.WaitSeconds(emissionRef.TimerRef.Timer, emissionParams.Delay))
            {
                yield return Enumerable.Repeat<Func<StepResult>>(() => new StepResult
                {
                    HasSpawnedBullet = false,
                    ShouldSkipFrame = true
                }, 1);
            }
        }
    }
}
