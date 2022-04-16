using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
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

        yield break;
    }
}
