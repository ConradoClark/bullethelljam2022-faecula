using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class FaeDeath : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public TimerScriptable GlobalTimerRef;
    public FaeFollowMouse FollowMouse;

    public PressableButton RetryButton;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    private IEventPublisher<FaeStats.FaeEvents> _eventPublisher;

    private void OnEnable()
    {
        _eventPublisher = this.RegisterAsEventPublisher<FaeStats.FaeEvents>();
        this.ObserveEvent<FaeStats.FaeEvents, FaeStats.FaeHitPointsEventHandler>(FaeStats.FaeEvents.OnTakeDamage, OnEvent);
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        this.ObserveEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(InteractiveAction.InteractiveActionEvents.OnClick, OnInteractive);
    }

    private void OnInteractive(InteractiveAction.InteractiveActionEvent obj)
    {
        if (RetryButton != obj.Source) return;

        MachineryRef.Machinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        });
    }

    private void OnEvent(FaeStats.FaeHitPointsEventHandler obj)
    {
        if (obj.CurrentHitPoints != 0) return;

        FollowMouse.enabled = false;
        _eventPublisher.PublishEvent(FaeStats.FaeEvents.OnDeath);
        MachineryRef.Machinery.AddBasicMachine(Die());
    }

    private IEnumerable<IEnumerable<Action>> Die()
    {
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Fae has met their fate...");
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "Click on retry to try again.");
        
        yield return DeathAnim().AsCoroutine();

        _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
        {
            Source = this,
            Text = ""
        });

        RetryButton.gameObject.SetActive(true);
    }

    private IEnumerable<IEnumerable<Action>> Rotate()
    {
        const float intensity = 0.01f;
        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2f, (ms) =>
        {
            transform.Rotate(0, 0, intensity * (float)ms);
        });

        transform.rotation = Quaternion.identity;
    }

    private IEnumerable<IEnumerable<Action>> Slowdown()
    {
        yield return new LerpBuilder(value => TimerRef.Timer.Multiplier = value, () => (float)TimerRef.Timer.Multiplier)
            .Decrease(0.75f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GlobalTimerRef.Timer)
            .Build();

        yield return new LerpBuilder(value => TimerRef.Timer.Multiplier = value, () => (float)TimerRef.Timer.Multiplier)
            .Increase(0.75f)
            .Over(1.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(GlobalTimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> DeathAnim()
    {
        var scale = transform.GetAccessor()
            .UniformScale()
            .SetTarget(0)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return scale.Combine(Rotate().AsCoroutine())
            .Combine(Slowdown().AsCoroutine());
    }
}
