using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;

public class LookAction : PressableButton
{
    public string Description;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;

    protected override void OnEnable()
    {
        base.OnEnable();
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
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

            while (IsActive) yield return TimeYields.WaitOneFrameX; // Check for clicks on main camera

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
