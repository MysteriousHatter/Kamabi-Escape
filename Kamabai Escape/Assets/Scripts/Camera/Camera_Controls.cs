using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Camera_Controls : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera => GetComponent<CinemachineVirtualCamera>();
    private PlayerInputHandler _playerInputHandler => FindObjectOfType<PlayerInputHandler>();
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
        }
    }

    private void ZoomInAndOut()
    {
        if(_playerInputHandler.CameraInput)
        {
            if( zoomIn ) 
            {
                _virtualCamera.m_Lens.FieldOfView += zoomSpeed;
                zoomOut = true;
                zoomIn = false;
                _playerInputHandler.UseCameraInput();
            }
            else if(zoomOut ) 
            {
                _virtualCamera.m_Lens.FieldOfView -= zoomSpeed;
                zoomOut = false;
                zoomIn = true;
                _playerInputHandler.UseCameraInput();
            }
        }
    }
}
