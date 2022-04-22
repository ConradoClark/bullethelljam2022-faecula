using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletEmitter : MonoBehaviour
{
    public enum EmitterAudioTriggers
    {
        OnEmission,
        OnBulletSpawn
    }

    public PrefabPool IndicatorPrefab;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public IndicatorPositionList IndicatorPositionList;

    public void EmitBullets(EmissionReference emissionRef, EmissionParameters emissionParams, bool useIndicator=false)
    {
        MachineryRef.Machinery.AddBasicMachine(Emit(emissionRef, emissionParams, useIndicator));
    }

    private Vector3 GetIndicatorPosition(IndicatorPositions position)
    {
        return IndicatorPositionList.Indicators.FirstOrDefault(pos => position == pos.IndcatorPosition)?.Position
               ?? throw new Exception($"Position {position} not found");
    }

    protected IEnumerable<IEnumerable<Action>> Emit(EmissionReference emissionRef, EmissionParameters emissionParams, bool useIndicator)
    {
        if (useIndicator)
        {
            if (IndicatorPrefab.TryGetFromPool(out var indicator))
            {
                indicator.Component.transform.SetPositionAndRotation(
                    GetIndicatorPosition(emissionParams.IndicatorPosition),
                    Quaternion.FromToRotation(Vector2.right, emissionParams.IndicatorPositionDirection));
            }

            yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        }

        if (emissionRef.EmitterAudioTrigger == EmitterAudioTriggers.OnEmission)
        {
            emissionRef.SpawnSound.pitch = 0.8f + Random.value *0.4f;
            emissionRef.SpawnSound?.Play();
        }
        foreach (var step in emissionRef.EmissionFunction.EmitBullets(emissionRef, emissionParams).SelectMany(step => step))
        {
            var stepResult = step();
            if (stepResult.HasSpawnedBullet)
            {
                if (emissionRef.EmitterAudioTrigger == EmitterAudioTriggers.OnBulletSpawn)
                {
                    emissionRef.SpawnSound.pitch = 0.8f + Random.value * 0.4f;
                    emissionRef.SpawnSound?.Play();
                }
            }

            if (stepResult.ShouldSkipFrame) yield return TimeYields.WaitOneFrameX;
        }
    }
}
