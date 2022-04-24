using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletProcessor : MonoBehaviour
{
    private BaseBullet[] _bullets;
    public Transform BulletPoolContainer;
    public FaeHit FaeHit;

    public GlobalTrigger GrazeEnabled;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public float GrazeDelay;

    public AudioSource GrazeSound;
    public PrefabPool GrazeEffect;
    public FaeStats FaeStats;

    private List<Collider2D> _grazingBullets = new List<Collider2D>();

    void OnEnable()
    {
        _grazingBullets = new List<Collider2D>();
        MachineryRef.Machinery.AddBasicMachine(Process());
    }

    IEnumerable<IEnumerable<Action>> Process()
    {
        yield return TimeYields.WaitOneFrameX;
        _bullets = BulletPoolContainer.GetComponentsInChildren<BaseBullet>(true);

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

                if (bullet.Collider.OverlapCollider(contactFilter, results) == 0)
                {
                    if (GrazeEnabled.Value &&
                        !_grazingBullets.Contains(bullet.Collider) &&
                        Vector2.Distance(bullet.transform.position, FaeHit.transform.position) < 1f)
                    {
                        _grazingBullets.Add(bullet.Collider);
                        MachineryRef.Machinery.AddBasicMachine(Graze(bullet));
                    }
                    continue;
                }

                FaeHit.Hit();
                break;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    IEnumerable<IEnumerable<Action>> Graze(BaseBullet bullet)
    {
        while(Vector2.Distance(bullet.transform.position, FaeHit.transform.position) < 1f)
        {
            if (GrazeEffect.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position =
                    bullet.transform.position + (FaeHit.transform.position - bullet.transform.position) / 2
                                              + (Vector3) (Random.insideUnitCircle * 0.4f);

            }

            GrazeSound.pitch = 1f + Random.value * 0.4f;
            GrazeSound.Play();
            FaeStats.Graze();
            yield return TimeYields.WaitSeconds(TimerRef.Timer, GrazeDelay);
        }

        _grazingBullets.Remove(bullet.Collider);
    }
}
