using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class Lookable : MonoBehaviour
{
    public LookableGroup Group;
    public Collider2D[] Colliders;

    public string Text;
    public string Key;

    public enum LookableEvents
    {
        OnLookableClicked
    }

    private IEventPublisher<LookableEvents, Lookable> _eventPublisher;

    private void OnEnable()
    {
        Group.AddLookable(this);
        _eventPublisher = this.RegisterAsEventPublisher<LookableEvents, Lookable>();
    }

    private void OnDisable()
    {
        Group.RemoveLookable(this);
        this.UnregisterAsEventPublisher<LookableEvents, Lookable>();
    }

    protected virtual string GetText => Text;

    public bool Overlaps(Vector3 position, bool triggerEvent = true)
    {
        var overlaps = Colliders.Any(col => col.OverlapPoint(position));
        if (triggerEvent && overlaps) _eventPublisher.PublishEvent(LookableEvents.OnLookableClicked, this);
        return overlaps;
    }
}
