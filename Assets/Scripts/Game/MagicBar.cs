using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Pooling;
using UnityEngine;

public class MagicBar : EffectPoolable
{
    public override void OnActivation()
    {
    }

    public override bool IsEffectOver { get; protected set; }
}
