using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Camera_Controls : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera => GetComponent<CinemachineVirtualCamera>();
    private PlayerInputHandler _playerInputHandler => FindObjectOfType<PlayerInputHandler>();
    private Player player => FindAnyObjectByType<Player>();
    [SerializeField] private float zoomSpeed = 10f;
    private bool zoomIn;
    private bool zoomOut;

    // Start is called before the first frame update
    void Start()
    {
        zoomIn = true;
        zoomOut = false;

    }

    // Update is called once per frame
    void Update()
    {
        if( _playerInputHandler != null )
        {
            
            ZoomInAndOut();
            //SwingZoomOut();
        }
        
    }

    private void SwingZoomOut()
    {
        CinemachineComponentBase componentBase = _virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (_playerInputHandler.isSwinging)
        {
          
            componentBase.GetComponent<CinemachineFramingTransposer>().m_CameraDistance = 75;
        }
        else
        {
            componentBase.GetComponent<CinemachineFramingTransposer>().m_CameraDistance = 60f;
        }
    }

    private void ZoomInAndOut()
    {
        CinemachineComponentBase componentBase = _virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (_playerInputHandler.CameraInput)
        {
            Debug.Log("Click for camera input");
            if ( zoomIn ) 
            {
                componentBase.GetComponent<CinemachineFramingTransposer>().m_CameraDistance += zoomSpeed;
                zoomOut = true;
                zoomIn = false;
                _playerInputHandler.UseCameraInput();
            }
            else if(zoomOut ) 
            {
                componentBase.GetComponent<CinemachineFramingTransposer>().m_CameraDistance -= zoomSpeed;
                zoomOut = false;
                zoomIn = true;
                _playerInputHandler.UseCameraInput();
            }
        }
    }

}
