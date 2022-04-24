using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class FaeStats : MonoBehaviour
{
    public enum FaeEvents
    {
        OnGraze,
        OnTakeDamage,
        OnDeath
    }

    public class FaeHitPointsEventHandler
    {
        public int MaxHitPoints;
        public int CurrentHitPoints;
    }

    public class FaeMagicEventHandler
    {
        public int MaxMagic;
        public int Magic;
    }

    public int MaxHitPoints;
    public int HitPoints { get; private set; }

    public int MaxMagic;

    public int Magic { get; private set; }

    private IEventPublisher<FaeEvents, FaeHitPointsEventHandler> _eventPublisher;
    private IEventPublisher<FaeEvents, FaeMagicEventHandler> _magicEventPublisher;

    private void OnEnable()
    {
        HitPoints = MaxHitPoints;
        _eventPublisher = this.RegisterAsEventPublisher<FaeEvents, FaeHitPointsEventHandler>();
        _magicEventPublisher = this.RegisterAsEventPublisher<FaeEvents, FaeMagicEventHandler>();
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

    public void Graze()
    {
        if (Magic >= MaxMagic) return;
        Magic++;

        _magicEventPublisher.PublishEvent(FaeEvents.OnGraze, new FaeMagicEventHandler
        {
            MaxMagic = MaxMagic,
            Magic = Magic
        });
    }

    public void ConsumeMagic(int amount)
    {
        if (Magic == 0) return;

        Magic -= amount;
        if (Magic < 0) Magic = 0;

        _magicEventPublisher.PublishEvent(FaeEvents.OnGraze, new FaeMagicEventHandler
        {
            MaxMagic = MaxMagic,
            Magic = Magic
        });
    }

}
