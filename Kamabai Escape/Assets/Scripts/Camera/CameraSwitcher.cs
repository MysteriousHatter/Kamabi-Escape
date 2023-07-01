using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraSwitcher
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera ActiveCamera = null;

    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == ActiveCamera;
    }

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        ActiveCamera = camera;

        foreach(CinemachineVirtualCamera c in cameras) 
        {
            if(c != camera && c.Priority != 0)
            {
                c.Priority = 0;
            }
        }
    }


    public static void Register(CinemachineVirtualCamera cam)
    {
        cameras.Add(cam);
        Debug.Log("Camera registred: " + cam);
    }

    public static void Unregister(CinemachineVirtualCamera cam)
    {
        cameras.Remove(cam);
        Debug.Log("Camera Unregistred: " + cam);
    }
}
