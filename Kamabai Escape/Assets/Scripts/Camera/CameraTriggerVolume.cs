using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CameraTriggerVolume : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_Camera;
    [SerializeField] private Vector3 boxSize;

    BoxCollider m_Collider;
    Rigidbody rigidbody;



    private void Awake()
    {
       m_Collider = GetComponent<BoxCollider>();
       rigidbody = GetComponent<Rigidbody>();
       m_Collider.isTrigger = true;
       m_Collider.size = boxSize;


        rigidbody.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (CameraSwitcher.ActiveCamera != m_Camera) { CameraSwitcher.SwitchCamera(m_Camera); }
        }
    }
}
