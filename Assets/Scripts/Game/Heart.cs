using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using UnityEngine;

public class Heart : EffectPoolable
{
    public Sprite FullHeartSprite;
    public Sprite EmptyHeartSprite;
    public SpriteRenderer SpriteRenderer;

    public void SetEmpty()
    {
        SpriteRenderer.sprite = EmptyHeartSprite;
    }

    public override void OnActivation()
    {
        SpriteRenderer.sprite = FullHeartSprite;
    }

    public override bool IsEffectOver { get; protected set; }
}
