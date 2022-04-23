using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Licht.Impl.Orchestration;
using UnityEngine;

public class RadialBulletEmission : EmissionFunction
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

        foreach (var value in Enumerable.Range(1, (int) emissionParams.Intensity))
        {
            if (!emissionRef.BulletPool.TryGetFromPool(out var b) || !(b is StraightMovingBullet bullet)) continue;
            bullet.Speed = main.Speed * 2;
            bullet.Direction = new Vector2((float)Math.Cos(Mathf.Deg2Rad * value * emissionParams.Range), (float)Math.Sin(Mathf.Deg2Rad * value * emissionParams.Range));
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
