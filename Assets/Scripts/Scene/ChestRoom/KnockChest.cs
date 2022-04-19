using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Generation;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Generation;
using Licht.Unity.Objects;
using UnityEngine;

public class KnockChest : MonoBehaviour
{
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public LockedChest LockedChest;
    public ColorDefaults ColorDefaults;
    public InteractiveAction LookAction;

    public BasicMachineryScriptable MachineryRef;
    public GlobalTrigger ClearedChest;
    private int _knockCount;

    protected void OnEnable()
    {
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        this.ObserveEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);

        MachineryRef.Machinery.AddBasicMachine(HandleKnock());
    }

    protected void OnDisable()
    {
        this.StopObservingEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);
    }

    private void OnEvent(InteractiveAction.InteractiveActionEvent obj)
    {
        MachineryRef.Machinery.AddBasicMachine(HandleKnock());
    }

    private class WeightedText : IWeighted<float>
    {
        public readonly string Text;
        public float Weight { get; } = 1f;

        public WeightedText(string text) => Text = text;
    }

    private IEnumerable<IEnumerable<Action>> HandleKnock()
    {
        _knockCount++;

        if (!ClearedChest.Value) HandleKnockingBeforeClearingChest(_knockCount);
        else HandleKnockingAfterClearingChest(_knockCount);
        yield return LockedChest.Knock().AsCoroutine();
    }

    private void HandleKnockingBeforeClearingChest(int knockCount)
    {
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

        if (knockCount <= 9 || knockCount % 3 != 0) return;
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

    private void HandleKnockingAfterClearingChest(int knockCount)
    {
        switch (knockCount)
        {
            case 3:
                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    $"The <color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.Objects.Value)}>chest</color> moves slightly.");
                break;
            case 6:
                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    $"The <color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.Objects.Value)}>chest</color> rattles around.");
                break;
            case 9:
                // make the chest break
                break;
        }
    }
}
