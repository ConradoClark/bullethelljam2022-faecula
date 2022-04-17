using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class TutorialBattle : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;

    public string LockText1;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public ColorDefaults ColorDefaults;

    public TimerScriptable TimerRef;
    public PrefabPool HorizontalBulletsPool;
    public PrefabPool SlowHorizontalBulletsPool;

    void OnEnable()
    {
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        MachineryRef.Machinery.AddBasicMachine(HandleTutorial());
    }

    IEnumerable<IEnumerable<Action>> HandleTutorial()
    {
        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = LockText1
        });

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Spirits of past fairies reach out to you...");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The sigil detects your presence, initiating its defenses...");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Freedom calls me! Focus, fae!</color>");

        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = "Evade all attacks from the magic sigil"
        });

        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);


        var offset = new Vector2(0, -0.5f);

        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, offset).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, offset).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);

        offset = new Vector2(0, -.75f);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, offset).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, offset).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Not bad! Keep going!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, -0.4f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, 0.4f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, 0.8f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, 1.2f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, 1.6f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0, 1.4f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.5f);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, new Vector2(0, 1.6f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, new Vector2(0, 0.8f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, new Vector2(0, 1.4f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromRight(HorizontalBulletsPool, new Vector2(0, 0.4f)).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);
        yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool).AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.25f);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Here comes!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        yield return HereComes().AsCoroutine();
    }


    private IEnumerable<IEnumerable<Action>> HereComes()
    {
        for (var i = 0; i < 30; i++)
        {
            if (i % 5 == 0) yield return HorizontalBullets1.SimultaneousLineFromLeft(HorizontalBulletsPool, new Vector2(0,-0.8f)).AsCoroutine();
            else yield return HorizontalBullets1.SimultaneousLineFromRight(SlowHorizontalBulletsPool, new Vector2(0, -0.8f + 0.4f * i % 0.8f)).AsCoroutine();
            yield return TimeYields.WaitSeconds(TimerRef.Timer, 0.5f);
        }
    }
}
