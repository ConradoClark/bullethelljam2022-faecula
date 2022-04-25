using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;


public class TurnOffLightBox : Interactive
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public Interactive LookInteractive;

    public SpriteRenderer Sparks;
    public GlobalTrigger LightBoxTurnedOff;
    public GlobalTrigger FaultyPlugFixed;

    public AudioSource SwitchSound;
    public SpriteRenderer ScreenFlash;

    public Transform LitScene;

    protected IEventPublisher<TextLog.TextLogEvents, string> TextLogPublisher;

    protected override void OnEnable()
    {
        LitScene.gameObject.SetActive(FaultyPlugFixed.Value && !LightBoxTurnedOff.Value);
        ValidateLightBox();

        base.OnEnable();
        TextLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        this.ObserveEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);
    }

    protected override void OnDisable()
    {
        this.StopObservingEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);
    }

    private void OnEvent(InteractiveAction.InteractiveActionEvent obj)
    {
        if (obj.Group != Group || obj.Target != this) return;

        MachineryRef.Machinery.AddBasicMachine(HandleLightBox());

    }

    IEnumerable<IEnumerable<Action>> HandleLightBox()
    {
        LightBoxTurnedOff.Value = !LightBoxTurnedOff.Value;

        SwitchSound.Play();
        ValidateLightBox();

        if (LightBoxTurnedOff.Value)
        {
            TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The energy has been cut off." +
                                                                            (FaultyPlugFixed.Value ? "" : " It is now safe to touch the wires."));
        }
        else
        {
            TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, "The energy has been turned back on");
        }

        if (!FaultyPlugFixed.Value) yield break;

        ScreenFlash.enabled = true;
        ScreenFlash.color = new Color(1, 1, 1, 0);
        yield return ScreenFlash.GetAccessor()
            .Color.A
            .SetTarget(1f)
            .Over(0.2f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();

        LitScene.gameObject.SetActive(!LightBoxTurnedOff.Value);

        yield return ScreenFlash.GetAccessor()
            .Color.A
            .SetTarget(0f)
            .Over(0.2f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();
    }

    void ValidateLightBox()
    {
        if (LightBoxTurnedOff.Value)
        {
            Sparks.enabled = false;
            LookInteractive.Text =
                "This small box is connected to the power circuit in this place, somehow. It has been turned off.";
        }
        else
        {
            Sparks.enabled = !FaultyPlugFixed.Value;
            LookInteractive.Text =
                "This small box is connected to the power circuit in this place, somehow.";
        }
    }
}
