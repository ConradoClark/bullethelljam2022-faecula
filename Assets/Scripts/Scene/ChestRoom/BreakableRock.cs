using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class BreakableRock : Interactive
{
    public KnockEffect KnockEffect;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public SpriteRenderer Rock;
    public SpriteRenderer BrokenRock;

    public GlobalTrigger BrokenRockTrigger;

    public string Message;
    public string TeleportToScene;

    public AudioSource SmashAudio;
    public PressableButtonGroup Buttons;

    protected Camera DefaultCamera;
    protected PixelPerfectCamera Ppc;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;
    private bool _knocking;

    protected override void OnEnable()
    {
        if (BrokenRockTrigger.Value)
        {
            Rock.enabled = false;
            BrokenRock.enabled = true;
        }

        base.OnEnable();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

        this.ObserveEvent<InteractiveAction.InteractiveActionEvents, InteractiveAction.InteractiveActionEvent>(
            InteractiveAction.InteractiveActionEvents.OnClick, OnEvent);

        DefaultCamera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        Ppc = DefaultCamera?.GetComponent<PixelPerfectCamera>();
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

        _knocking = true;
        MachineryRef.Machinery.AddBasicMachine(KnockEffect.Knock().AsCoroutine()
            .Then(BreakRock().AsCoroutine()));
    }

    private IEnumerable<IEnumerable<Action>> BreakRock()
    {
        yield return TimeYields.WaitOneFrameX;
        Rock.enabled = false;
        BrokenRock.enabled = true;
        SmashAudio?.Play();
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, Message);
        _knocking = false;
        BrokenRockTrigger.Value = true;

        if (!string.IsNullOrWhiteSpace(TeleportToScene))
        {
            yield return LoadScene(TeleportToScene).AsCoroutine();
        }
    }

    private IEnumerable<IEnumerable<Action>> LoadScene(string teleportToScene)
    {
        Buttons.DisableAll();
        yield return ZoomIn().AsCoroutine();

        MachineryRef.Machinery.FinalizeWith(() => SceneManager.LoadScene(teleportToScene, LoadSceneMode.Single));
    }

    private IEnumerable<IEnumerable<Action>> ZoomIn()
    {
        Ppc.enabled = false;
        var cameraZoom = new LerpBuilder(val => DefaultCamera.orthographicSize = val, () => DefaultCamera.orthographicSize)
            .SetTarget(0.01f)
            .Over(4f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();

        var cameraMovement = DefaultCamera.transform.GetAccessor()
            .Towards(Rock.transform.position)
            .SetTarget(1f)
            .Over(4f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return cameraZoom.Combine(cameraMovement);
    }
}
