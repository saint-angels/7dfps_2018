using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField] new Camera camera;


    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float fovZoomIn = 25f;
    [SerializeField] private float fovZoomOut = 70f;

    public float zoomProgress = 0;

    void Start ()
    {
		
	}
	
	void Update ()
    {

        if (Input.GetKey(KeyCode.Mouse1))
        {       
            //float oldRange = fovZoomOut - fovZoomIn;
            //float zoomProgress = 1 - (camera.fieldOfView - fovZoomIn) / oldRange; // 0 - 1

            zoomProgress += zoomSpeed * Time.deltaTime;
            //zoomProgress *= zoomProgress;

            zoomProgress = Mathf.Clamp01(zoomProgress);
            camera.fieldOfView = Mathf.Lerp(fovZoomOut, fovZoomIn, zoomProgress);
        }
        else
        {
            zoomProgress -= zoomSpeed * Time.deltaTime;
            //zoomProgress *= zoomProgress;

            zoomProgress = Mathf.Clamp01(zoomProgress);
            camera.fieldOfView = Mathf.Lerp(fovZoomOut, fovZoomIn, zoomProgress);
        }



	}
}
