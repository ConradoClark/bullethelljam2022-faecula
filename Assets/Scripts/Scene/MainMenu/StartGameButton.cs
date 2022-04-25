using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public Collider2D Collider;
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    public SpriteRenderer ScreenFlash;
    public AudioSource StartGameSound;

    private PlayerInput _input;
    private Camera _camera;

    void OnEnable()
    {
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        MachineryRef.Machinery.AddBasicMachine(HandleStartGame());
    }

    Vector3 GetMousePosInWorld()
    {
        var mousePosition = _input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = _camera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }

    IEnumerable<IEnumerable<Action>> HandleStartGame()
    {
        var started = false;
        while (!started)
        {
            var click = _input.actions[Constants.Actions.Click];
            var contactPosition = GetMousePosInWorld();
            if (!click.WasPerformedThisFrame() || !Collider.OverlapPoint(contactPosition))
            {
                yield return TimeYields.WaitOneFrameX;
                continue;
            }

            started = true;
            StartGameSound.Play();
            ScreenFlash.enabled = true;
            ScreenFlash.color = new Color(1, 1, 1, 0);
            yield return ScreenFlash.GetAccessor()
                .Color.A
                .SetTarget(1f)
                .Over(0.2f)
                .UsingTimer(TimerRef.Timer)
                .Easing(EasingYields.EasingFunction.CubicEaseOut)
                .Build();

            yield return TimeYields.WaitSeconds(TimerRef.Timer, 1);

            MachineryRef.Machinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene(Constants.Scenes.ChestRoom, LoadSceneMode.Single);
            });
        }
    }
}

