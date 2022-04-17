using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using UnityEngine;

public class InteractiveAction : ActionBase
{
    public string Description;
    public InteractiveGroup InteractiveGroup;
    public float ClickVerticalLimit;

    public string DefaultMessage;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    private IEventPublisher<InteractiveActionEvents, InteractiveActionEvent> _interactiveEventPublisher;

    public bool BlockAction;

    public enum InteractiveActionEvents
    {
        OnClick
    }

    public class InteractiveActionEvent
    {
        public ActionBase Source { get; set; }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();

        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
        _interactiveEventPublisher = this.RegisterAsEventPublisher<InteractiveActionEvents, InteractiveActionEvent>();

        MachineryRef.Machinery.AddBasicMachine(HandleLook());
    }

    private IEnumerable<IEnumerable<Action>> HandleLook()
    {
        while (isActiveAndEnabled)
        {
            while (!IsActive) yield return TimeYields.WaitOneFrameX;

            _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
            {
                Source = this,
                Text = Description
            });


            while (IsActive)
            {
                yield return TimeYields.WaitOneFrameX;

                if (BlockAction) continue;

                var triggered = Input.actions[Constants.Actions.Click].WasPerformedThisFrame();
                if (!triggered) continue;

                var mousePos = GetMousePosInWorld();
                if (mousePos.y < ClickVerticalLimit) continue;
                var result = InteractiveGroup != null ? InteractiveGroup.GetClickedInteractive(mousePos) : null;

                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    result == null ? DefaultMessage : result.Text);

                _interactiveEventPublisher.PublishEvent(InteractiveActionEvents.OnClick, new InteractiveActionEvent
                {
                    Source = this
                });
            }

            _textUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
            {
                Source = this,
            });
        }

        _textUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
        {
            Source = this,
        });
    }
}
