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
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, m_Material);


    }


}
