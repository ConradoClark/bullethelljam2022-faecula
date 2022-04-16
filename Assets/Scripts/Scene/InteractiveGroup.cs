using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractiveGroup : MonoBehaviour
{
    private readonly List<Interactive> _interactiveObjects = new List<Interactive>();

    public void AddInteractive(Interactive interactive)
    {
        if (!_interactiveObjects.Contains(interactive)) _interactiveObjects.Add(interactive);
    }

    public void RemoveInteractive(Interactive interactive)
    {
        if (_interactiveObjects.Contains(interactive)) _interactiveObjects.Remove(interactive);
    }

    public Interactive GetClickedInteractive(Vector3 position)
    {
        return _interactiveObjects.ToArray().FirstOrDefault(l => l.Overlaps(position));
    }
}
