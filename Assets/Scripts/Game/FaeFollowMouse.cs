using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class FaeFollowMouse : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public SpriteRenderer MouseSprite;

    private PlayerInput _input;
    private Camera _camera;

    public float MaxMoveSpeed = 1;
    public float SmoothTime = 0.3f;
    Vector2 _currentVelocity;

    private void OnEnable()
    {
        Cursor.visible = false;
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));

        MachineryRef.Machinery.AddBasicMachine(Follow());
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private IEnumerable<IEnumerable<Action>> Follow()
    {
        while (isActiveAndEnabled)
        {
            var pos = GetMousePosInWorld();

            MouseSprite.transform.position = new Vector3(pos.x, pos.y, 0);

            transform.position =
                Vector2.SmoothDamp(transform.position, pos, ref _currentVelocity, (1f / (float)TimerRef.Timer.Multiplier) * SmoothTime,
                    MaxMoveSpeed * (float)TimerRef.Timer.Multiplier);

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -14f, 14f),
                Mathf.Clamp(transform.position.y, -0.85f, 8.2f),
                0
            );

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private Vector3 GetMousePosInWorld()
    {
        var mousePosition = _input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = _camera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }
}
