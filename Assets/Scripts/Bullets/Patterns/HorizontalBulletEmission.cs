using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Pooling;
using UnityEngine;

public class HorizontalBulletEmission : EmissionFunction
{
    public override IEnumerable<IEnumerable<Func<StepResult>>> EmitBullets(EmissionReference emissionRef, EmissionParameters emissionParams)
    {
        if (!emissionRef.BulletPool.TryGetManyFromPool(6, out var objects)) yield break;
        var y = 8.25f;
        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.position = new Vector3(-16f * emissionParams.Direction.x, y) + (Vector3)emissionParams.Offset;
            bullet.Direction = emissionParams.Direction;
            y -= 1.75f;
            yield return Enumerable.Repeat<Func<StepResult>>(() => new StepResult
            {
                HasSpawnedBullet = true,
                ShouldSkipFrame = false
            }, 1);
        }
    }
}
