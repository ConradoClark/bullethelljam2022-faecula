using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;

public class HorizontalBullets1
{
    public static IEnumerable<IEnumerable<Action>> SimultaneousLineFromLeft(PrefabPool bulletPool, Vector2 offset = default,
        float? speedOverride = null)
    {
        if (!bulletPool.TryGetManyFromPool(6, out var objects)) yield break;
        var y = 8.45f;
        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.position = new Vector3(-16f, y) + (Vector3)offset;
            bullet.Direction = Vector2.right;
            y -= 1.75f;
            if (speedOverride != null) bullet.Speed = speedOverride.Value;
        }
    }

    public static IEnumerable<IEnumerable<Action>> SimultaneousLineFromRight(PrefabPool bulletPool, Vector2 offset = default,
        float? speedOverride = null)
    {
        if (!bulletPool.TryGetManyFromPool(6, out var objects)) yield break;
        var y = 8.45f;
        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.position = new Vector3(16f, y) + (Vector3)offset;
            bullet.Direction = Vector2.left;
            if (speedOverride != null) bullet.Speed = speedOverride.Value;
            y -= 1.75f;
        }
    }
}
