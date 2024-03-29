using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestRoomSpotlight : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer SpriteRenderer;

    public InteractiveAction KnockAction;
    private HelpContext _knockActionHelpContext;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public ColorDefaults ColorDefaults;
    public GlobalTrigger ClearedChest;

    public AudioSource LightFlickerSound;

    void OnEnable()
    {
        if (ClearedChest.Value)
        {
            SpriteRenderer.enabled = false;
            return;
        }
        MachineryRef.Machinery.AddBasicMachine(Storyboard());
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
        _knockActionHelpContext = KnockAction.GetComponent<HelpContext>();
    }

    void OnDisable()
    {
        this.UnregisterAsEventPublisher<TextLog.TextLogEvents, string>();
    }

    IEnumerable<IEnumerable<Action>> Flicker(bool reverse=false)
    {
        LightFlickerSound.pitch = 1.3f + Random.value * 0.4f;
        LightFlickerSound?.Play();
        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(reverse ? 1 : 0)
            .Over(0.1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(reverse? 0 : 1)
            .Over(0.1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();
    }

    IEnumerable<IEnumerable<Action>> Storyboard()
    {
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();
        yield return Flicker().AsCoroutine();

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "It is dark. You can barely see your surroundings.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        yield return Flicker().AsCoroutine();

        yield return SpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(0)
            .Over(1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "A faint, flickering light reaches your vision through the chest.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Is there anyway out?</color> - You think to yourself, pondering about your next step.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);


        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Hello? Is anybody there? Maybe I should try knocking.</color>");

        KnockAction.gameObject.SetActive(true);
        KnockAction.Description = "click on the chest to knock it from the inside";
        _knockActionHelpContext.Text = "knock (A) - try knocking on the chest from inside";
    }
}
