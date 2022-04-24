using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressableButton : MonoBehaviour, IActivable, IDeactivable
{
    public Sprite DefaultSprite;
    public Sprite PressedSprite;
    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider;
    public PressableButtonGroup Group;
    public PrefabPool EffectPool;
    public Color EffectColor;
    public TimerScriptable TimerRef;

    public BasicMachineryScriptable MachineryRef;
    protected PlayerInput Input;
    protected Camera Camera;

    public AudioSource ActivationSound;
    public AudioSource DeactivationSound;

    protected virtual void OnEnable()
    {
        Input = Input != null ? Input : PlayerInput.GetPlayerByIndex(0);
        Camera = Camera.allCameras.FirstOrDefault(
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

    private IEnumerable<IEnumerable<Action>> HandleButtonPress()
    {
        while (isActiveAndEnabled)
        {
            var triggered = Input.actions[Constants.Actions.Click].triggered;
            if (triggered && Collider.OverlapPoint(GetMousePosInWorld()))
            {
                if (IsActive)
                {
                    Deactivate();
                    DeactivationSound?.Play();
                }
                else
                {
                    Group?.DeactivateAllExcept(this);
                    Activate();
                    ActivationSound?.Play();
                }
                
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        while (IsActive)
        {
            yield return SpriteRenderer.GetAccessor()
                .Color.ToColor(EffectColor)
                .SetTarget(1f)
                .Over(0.35f)
                .Easing(EasingYields.EasingFunction.CubicEaseIn)
                .UsingTimer(TimerRef.Timer)
                .BreakIf(() => !IsActive)
                .Build();

            yield return SpriteRenderer.GetAccessor()
                .Color.ToColor(Color.white)
                .SetTarget(1f)
                .Over(0.35f)
                .Easing(EasingYields.EasingFunction.CubicEaseIn)
                .BreakIf(() => !IsActive)
                .UsingTimer(TimerRef.Timer)
                .Build();
        }

        SpriteRenderer.color = Color.white;
    }

    private Vector3 GetMousePosInWorld()
    {
        var mousePosition = Input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = Camera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }


    public bool IsActive { get; private set; }
    public bool Activate()
    {
        MachineryRef.Machinery.AddBasicMachine(Blink());
        if (EffectPool.TryGetFromPool(out var obj) && obj is PressedEffectPoolable effect)
        {
            effect.Button = this;
            effect.transform.position = transform.position;
        }
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
