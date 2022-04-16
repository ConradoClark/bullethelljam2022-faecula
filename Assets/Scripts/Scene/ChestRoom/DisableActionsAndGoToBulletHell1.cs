using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class DisableActionsAndGoToBulletHell1 : MonoBehaviour
{
    public Interactive KeyHole;
    public PressableButtonGroup Buttons;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer ChestDarkLight;

    protected Camera DefaultCamera;
    protected PixelPerfectCamera Ppc;

    private void OnEnable()
    {
        DefaultCamera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        Ppc = DefaultCamera?.GetComponent<PixelPerfectCamera>();
        this.ObserveEvent<Interactive.InteractiveEvents, Interactive>(Interactive.InteractiveEvents.OnInteractiveClicked, OnEvent);
    }

    private void OnEvent(Interactive obj)
    {
        if (obj != KeyHole) return;
        Buttons.DisableAll();

        MachineryRef.Machinery.AddBasicMachine(ZoomIn());
        MachineryRef.Machinery.AddBasicMachine(Fade());

        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>)OnEvent);
    }

    private void OnDisable()
    {
        if (Ppc!=null) Ppc.enabled = true;
        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>)OnEvent);
    }

    private IEnumerable<IEnumerable<Action>> LoadNextScene()
    {
        yield return ZoomIn().AsCoroutine().Combine(Fade().AsCoroutine());
        SceneManager.LoadScene("ChestRoomBulletHell", LoadSceneMode.Single);
    }

    private IEnumerable<IEnumerable<Action>> Fade()
    {
        yield return ChestDarkLight.GetAccessor()
            .Color.A
            .SetTarget(1)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> ZoomIn()
    {
        Ppc.enabled = false;
        yield return new LerpBuilder(val => DefaultCamera.orthographicSize = val, () => DefaultCamera.orthographicSize)
            .SetTarget(0)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
    }
}
