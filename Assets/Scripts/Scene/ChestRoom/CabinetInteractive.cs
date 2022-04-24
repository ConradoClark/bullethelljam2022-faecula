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
using UnityEngine.U2D;


public class CabinetInteractive : Interactive
{
    public ColorDefaults ColorDefaults;
    public Animator CabinetAnimator;
    public string OpenAnimName;
    public string CloseAnimName;
    public PressableButtonGroup ButtonsGroup;
    public InteractiveAction InteractAction;

    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public Collider2D ClosedCollider2D;
    public Collider2D OpenedCollider2D;

    public AudioSource OpenCabinetSound;
    public AudioSource CloseCabinetSound;

    public SpriteRenderer SpriteRenderer;
    public Sprite OpenedSprite;

    public GlobalTrigger OpenCabinet;

    public Transform[] Contents;

    private IEventPublisher<TextLog.TextLogEvents, string> _textLogPublisher;
    private bool _open;

    protected override void OnEnable()
    {
        if (OpenCabinet.Value)
        {
            CabinetAnimator.Play(OpenAnimName);
            SpriteRenderer.sprite = OpenedSprite;
            _open = true;
            foreach (var content in Contents)
            {
                content.gameObject.SetActive(true);
            }
        }

        base.OnEnable();
        _textLogPublisher = this.RegisterAsEventPublisher<TextLog.TextLogEvents, string>();

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

        MachineryRef.Machinery.AddBasicMachine(Toggle());
    }

    private IEnumerable<IEnumerable<Action>> Toggle()
    {
        var buttons = ButtonsGroup.DisableAll();

        _open = !_open;
        if (_open)
        {
            yield return Open().AsCoroutine();
        }
        else
        {
            yield return Close().AsCoroutine();
        }

        yield return TimeYields.WaitOneFrameX;

        ButtonsGroup.Enable(buttons);

        yield return TimeYields.WaitOneFrameX;

        InteractAction.Activate();
    }

    private IEnumerable<IEnumerable<Action>> Open()
    {
        OpenCabinet.Value = true;
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"You open the <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Objects.Value)}>cabinet</color> in front of you.");
        OpenCabinetSound.Play();
        
        CabinetAnimator.Play(OpenAnimName);
        yield return TimeYields.WaitOneFrameX;
        yield return TimeYields.WaitOneFrameX;

        // wait for animation to finish
        while (CabinetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || CabinetAnimator.IsInTransition(0))
        {
            Debug.Log("waiting anim");
            yield return TimeYields.WaitOneFrameX;
        }

        foreach (var content in Contents)
        {
            content.gameObject.SetActive(true);
        }

        Colliders = new[] { OpenedCollider2D };
        Debug.Log("finished opening");
    }

    private IEnumerable<IEnumerable<Action>> Close()
    {
        OpenCabinet.Value = false;
        _textLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"You close the <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Objects.Value)}>cabinet</color> in front of you.");
        CloseCabinetSound.Play();
        CabinetAnimator.Play(CloseAnimName);
        yield return TimeYields.WaitOneFrameX;
        yield return TimeYields.WaitOneFrameX;

        foreach (var content in Contents)
        {
            content.gameObject.SetActive(false);
        }

        // wait for animation to finish
        while (CabinetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || CabinetAnimator.IsInTransition(0))
        {
            yield return TimeYields.WaitOneFrameX;
        }

        Colliders = new[] { ClosedCollider2D };
        Debug.Log("finished closing");
    }
}
