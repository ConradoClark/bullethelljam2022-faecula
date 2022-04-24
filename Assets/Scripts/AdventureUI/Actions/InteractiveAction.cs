using System;
using System.Collections.Generic;
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

    protected IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> TextLockPublisher;
    protected IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> TextUnlockPublisher;
    protected IEventPublisher<TextLog.TextLogEvents, string> TextLogPublisher;

    protected IEventPublisher<InteractiveActionEvents, InteractiveActionEvent> InteractiveEventPublisher;

    public bool BlockAction;
    public bool PublishesMessage;

    public enum InteractiveActionEvents
    {
        OnClick
    }

    public class InteractiveActionEvent
    {
        public ActionBase Source { get; set; }
        public InteractiveGroup Group { get; set; }
        public Interactive Target { get; set; }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        TextLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        TextUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();

        TextLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
        InteractiveEventPublisher = this.RegisterAsEventPublisher<InteractiveActionEvents, InteractiveActionEvent>();

        MachineryRef.Machinery.AddBasicMachine(HandleLook());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.UnregisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        this.UnregisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();

        this.UnregisterAsEventPublisher<TextLog.TextLogEvents, string>();
        this.UnregisterAsEventPublisher<InteractiveActionEvents, InteractiveActionEvent>();
    }

    private IEnumerable<IEnumerable<Action>> HandleLook()
    {
        while (isActiveAndEnabled)
        {
            while (!IsActive && isActiveAndEnabled)
                yield return TimeYields.WaitOneFrameX;

            TextLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
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

                if (result!=null)
                {
                    if (PublishesMessage && !string.IsNullOrWhiteSpace(result.Text))
                    {
                        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, result.Text);
                    }
                }
                else
                {
                    TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, DefaultMessage);
                }

                InteractiveEventPublisher.PublishEvent(InteractiveActionEvents.OnClick, new InteractiveActionEvent
                {
                    Source = this,
                    Group = InteractiveGroup,
                    Target = result,
                });
                Debug.Log(" published event");
            }

            Debug.Log(" not active anymore, " + gameObject.name);

            TextUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
            {
                Source = this,
            });
        }

        TextUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
        {
            Source = this,
        });
    }
}
