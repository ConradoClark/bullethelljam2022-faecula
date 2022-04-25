using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;

public class MagicBallBattle : MonoBehaviour
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

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "You feel a surge of power coming from the mysterious sphere...");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"Try getting dangerously close to the <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Objects.Value)}>bullets</color>, without touching them.");

        RandomRockMusic?.Play();

        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = "Fill up your magic and slowdown time by holding the left mouse button!"
        });

        BulletEmitter.EmitBullets(RadialBulletEmission, RadialEmission.Params
            .WithDelay(0.1f)
            .WithIntensity(150)
            .WithSpeedOverride(0.01f), true);

        BulletEmitter.EmitBullets(RadialBulletEmission, RadialEmission.Params
            .WithSpeedOverride(0.01f)
            .WithDelay(0.1f)
            .WithIntensity(150)
            .WithIndicatorPosition(IndicatorPositions.East, Vector2.left)
            .WithDirection(Vector2.left), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithIndicatorPosition(IndicatorPositions.NorthWest, new Vector2(1,-1))
            .WithDelay(0.15f)
            .WithDirection(new Vector2(1,-1))
            .WithIntensity(40), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithIndicatorPosition(IndicatorPositions.NorthEast, new Vector2(-1, -1))
            .WithDelay(0.15f)
            .WithDirection(new Vector2(-1, 1))
            .WithIntensity(40), true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4f);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithIndicatorPosition(IndicatorPositions.West, Vector2.right)
            .WithDelay(0.15f)
            .WithDirection(Vector2.right)
            .WithIntensity(40), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithIndicatorPosition(IndicatorPositions.East, Vector2.left)
            .WithDelay(0.15f)
            .WithDirection(Vector2.left)
            .WithIntensity(40), true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 15f);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.SouthWest, new Vector2(1, 1))
            .WithDirection(new Vector2(1, 1)), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.NorthWest, new Vector2(1, -1))
            .WithDirection(new Vector2(1, -1)), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.SouthEast, new Vector2(-1, 1))
            .WithDirection(new Vector2(-1, 1)), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.NorthEast, new Vector2(-1, -1))
            .WithDirection(new Vector2(-1, -1)), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.West, Vector2.right)
            .WithDirection(Vector2.right), true);

        BulletEmitter.EmitBullets(ShowerBulletEmission, ShowerEmission.Params
            .WithSpeedOverride(0.1f)
            .WithIndicatorPosition(IndicatorPositions.East, Vector2.left)
            .WithDirection(Vector2.left), true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 15f);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Phew! What was that!?");


        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithSpeedOverride(0.3f)
            .WithIndicatorPosition(IndicatorPositions.West, Vector2.right)
            .WithDelay(0.15f)
            .WithDirection(Vector2.right)
            .WithIntensity(40), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithSpeedOverride(0.3f)
            .WithIndicatorPosition(IndicatorPositions.East, Vector2.left)
            .WithDelay(0.15f)
            .WithDirection(Vector2.left)
            .WithIntensity(40), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithSpeedOverride(0.3f)
            .WithIndicatorPosition(IndicatorPositions.NorthWest, new Vector2(1,-1))
            .WithDelay(0.15f)
            .WithDirection(new Vector2(1, -1))
            .WithIntensity(40), true);

        BulletEmitter.EmitBullets(CandyBulletEmission, DefaultEmission.Params
            .WithSpeedOverride(0.3f)
            .WithIndicatorPosition(IndicatorPositions.NorthEast, new Vector2(-1,-1))
            .WithDelay(0.15f)
            .WithDirection(new Vector2(-1, -1))
            .WithIntensity(40), true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 10);

        if (_dead) yield break;

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The sigil is finally broken!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);
        MachineryRef.Machinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(Constants.Scenes.ChestRoom, LoadSceneMode.Single);
        });
    }

}
