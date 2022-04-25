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
using UnityEngine.SceneManagement;

public class ExitDoor : Interactive
{
    public ColorDefaults ColorDefaults;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    protected IEventPublisher<TextLog.TextLogEvents, string> TextLogPublisher;

    public AudioSource Music;
    public AudioSource OpenSound;
    public AudioSource ExitSound;
    public SpriteRenderer ScreenFlash;

    public PressableButtonGroup ButtonGroup;

    protected override void OnEnable()
    {
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

        MachineryRef.Machinery.AddBasicMachine(HandleExit());
    }

    IEnumerable<IEnumerable<Action>> HandleExit()
    {
        Music.Stop();
        ButtonGroup.DisableAll();
        OpenSound.Play();
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"<color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.FaeColor.Value)}>Fae</color> opens the door, finally finding the world they belong to.");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 2);

        ExitSound.Play();
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"No longer bound by darkness, the fairy travels across the door, bringing a sliver of happiness to whoever is on the other side.");

        ScreenFlash.enabled = true;
        ScreenFlash.color = new Color(1, 1, 1, 0);
        yield return ScreenFlash.GetAccessor()
            .Color.A
            .SetTarget(1f)
            .Over(5f)
            .UsingTimer(TimerRef.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 5);
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"Thanks for saving Fae!");

        yield return TimeYields.WaitSeconds(TimerRef.Timer, 3);

        MachineryRef.Machinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(Constants.Scenes.Credits, LoadSceneMode.Single);
        });
    }

}

