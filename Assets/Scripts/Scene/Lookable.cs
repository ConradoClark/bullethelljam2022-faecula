using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lookable : MonoBehaviour
{
    public LookableGroup Group;
    public Collider2D[] Colliders;

    public string Text;

    private void OnEnable()
    {
        Group.AddLookable(this);
    }

    private void OnDisable()
    {
        Group.RemoveLookable(this);
    }

    protected virtual string GetText => Text;

    public bool Overlaps(Vector3 position)
    {
        return Colliders.Any(col =>col.OverlapPoint(position));
    }
}
