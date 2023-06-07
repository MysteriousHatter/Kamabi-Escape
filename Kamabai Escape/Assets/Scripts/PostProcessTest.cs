using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PostProcessTest : MonoBehaviour
{
    public Shader shader;
    public LayerMask exclusionLayer; // Layer to exclude from grayscale conversion
    public Camera camera;

    private Material m_Material;

    void Start()
    {
        m_Material = new Material(shader);
        //m_Material.SetColor("_Color", colorTreatment); // Set the desired color treatment
        //camera.SetReplacementShader(shader, "");
        //var renderer = FindObjectsOfType<Renderer>();
        //for (int i = 0; i < renderer.Length; i++)
        //{
        //    for (int j = 0; j < renderer[i].sharedMaterials.Length; j++)
        //    {
        //        renderer[i].materials[j].SetFloat("_Blend", 1f);
        //    }
        //}
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, m_Material);
        //Camera.set

    }


}
