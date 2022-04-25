using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class ReplaceWires : Interactive
{
    public ColorDefaults ColorDefaults;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    protected IEventPublisher<TextLog.TextLogEvents, string> TextLogPublisher;

    public GlobalTrigger FixedFaultyPlug;
    public AudioSource FixWiresSound;

    public SpriteRenderer FixedPlug;

    protected override void OnEnable()
    {
        if (FixedFaultyPlug.Value)
        {
            FixedPlug.enabled = true;
            return;
        }

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

        FixedPlug.enabled = true;
        FixWiresSound.Play();
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"You carefully replace the <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Objects.Value)}>wires</color>. The lights should work as expected now.");
        FixedFaultyPlug.Value = true;
    }
}