using System;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    private Vector3 prevPos = Vector3.zero;
    private float cameraToGroundDstance = 0f;

    private float min = 20f;
    private float max = 60f;

    private float zoomTo;

    [SerializeField]
    private Transform groundTransform;

    private void Awake()
    {
        cameraToGroundDstance = transform.position.y - groundTransform.position.y;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = cameraToGroundDstance;
            prevPos = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = cameraToGroundDstance;
            var pos = Camera.main.ScreenToWorldPoint(mousePosition);
            pos.y = transform.position.y;
            prevPos.y = pos.y;

            transform.Translate(prevPos - pos, Space.World);
        }

        Zoom();
    }

    private void Zoom()
    {
        float mouseWheelScroll = Input.mouseScrollDelta.y;

        if (Math.Abs(mouseWheelScroll) < 0.00001f)
        {
            return;
        }

        if (mouseWheelScroll >= 1)
        {
            zoomTo = 5f;
        }
        else if (mouseWheelScroll <= -1)
        {
            zoomTo = -5f;
        }

        Camera.main.fieldOfView = Mathf.Clamp(zoomTo + Camera.main.fieldOfView, min, max);
        zoomTo = 0;
    }
}