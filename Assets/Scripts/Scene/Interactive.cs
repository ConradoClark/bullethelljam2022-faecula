using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public InteractiveGroup Group;
    public Collider2D[] Colliders;

    public string Text;
    public string Key;

    public enum InteractiveEvents
    {
        OnInteractiveClicked
    }

    private IEventPublisher<InteractiveEvents, Interactive> _eventPublisher;

    private void OnEnable()
    {
        Group.AddInteractive(this);
        _eventPublisher = this.RegisterAsEventPublisher<InteractiveEvents, Interactive>();
    }

    private void OnDisable()
    {
        Group.RemoveInteractive(this);
        this.UnregisterAsEventPublisher<InteractiveEvents, Interactive>();
    }

    protected virtual string GetText => Text;

    public bool Overlaps(Vector3 position, bool triggerEvent = true)
    {
        var overlaps = Colliders.Any(col => col.OverlapPoint(position));
        if (triggerEvent && overlaps) _eventPublisher.PublishEvent(InteractiveEvents.OnInteractiveClicked, this);
        return overlaps;
    }
}
