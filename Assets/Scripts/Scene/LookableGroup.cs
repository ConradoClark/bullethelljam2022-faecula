using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LookableGroup : MonoBehaviour
{
    private List<Lookable> _lookables = new List<Lookable>();

    public void AddLookable(Lookable lookable)
    {
        if (!_lookables.Contains(lookable)) _lookables.Add(lookable);
    }

    public void RemoveLookable(Lookable lookable)
    {
        if (_lookables.Contains(lookable)) _lookables.Remove(lookable);
    }

    public Lookable GetClickedLookable(Vector3 position)
    {
        return _lookables.FirstOrDefault(l => l.Overlaps(position));
    }
}
