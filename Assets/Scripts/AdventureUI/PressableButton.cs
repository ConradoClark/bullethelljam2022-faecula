using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressableButton : MonoBehaviour, IActivable, IDeactivable
{
    public Sprite DefaultSprite;
    public Sprite PressedSprite;
    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider;
    public PressableButtonGroup Group;

    public BasicMachineryScriptable MachineryRef;
    private PlayerInput _input;
    private Camera _camera;

    protected virtual void OnEnable()
    {
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("UI"));

        IsActive = false;
        Group?.AddToGroup(this);
        SpriteRenderer.sprite = DefaultSprite;
        MachineryRef.Machinery.AddBasicMachine(HandleButtonPress());
        
    }

    protected virtual void OnDisable()
    {
        Group?.RemoveFromGroup(this);
    }

    IEnumerable<IEnumerable<Action>> HandleButtonPress()
    {
        while (isActiveAndEnabled)
        {
            var triggered = _input.actions[Constants.Actions.Click].triggered;
            if (triggered && Collider.OverlapPoint(GetMousePosInWorld()))
            {
                if (IsActive)
                {
                    Deactivate();
                }
                else
                {
                    Group?.DeactivateAllExcept(this);
                    Activate();
                }
                
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
    Vector3 GetMousePosInWorld()
    {
        var mousePosition = _input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = _camera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }

    public bool IsActive { get; private set; }
    public bool Activate()
    {
        IsActive = true;
        SpriteRenderer.sprite = PressedSprite;

        return true;
    }

    public bool Deactivate()
    {
        IsActive = false;
        SpriteRenderer.sprite = DefaultSprite;

        return true;
    }
}
