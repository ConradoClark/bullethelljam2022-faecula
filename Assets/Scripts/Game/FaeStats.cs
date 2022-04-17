using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class FaeStats : MonoBehaviour
{
    public enum FaeEvents
    {
        OnTakeDamage
    }

    public class FaeHitPointsEventHandler
    {
        public int MaxHitPoints;
        public int CurrentHitPoints;
    }

    public int MaxHitPoints;
    public int HitPoints { get; private set; }

    private IEventPublisher<FaeEvents, FaeHitPointsEventHandler> _eventPublisher;

    private void OnEnable()
    {
        HitPoints = MaxHitPoints;
        _eventPublisher = this.RegisterAsEventPublisher<FaeEvents, FaeHitPointsEventHandler>();
    }

    public void TakeDamage()
    {
        HitPoints--;
        _eventPublisher.PublishEvent(FaeEvents.OnTakeDamage, new FaeHitPointsEventHandler
        {
            MaxHitPoints = MaxHitPoints,
            CurrentHitPoints = HitPoints
        });
    }

}
