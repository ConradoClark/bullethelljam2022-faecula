using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShowerBulletEmission : EmissionFunction
{
    public override IEnumerable<IEnumerable<Func<StepResult>>> EmitBullets(EmissionReference emissionRef, EmissionParameters emissionParams)
    {
        if (!emissionRef.BulletPool.TryGetManyFromPool(Mathf.CeilToInt(6 * emissionParams.Intensity), out var objects)) yield break;

        var fx = emissionRef.SpecialEffects.FirstOrDefault(fx => fx.Key == "Bubbles");

        var delay = 0;

        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.SetPositionAndRotation(
                new Vector3(-16f * emissionParams.Direction.x, 4 - 6 * emissionParams.Direction.y)
                + (Vector3)(emissionParams.Offset - emissionParams.Direction * (delay % 6) + Random.insideUnitCircle * emissionParams.Range),
                Quaternion.FromToRotation(Vector2.right, emissionParams.Direction)
            );
            bullet.Direction = Vector2.zero;
        }

        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.SetPositionAndRotation(
                new Vector3(-16f * emissionParams.Direction.x, 4 - 6 * emissionParams.Direction.y)
                + (Vector3)(emissionParams.Offset - emissionParams.Direction * (delay % 6) + Random.insideUnitCircle * emissionParams.Range),
                Quaternion.FromToRotation(Vector2.right, emissionParams.Direction)
            );

            bullet.Direction = emissionParams.Direction;
            bullet.Speed = emissionParams.SpeedOverride > 0 ? emissionParams.SpeedOverride : bullet.Speed;

            if (fx.EffectPool!=null && fx.EffectPool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position =
                    bullet.transform.position + (Vector3) emissionParams.Direction * (delay % 6) * 3;
            }

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

            delay++;
        }
    }
}