using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnHearts : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public PrefabPool HeartsPool;
    public float Delay;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(Hearts());
    }

    IEnumerable<IEnumerable<Action>> Hearts()
    {
        while (isActiveAndEnabled)
        {
            if (HeartsPool.TryGetFromPool(out var heart))
            {
                var spawnY = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
                var spawnX = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);


                heart.Component.transform.position = new Vector2(spawnX, spawnY);
            }
            yield return TimeYields.WaitSeconds(TimerRef.Timer, Delay <= 0 ? 0.01f : Delay);
        }
    }
}
