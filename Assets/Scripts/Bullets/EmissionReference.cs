using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class EmissionReference : MonoBehaviour
{
    [Serializable]
    public struct EmissionSpecialEffect
    {
        public string Key;
        public PrefabPool EffectPool;
    }

    public TimerScriptable TimerRef;
    public EmissionFunction EmissionFunction;
    public PrefabPool BulletPool;
    public EmissionSpecialEffect[] SpecialEffects;
    public AudioSource SpawnSound;
    public BulletEmitter.EmitterAudioTriggers EmitterAudioTrigger;
}
