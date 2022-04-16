using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionBase : PressableButton
{
    protected Camera DefaultCamera;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultCamera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("Default"));
    }

    public bool IsClickingOn(Collider2D clickCollider)
    {
        var triggered = Input.actions[Constants.Actions.Click].triggered;
        return triggered && clickCollider.OverlapPoint(GetMousePosInWorld());
    }

    protected Vector3 GetMousePosInWorld()
    {
        var mousePosition = Input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = DefaultCamera.ScreenToWorldPoint(mousePosition);
        return contactPosition;
    }
}
