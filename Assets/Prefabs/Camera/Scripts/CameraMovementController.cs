using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraMovementController : MonoBehaviour
{
    public bool lockRightClick;
    private bool resetingCameraPos;
    Dictionary <int, GameObject> cameraPoints = new Dictionary<int, GameObject>();

    int previousCameraPoint = 0;
    int activeCameraPoint = 0;
    public CinemachineFreeLook cameraLook;
    bool rotationActive = true;

    //SmoothDamp variables
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    private float cameraXPosSmoothVelocity = 0.0f;
    private float cameraYPosSmoothVelocity = 0.0f;


    void Start()
    {
        Debug.Log("start called");
        Debug.Log(cameraPoints.Count);
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CameraPoint"))
        {
            bool duplicate = false;
            foreach (KeyValuePair<int, GameObject> pair in cameraPoints)
            {
                if(!duplicate)
                    duplicate = (pair.Key == obj.GetComponent<CameraPoint>().orderInPointsStack);
            }
            if(duplicate){
                Debug.LogError("orderInPointsStack repeated in CameraPositions, obj: "+ obj.name);
                continue;
            }
            cameraPoints.Add(obj.GetComponent<CameraPoint>().orderInPointsStack, obj);
        }
    }
    private void FixedUpdate()
    {

        InterpolateCameraObjectFollowPosition();

        if(resetingCameraPos){
            ResetCameraPos();
            return;
        }

        if(!lockRightClick){
            cameraLook.m_YAxis.m_InputAxisName = "Mouse Y";
            cameraLook.m_XAxis.m_InputAxisName = "Mouse X";
            return;
        }

        rotationActive = Input.GetMouseButton(1);

        if (rotationActive)
        {
            cameraLook.m_YAxis.m_InputAxisName = "Mouse Y";
            cameraLook.m_XAxis.m_InputAxisName = "Mouse X";
            if(Cursor.visible){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        } else {
            /* Debug.Log("rotationActive false"); */

            if(!Cursor.visible){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            cameraLook.m_YAxis.m_InputAxisName = "";
            cameraLook.m_XAxis.m_InputAxisName = "";
            cameraLook.m_YAxis.m_InputAxisValue = 0;
            cameraLook.m_XAxis.m_InputAxisValue = 0;
        }
    }

    private void InterpolateCameraObjectFollowPosition()
    {
        cameraLook.Follow.position = Vector3.SmoothDamp(cameraLook.Follow.position, cameraPoints[activeCameraPoint].transform.position, ref velocity, smoothTime);

    }
    private void ResetCameraPos()
    {
        cameraLook.m_YAxis.Value = Mathf.SmoothDamp(
            cameraLook.m_YAxis.Value, 
            cameraPoints[activeCameraPoint].GetComponent<CameraPoint>().yAxisValue, 
            ref cameraYPosSmoothVelocity, 
            smoothTime
        );
        cameraLook.m_XAxis.Value = Mathf.SmoothDamp(
            cameraLook.m_XAxis.Value, 
            cameraPoints[activeCameraPoint].GetComponent<CameraPoint>().xAxisValue, 
            ref cameraXPosSmoothVelocity, 
            smoothTime
        );

        if(
            Mathf.Abs(cameraLook.m_YAxis.Value - cameraPoints[activeCameraPoint].GetComponent<CameraPoint>().yAxisValue) < .1f && 
            Mathf.Abs(cameraLook.m_XAxis.Value - cameraPoints[activeCameraPoint].GetComponent<CameraPoint>().xAxisValue) < .1f
            )
            resetingCameraPos = false;
    }

    public void SetCameraFocus(GameObject directionPoint)
    {
        resetingCameraPos = true;
        cameraLook.m_YAxis.m_InputAxisName = "";
        cameraLook.m_XAxis.m_InputAxisName = "";
        cameraLook.m_YAxis.m_InputAxisValue = 0;
        cameraLook.m_XAxis.m_InputAxisValue = 0;

        /* cameraLook.Follow.position = directionPoint.transform.position; */
        previousCameraPoint = activeCameraPoint;
        activeCameraPoint = directionPoint.GetComponent<CameraPoint>().orderInPointsStack;
    }
    public void SetNextCamera(){
        resetingCameraPos = true;
        cameraLook.m_YAxis.m_InputAxisName = "";
        cameraLook.m_XAxis.m_InputAxisName = "";
        cameraLook.m_YAxis.m_InputAxisValue = 0;
        cameraLook.m_XAxis.m_InputAxisValue = 0;

        /* cameraLook.Follow.position = directionPoint.transform.position; */
        Debug.Log(cameraPoints.Count);
        previousCameraPoint = activeCameraPoint;
        if((activeCameraPoint + 1) < cameraPoints.Count){
            activeCameraPoint++;
        }else {
            activeCameraPoint = 0;
        }


    }
    public void SetPrevCamera(){
        resetingCameraPos = true;
        cameraLook.m_YAxis.m_InputAxisName = "";
        cameraLook.m_XAxis.m_InputAxisName = "";
        cameraLook.m_YAxis.m_InputAxisValue = 0;
        cameraLook.m_XAxis.m_InputAxisValue = 0;

        /* cameraLook.Follow.position = directionPoint.transform.position; */
        previousCameraPoint = activeCameraPoint;

        if((activeCameraPoint - 1) >= 0){
            activeCameraPoint--;
        }else {
            activeCameraPoint = cameraPoints.Count - 1;
        }
    }

    //* UTIL METHODS
    public void SetCameraRotationStatus( bool isActive){
        rotationActive = isActive;
    }


}
