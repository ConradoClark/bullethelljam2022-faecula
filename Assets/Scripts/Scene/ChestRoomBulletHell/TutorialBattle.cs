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

public class TutorialBattle : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;

    public string LockText1;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public ColorDefaults ColorDefaults;

    public TimerScriptable TimerRef;
    public HeartCounter HeartCounter;

    public GlobalTrigger ClearedChest;
    public AudioSource TutorialMusic;

    public BulletEmitter BulletEmitter;
    public EmissionReference HorizontalBulletEmission;
    public EmissionReference HorizontalSlowBulletEmission;

    public EmissionParametersScriptable LeftToRightEmission;
    public EmissionParametersScriptable RightToLeftEmission;

    private bool _dead = false;

    void OnEnable()
    {
        this.ObserveEvent(FaeStats.FaeEvents.OnDeath, OnEvent);
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        MachineryRef.Machinery.AddBasicMachine(HandleTutorial());
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

    IEnumerable<IEnumerable<Action>> HandleTutorial()
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

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The sigil detects your presence, initiating its defenses...");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        TutorialMusic?.Play();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Freedom calls me! Focus, fae!</color>");

        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = "Evade all attacks from the magic sigil"
        });


        BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params, true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params, true);

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        var offset = new Vector2(0, -0.5f);

        BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params.WithOffset(offset), true);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params.WithOffset(offset));
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);

        offset = new Vector2(0, -.75f);

        BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params.WithOffset(offset), true);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params.WithOffset(offset));
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        BulletEmitter.EmitBullets(HorizontalBulletEmission, RightToLeftEmission.Params);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

        if (!_dead) _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Not bad! Keep going!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        yield return HereComes().AsCoroutine();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        if (_dead) yield break;

        ClearedChest.Value = true;
        
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"The sigil deactivates.");
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeColor.Value)}>Fae</color> can finally get out of the chest!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);
        MachineryRef.Machinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(Constants.Scenes.ChestRoom, LoadSceneMode.Single);
        });
    }


    private IEnumerable<IEnumerable<Action>> HereComes()
    {
        var first = true;
        for (var i = 0; i < 30; i++)
        {
            if (i % 5 == 0) BulletEmitter.EmitBullets(HorizontalBulletEmission, LeftToRightEmission.Params.WithOffset(new Vector2(0, -0.4f + 0.6f * i % 1.4f)),true);
            else BulletEmitter.EmitBullets(HorizontalSlowBulletEmission, RightToLeftEmission.Params.WithOffset(new Vector2(0, -0.8f + 0.4f * i % 1.2f)), first);

            first = false;
            yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.5f);
        }
    }
}
