using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    private PlayerInput _input;
    private Camera _camera;
    void OnEnable()
    {
        Cursor.visible = false;
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        _camera = Camera.allCameras.FirstOrDefault(
            cam => cam.gameObject.layer == LayerMask.NameToLayer("UI"));
    }
    void OnDisable()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GetMousePosInWorld();
    }

    private Vector3 GetMousePosInWorld()
    {
        var mousePosition = _input.actions[Constants.Actions.MousePosition].ReadValue<Vector2>();
        var contactPosition = _camera.ScreenToWorldPoint(mousePosition);
        return new Vector3(contactPosition.x,contactPosition.y, 0);
    }
}
