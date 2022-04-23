using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Generation;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Generation;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class KnockChest : Interactive
{
    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    public LockedChest LockedChest;
    public ColorDefaults ColorDefaults;
    public InteractiveAction LookAction;

    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public GlobalTrigger ClearedChestTrigger;
    public GlobalTrigger OpenedChestTrigger;
    private int _knockCount;
    private bool _knocking;

    public Transform Chest;
    public Transform OpenedChest;
    public SpriteRenderer ChestFront;
    public Transform Fae;
    public SpriteRenderer ScreenFlash;

    public AudioSource ActionSound;

    protected override void OnEnable()
    {
        if (OpenedChestTrigger.Value)
        {
            Chest.gameObject.SetActive(false);
            OpenedChest.gameObject.SetActive(true);
            ChestFront.enabled = false;
            return;
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

        ActionSound?.Play();
        _knocking = true;
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

        if (!ClearedChestTrigger.Value) yield return HandleKnockingBeforeClearingChest(_knockCount).AsCoroutine();
        else yield return HandleKnockingAfterClearingChest(_knockCount).AsCoroutine();

        _knocking = false;
    }

    private IEnumerable<IEnumerable<Action>> HandleKnockingBeforeClearingChest(int knockCount)
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

        yield return LockedChest.Knock().AsCoroutine();

        if (knockCount <= 9 || knockCount % 3 != 0) yield break;
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

    private IEnumerable<IEnumerable<Action>> HandleKnockingAfterClearingChest(int knockCount)
    {
        if (knockCount < 9)
        {
            yield return LockedChest.Knock().AsCoroutine();
        }
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
                ScreenFlash.enabled = true;
                ScreenFlash.color = new Color(1, 1, 1, 0);
                yield return ScreenFlash.GetAccessor()
                    .Color.A
                    .SetTarget(1f)
                    .Over(0.2f)
                    .UsingTimer(TimerRef.Timer)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .Build();

                Chest.gameObject.SetActive(false);
                OpenedChest.gameObject.SetActive(true);
                
                yield return ScreenFlash.GetAccessor()
                    .Color.A
                    .SetTarget(0f)
                    .Over(1.4f)
                    .UsingTimer(TimerRef.Timer)
                    .Easing(EasingYields.EasingFunction.ExponentialEaseIn)
                    .Build();

                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeColor.Value)}>fae</color> is finally free!");

                Fae.gameObject.SetActive(true);

                OpenedChestTrigger.Value = true;

                LookAction.DefaultMessage = "There's nothing here of relevant importance.";

                yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>Holy fountain fairy, where am I?</color>");

                ChestFront.enabled = false;

                yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);

                _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
                    $"<color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.FaeculaSpeechColor.Value)}>I gotta get outta here, my people must be missing me right now... How did I come to this place anyway?</color>");

                break;
        }
    }
}
