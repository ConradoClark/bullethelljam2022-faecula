using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class ChestRoomSpotlight : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer SpriteRenderer;

    public KnockAction KnockAction;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public ColorDefaults ColorDefaults;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(Storyboard());
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
    }

    void OnDisable()
    {
        this.UnregisterAsEventPublisher<TextLog.TextLogEvents, string>();
    }

    IEnumerable<IEnumerable<Action>> Flicker()
    {
        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(0)
            .Over(0.2f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(1)
            .Over(0.1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();
    }

    IEnumerable<IEnumerable<Action>> Storyboard()
    {
        Debug.Log("Current Multiplier: " + TimerRef.Timer.Multiplier);
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();
        yield return Flicker().AsCoroutine();

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "It is dark. You can barely see your surroundings.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        yield return Flicker().AsCoroutine();

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(0)
            .Over(1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "A faint, flickering light reaches your vision through the chest.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        yield return Flicker().AsCoroutine();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 4);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Is there anyway out?</color> - You think to yourself, pondering about your next step.");
        yield return Flicker().AsCoroutine();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();
        yield return Flicker().AsCoroutine();
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);
        yield return Flicker().AsCoroutine();


        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Hello? Is anybody there? Maybe I should try knocking.</color>");

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(0)
            .Over(1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();

        KnockAction.gameObject.SetActive(true);
    }
}
