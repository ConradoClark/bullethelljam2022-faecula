using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class BreakableRock : Interactive
{
    public KnockEffect KnockEffect;
    public BasicMachineryScriptable MachineryRef;

    public SpriteRenderer Rock;
    public SpriteRenderer BrokenRock;

    public GlobalTrigger BrokenRockTrigger;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;
    private bool _knocking;

    protected override void OnEnable()
    {
        if (BrokenRockTrigger.Value)
        {
            Rock.enabled = false;
            BrokenRock.enabled = true;
        }

        base.OnEnable();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        this.ObserveEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);
    }

    private void OnEvent(InteractiveAction.InteractiveActionEvent obj)
    {
        if (obj.Group != Group || obj.Target != this) return;
        if (_knocking) return;

        _knocking = true;
        MachineryRef.Machinery.AddBasicMachine(KnockEffect.Knock().AsCoroutine()
            .Then(BreakRock().AsCoroutine()));
    }

    private IEnumerable<IEnumerable<Action>> BreakRock()
    {
        yield return TimeYields.WaitOneFrameX;
        Rock.enabled = false;
        BrokenRock.enabled = true;
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The poor rock crumbled...");
        _knocking = false;
        BrokenRockTrigger.Value = true;
    }
}
