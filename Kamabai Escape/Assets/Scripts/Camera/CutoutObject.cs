using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;
    RaycastHit hit;
    GameObject previousObject;
    bool weHitACutout = false;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);
        Debug.Log("Are cutout position " + cutoutPos);

        Vector3 offset = targetObject.position - transform.position;
       // RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        if(Physics.Raycast(transform.position, offset, out hit, offset.magnitude, wallMask)) 
        {
            if(hit.collider.gameObject.GetComponent<Renderer>() != null) 
            {
                Material material = hit.collider.gameObject.GetComponent<Renderer>().material;
                if (material != null)
                {
                    material.SetVector("_Cutout_Position", cutoutPos);
                    material.SetFloat("_Cutout_Size", 2f);
                    material.SetFloat("_Falloff_Size", 2f);
                    previousObject = hit.collider.gameObject;
                    weHitACutout = true;
                }
                else
                {
                    weHitACutout=false;
                }
            }
        }
        else if( weHitACutout== true) 
        {
            weHitACutout = false;
            previousObject.GetComponent<Renderer>().material.SetVector("_Cutout_Position", Vector2.zero);
            previousObject.GetComponent<Renderer>().material.SetFloat("_Cutout_Size", 0f);
            previousObject.GetComponent<Renderer>().material.SetFloat("_Falloff_Size", 0f);
        }
    }
}
