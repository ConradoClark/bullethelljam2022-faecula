using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Generation;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Generation;
using UnityEngine;

public class KnockAction : ActionBase
{
    public string Description;

    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textLockPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textUnlockPublisher;
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public Collider2D ChestCollider;
    public LockedChest LockedChest;
    public ColorDefaults ColorDefaults;
    public LookAction LookAction;

    protected override void OnEnable()
    {
        base.OnEnable();
        _textLockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textUnlockPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
        MachineryRef.Machinery.AddBasicMachine(HandleKnock());
    }

    private class WeightedText :IWeighted<float>
    {
        public string Text;
        public float Weight { get; } = 1f;

        public WeightedText(string text) => Text = text;
    }

    private IEnumerable<IEnumerable<Action>> HandleKnock()
    {
        while (isActiveAndEnabled)
        {
            while (!IsActive) yield return TimeYields.WaitOneFrameX;

            _textLockPublisher.PublishEvent(HelpText.HelpTextEvents.TextLock, new HelpText.TextChangedEvent
            {
                Source = this,
                Text = Description
            });

            var knockCount = 0;
            while (IsActive)
            {
                if (IsClickingOn(ChestCollider))
                {
                    knockCount++;
                    yield return LockedChest.Knock().AsCoroutine();
                    switch (knockCount)
                    {
                        case 3:
                            _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, 
                                $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Ouch!</color>");
                            break;
                        case 6:
                            _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                                $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Ow! It hurts!</color>");
                            break;
                        case 9:
                            _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                                "You knock over and over again, but the chest won't budge. Maybe it's a good idea to look around.");
                            LookAction.gameObject.SetActive(true);
                            break;
                    }

                    if (knockCount> 9 && knockCount % 3 == 0)
                    {
                        var possibleTexts = new[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Ouch, my head!</color>",
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Ow ow ow ow ow!</color>",
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Aaaaaaaaaah! The pain!</color>",
                        };

                        var randomDice = new WeightedDice<WeightedText>(
                            possibleTexts.Select(text => new WeightedText(text)),
                            new UnityRandomGenerator());

                        var chosenText = randomDice.Generate();
                        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, chosenText.Text);
                    }
                }
                yield return TimeYields.WaitOneFrameX;
            }

            _textUnlockPublisher.PublishEvent(HelpText.HelpTextEvents.TextUnlock, new HelpText.TextEvent
            {
                Source = this,
            });
        }
    }
}
