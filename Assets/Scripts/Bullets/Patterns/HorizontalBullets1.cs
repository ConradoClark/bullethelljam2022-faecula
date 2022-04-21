using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;

public class HorizontalBullets1
{
    public static IEnumerable<IEnumerable<Action>> SimultaneousLine(Vector2 direction, PrefabPool bulletPool, Vector2 offset = default,
        float? speedOverride = null,
        [CanBeNull] AudioSource spawnSound = null)
    {
        if (!bulletPool.TryGetManyFromPool(6, out var objects)) yield break;
        var y = 8.45f;
        spawnSound?.Play();
        foreach (var bullet in objects.OfType<StraightMovingBullet>())
        {
            bullet.transform.position = new Vector3(-16f * direction.x, y) + (Vector3)offset;
            bullet.Direction = direction;
            y -= 1.75f;
            if (speedOverride != null) bullet.Speed = speedOverride.Value;
        }
    }

    public static IEnumerable<IEnumerable<Action>> SimultaneousLineFromLeft(PrefabPool bulletPool, Vector2 offset = default,
        float? speedOverride = null,
        [CanBeNull] AudioSource spawnSound = null)
    {
        return SimultaneousLine(Vector2.right, bulletPool, offset, speedOverride, spawnSound);
    }

    public static IEnumerable<IEnumerable<Action>> SimultaneousLineFromRight(PrefabPool bulletPool,
        Vector2 offset = default,
        float? speedOverride = null,
        [CanBeNull] AudioSource spawnSound = null)
    {
        return SimultaneousLine(Vector2.left, bulletPool, offset, speedOverride, spawnSound);
    }
}
