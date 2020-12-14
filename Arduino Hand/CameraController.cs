/* Filename:    CameraController.cs
 * Written by:  Travis McCormick
 * Course:      ECE 4960 Fall 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraDist = 5f;
    public float cameraSpeed = 2f;

    private float xzAngle;
    private float xyAngle;
    private Vector3 dragOrigin;
    private Vector3 previousPos;

    void Start() //Called before the first frame
    {
        dragOrigin = Vector3.zero;
        previousPos = Vector3.zero;

        //Set initial camera position
        xzAngle = Mathf.PI / 6;
        xyAngle = Mathf.PI / 4;
        transform.position = cameraDist * (new Vector3(Mathf.Cos(xzAngle), Mathf.Sin(xyAngle), Mathf.Sin(xzAngle)).normalized);
        transform.LookAt(Vector3.zero);
    }

    void Update() //Called every frame
    {
        cameraControl();
    }

    void cameraControl() //Pivot the camera around the origin. Controlled with left mouse click and dragging.
    {
        if (Input.GetMouseButtonDown(0)) //Initial left click
        {
            //Get mouse position at initial click
            dragOrigin = NormalizedMousePos();
            previousPos = Vector3.zero;
        }

        if (Input.GetMouseButton(0)) //Left click held down
        {
            //Calcualte how far the mouse moved
            Vector3 screenPos = NormalizedMousePos() - dragOrigin;
            xzAngle += cameraSpeed * (previousPos.x - screenPos.x);
            xyAngle += cameraSpeed * (previousPos.y - screenPos.y);

            //Limit angle of vetical camera view so camera does not flip upside-down
            if (xyAngle >= Mathf.PI / 2.0f)
            {
                xyAngle = Mathf.PI / 2.0f;
            }
            else if (xyAngle <= -Mathf.PI / 2.0f)
            {
                xyAngle = -Mathf.PI / 2.0f;
            }

            //Calcualte camera position
            Vector3 camPos = new Vector3(Mathf.Cos(xzAngle), Mathf.Sin(xyAngle), Mathf.Sin(xzAngle)).normalized;
            camPos *= cameraDist;
            transform.position = camPos;

            previousPos = screenPos;
        }

        //Aim camera at the origin
        transform.LookAt(Vector3.zero);
    }

    Vector3 NormalizedMousePos()
    {
        //Normalize mouse coords between [-1,1]
        Vector3 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;
        return mousePos;
    }
}
