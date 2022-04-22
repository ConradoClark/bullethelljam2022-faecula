using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Pooling;
using UnityEngine;

public abstract class EmissionFunction : MonoBehaviour
{
    public class StepResult {
        public bool HasSpawnedBullet { get; set; }
        public bool ShouldSkipFrame { get; set; }
    }
    public abstract IEnumerable<IEnumerable<Func<StepResult>>> EmitBullets(EmissionReference emissionRef, EmissionParameters emissionParams);
}
