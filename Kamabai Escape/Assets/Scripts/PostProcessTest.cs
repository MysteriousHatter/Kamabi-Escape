using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessTest : MonoBehaviour
{
    public Shader shader;
    public LayerMask exclusionLayer; // Layer to exclude from grayscale conversion

    private Material m_Material;

    void Start()
    {
        m_Material = new Material(shader);
        //m_Material.SetColor("_Color", colorTreatment); // Set the desired color treatment
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, m_Material);
    }
}
