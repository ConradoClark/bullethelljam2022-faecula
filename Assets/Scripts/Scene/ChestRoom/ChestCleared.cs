using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.U2D;

public class ChestCleared : MonoBehaviour
{
    public PressableButton KnockAction;
    public PressableButton LookAction;
    public PressableButton InteractAction;
    public DisableActionsAndGoToBulletHell1 BulletHellInteraction;

    public GlobalTrigger ClearedChest;

    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public ColorDefaults ColorDefaults;

    protected Camera DefaultCamera;
    protected PixelPerfectCamera Ppc;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;

    private void OnEnable()
    {
        if (!ClearedChest.Value) return;

        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();
        DefaultCamera = Camera.allCameras.First(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        Ppc = DefaultCamera.GetComponent<PixelPerfectCamera>();
        Ppc.enabled = false;
        DefaultCamera.orthographicSize = 0.01f;

        KnockAction.gameObject.SetActive(true);
        LookAction.gameObject.SetActive(true);
        InteractAction.gameObject.SetActive(true);
        BulletHellInteraction.enabled = false;
        MachineryRef.Machinery.AddBasicMachine(ZoomOut());
    }

    IEnumerable<IEnumerable<Action>> ZoomOut()
    {
        yield return new LerpBuilder(val => DefaultCamera.orthographicSize = val, () => DefaultCamera.orthographicSize)
            .SetTarget(8.4375f)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .UsingTimer(TimerRef.Timer)
            .Build();

        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
            $"The <color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.Sigils.Value)}>magic sigil</color> is broken."
            );
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry,
            $"You wonder... is the <color=#{ColorUtility.ToHtmlStringRGBA(ColorDefaults.Objects.Value)}>chest</color> is weak enough now?");

        Ppc.enabled = true;
    }
}
