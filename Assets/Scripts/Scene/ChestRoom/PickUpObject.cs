using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class PickUpObject : Interactive
{
    public ColorDefaults ColorDefaults;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public GlobalTrigger PickupTrigger;
    public string ObjectName;

    protected IEventPublisher<TextLog.TextLogEvents, string> TextLogPublisher;

    public SpriteRenderer ObjectSpriteRenderer;
    public AudioSource PickupSound;

    protected override void OnEnable()
    {
        if (PickupTrigger.Value)
        {
            gameObject.SetActive(false);
            return;
        }

        base.OnEnable();
        TextLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

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

        MachineryRef.Machinery.AddBasicMachine(Pickup());
    }

    protected virtual IEnumerable<IEnumerable<Action>> Pickup()
    {
        if (PickupTrigger.Value) yield break;

        PickupSound.Play();
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"You picked up {AOrAn()} <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Objects.Value)}>{ObjectName}</color>!");
        PickupTrigger.Value = true;

        yield return Fade().AsCoroutine();
    }

    private string AOrAn()
    {
        return new []{'A', 'E', 'I', 'O', 'U'}.Contains(ObjectName.FirstOrDefault()) ? "an" : "a";
    }

    protected IEnumerable<IEnumerable<Action>> Fade()
    {
        yield return ObjectSpriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(0f)
            .Over(1f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();

        gameObject.SetActive(false);
    }

}

