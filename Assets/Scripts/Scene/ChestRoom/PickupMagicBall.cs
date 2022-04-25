using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class PickupMagicBall : PickUpObject
{
    public PressableButtonGroup ButtonGroup;

    public AudioSource MagicSigilSound;

    protected Camera DefaultCamera;
    protected PixelPerfectCamera Ppc;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultCamera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        Ppc = DefaultCamera?.GetComponent<PixelPerfectCamera>();
    }

    protected override IEnumerable<IEnumerable<Action>> Pickup()
    {
        ButtonGroup.DisableAll();
        yield return base.Pickup().AsCoroutine();
        MagicSigilSound?.Play();
        TextLogPublisher.PublishEvent(TextLog.TextLogEvents.OnLogEntry, $"The <color=#{ColorUtility.ToHtmlStringRGB(ColorDefaults.Sigils.Value)}>magic sigil</color> is activated!");

        yield return ZoomIn().AsCoroutine();
        MachineryRef.Machinery.FinalizeWith(() =>
        {
            MagicSigilSound?.Stop();
            SceneManager.LoadScene(Constants.Scenes.MagicBallBulletHell, LoadSceneMode.Single);
        });
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
            .Towards(transform.position)
            .SetTarget(1f)
            .Over(4f)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .UsingTimer(TimerRef.Timer)
            .Build();

        yield return cameraZoom.Combine(cameraMovement);
    }
}

