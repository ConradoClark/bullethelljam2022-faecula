using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomRockSequence : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;

    public string LockText1;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public ColorDefaults ColorDefaults;

    public TimerScriptable TimerRef;
    public HeartCounter HeartCounter;

    public AudioSource RandomRockMusic;

    public BulletEmitter BulletEmitter;
    public EmissionReference HorizontalBulletEmission;
    public EmissionReference HorizontalSlowBulletEmission;
    public EmissionReference ShowerBulletEmission;
    public EmissionReference RadialBulletEmission;
    public EmissionReference CandyBulletEmission;

    public EmissionParametersScriptable LeftToRightEmission;
    public EmissionParametersScriptable RightToLeftEmission;
    public EmissionParametersScriptable ShowerEmission;
    public EmissionParametersScriptable RadialEmission;
    public EmissionParametersScriptable DefaultEmission;

    private bool _dead;

    void OnEnable()
    {
        this.ObserveEvent(FaeStats.FaeEvents.OnDeath, OnEvent);
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        MachineryRef.Machinery.AddBasicMachine(HandleBulletHell());
    }

    void OnDisable()
    {
        this.StopObservingEvent(FaeStats.FaeEvents.OnDeath, OnEvent);
    }

    private void OnEvent()
    {
        _dead = true;
        _textUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
        {
            Source = this
        });
    }

    IEnumerable<IEnumerable<Action>> HandleBulletHell()
    {
        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = LockText1
        });

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Spirits of past fairies reach out to you...");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        HeartCounter.gameObject.SetActive(true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The rock you've smashed sent out flying debris! Watch out!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        RandomRockMusic?.Play();

        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = "Evade the rock debris!"
        });

        for (var i = 1; i < 8; i++)
        {
            BulletEmitter.EmitBullets(RadialBulletEmission, RadialEmission.Params
                .WithOffset(new Vector2(0, i * 0.5f))
                .WithSpeedOverride(0.025f), i == 0);
            yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.3f);
        }

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 10f);

        for (var i = 1; i < 8; i++)
        {
            BulletEmitter.EmitBullets(RadialBulletEmission, RadialEmission.Params
                .WithIndicatorPosition(IndicatorPositions.East, Vector2.left)
                .WithDirection(Vector2.left)
                .WithOffset(new Vector2(0, 1f + i * -0.5f))
                .WithSpeedOverride(0.025f), i == 0);
            yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.3f);
        }

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 10f);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithIndicatorPosition(IndicatorPositions.SouthWest, new Vector2(1,1))
            .WithDirection(new Vector2(1,1)), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithIndicatorPosition(IndicatorPositions.NorthWest, new Vector2(1, -1))
            .WithDirection(new Vector2(1, -1)), true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5f);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithDelay(0.15f)
            .WithIntensity(70));
        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithDelay(0.15f)
            .WithOffset(new Vector2(0,-3))
            .WithIntensity(70));
        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithDelay(0.15f)
            .WithOffset(new Vector2(0, 3))
            .WithIntensity(70));

        for (var i = 1; i < 10; i++)
        {
            BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params
                .WithOffset(new Vector2(0,2f -(i < 5? i : 10 - i)*0.75f)));
            yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.45f);
        }

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

        if (_dead) yield break;

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Goodbye, rock.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);
        MachineryRef.Machinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(Constants.Scenes.ChestRoom, LoadSceneMode.Single);
        });
    }

}
