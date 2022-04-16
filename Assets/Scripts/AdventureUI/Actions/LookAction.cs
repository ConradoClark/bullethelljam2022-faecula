using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using UnityEngine;

public class LookAction : ActionBase
{
    public string Description;
    public LookableGroup LookableGroup;
    public float ClickVerticalLimit;

    public string DefaultMessage;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    protected override void OnEnable()
    {
        base.OnEnable();
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();

        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

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
                var triggered = Input.actions[Constants.Actions.Click].WasPerformedThisFrame();
                if (!triggered) continue;

                var mousePos = GetMousePosInWorld();
                if (mousePos.y < ClickVerticalLimit) continue;
                var result = LookableGroup.GetClickedLookable(mousePos);
                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    result == null ? DefaultMessage : result.Text);
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
