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
    public Transform KeyHoleAnimation;

    public AudioSource KeyHoleSound;

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

        MachineryRef.Machinery.AddBasicMachine(LoadNextScene());

        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>)OnEvent);
    }

    private void OnDisable()
    {
        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>)OnEvent);
    }

    private IEnumerable<IEnumerable<Action>> LoadNextScene()
    {
        KeyHoleSound?.Play();
        KeyHoleAnimation.gameObject.SetActive(true);
        yield return ZoomIn().AsCoroutine().Combine(Fade().AsCoroutine());

        MachineryRef.Machinery.FinalizeWith(() =>
        {
            KeyHoleSound?.Stop();
            SceneManager.LoadScene(Constants.Scenes.ChestRoomBulletHell, LoadSceneMode.Single);
        });
    }

    private IEnumerable<IEnumerable<Action>> Fade()
    {
        yield return ChestDarkLight.GetAccessor()
            .Color.A
            .SetTarget(1)
            .Over(3f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();
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
            .Towards(KeyHole.transform.position)
            .SetTarget(1f)
            .Over(4f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return cameraZoom.Combine(cameraMovement);
    }
}
