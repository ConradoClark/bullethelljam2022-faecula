using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelpContext : MonoBehaviour
{
    public enum ContextCamera
    {
        Default,
        UI
    }

    public ContextCamera CameraType;
    public Collider2D Collider;
    public BasicMachineryScriptable MachineryRef;
    public string Text;

    private PlayerInput _input;
    private Camera _camera;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent> _textChangedPublisher;
    private IEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent> _textClearPublisher;

    void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(HandleHelpContext());
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer(Enum.GetName(typeof(ContextCamera), CameraType)));

        if (_camera == null) throw new Exception($"No suitable camera found! ({CameraType})");

        _textChangedPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextChangedEvent>();
        _textClearPublisher = this.RegisterAsEventPublisher<HelpText.HelpTextEvents, HelpText.TextEvent>();
    }

    IEnumerable<IEnumerable<Action>> HandleHelpContext()
    {
        while (isActiveAndEnabled)
        {
            var contactPosition = GetMousePosInWorld();

            if (Collider.OverlapPoint(contactPosition))
            {
                do
                {
                    _textChangedPublisher.PublishEvent(HelpText.HelpTextEvents.TextChanged, new HelpText.TextChangedEvent
                    {
                        Source = this,
                        Text = Text,
                    });
                    yield return TimeYields.WaitOneFrameX;
                    contactPosition = GetMousePosInWorld();
                } while (Collider.OverlapPoint(contactPosition));

                _textClearPublisher.PublishEvent(HelpText.HelpTextEvents.TextClear, new HelpText.TextEvent { Source = this });
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
}
