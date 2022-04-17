using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class BulletProcessor : MonoBehaviour
{
    private ScreenBullet[] _bullets;
    public Transform BulletPoolContainer;
    public FaeHit FaeHit;

    public BasicMachineryScriptable MachineryRef;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(Process());
    }

    IEnumerable<IEnumerable<Action>> Process()
    {
        yield return TimeYields.WaitOneFrameX;
        _bullets = BulletPoolContainer.GetComponentsInChildren<ScreenBullet>(true);
        Debug.Log("found bullets: " + _bullets.Length);

        var faeMask = LayerMask.NameToLayer("Fae");
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(~faeMask);

        while (isActiveAndEnabled)
        {
            if (!FaeHit.CanBeHit)
            {
                yield return TimeYields.WaitOneFrameX;
                continue;
            }

            var activeBullets = _bullets.Where(bullet => bullet.isActiveAndEnabled).ToArray();
            foreach (var bullet in activeBullets)
            {
                if (!FaeHit.CanBeHit) break;
                if (Vector2.Distance(bullet.transform.position, FaeHit.transform.position) > 2f) continue;

                var results = new Collider2D[1];
                
                if (bullet.Collider.OverlapCollider(contactFilter, results) == 0) continue;

                FaeHit.Hit();
                break;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
