using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyGameObjectsWithTag : MonoBehaviour
{
    public LayerMask greyScaleLayer;
    public Material[] newMaterial;
    public Material oldMaterial;
    public Material candyMaterial;
    private PlayerInputHandler _playerInputHandler => FindObjectOfType<PlayerInputHandler>();

    void Start()
    {
        ChangBack();
    }

    private void Update()
    {
        if (this.gameObject.GetComponent<PostProcessTest>().enabled)
        {
            Debug.Log("The current material");
            ChangeMaterials();
        }
        else
        {
            ChangBack();
        }
    }

    private void ChangBack()
    {
        // Find all GameObjects with the layers included in the targetLayer
        GameObject[] gameObjects = FindGameObjectsWithLayerMask(greyScaleLayer);

        // Apply modifications to each GameObject
        foreach (GameObject gameObject in gameObjects)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (gameObject.name == "Bonus_Candy") { renderer.material = candyMaterial; }
                // Modify the material of the GameObject
                else { renderer.material = oldMaterial; }
            }
        }
    }

    void ChangeMaterials()
    {
        // Find all GameObjects with the layers included in the targetLayer
        GameObject[] gameObjects = FindGameObjectsWithLayerMask(greyScaleLayer);

        // Apply modifications to each GameObject
        foreach (GameObject gameObject in gameObjects)
        {
            // Get the tag of the GameObject
            string tag = gameObject.tag;

            // Use switch statements to assign materials based on tags
            switch (tag)
            {
                case "Grab-Platform":
                    gameObject.GetComponent<Renderer>().material = newMaterial[0];
                    break;
                case "Collectiable":
                    gameObject.GetComponent<Renderer>().material = newMaterial[0];
                    break;
                case "Platform":
                    gameObject.GetComponent<Renderer>().material = newMaterial[1];
                    break;
                case "Swing-Reel":
                    gameObject.GetComponent<Renderer>().material = newMaterial[2];
                    break;
                // Add more cases for additional tags and materials if needed
                default:
                    // Default case, in case no matching tag is found
                    // You can choose to skip or handle these GameObjects differently
                    break;
            }

            gameObject.GetComponent<Renderer>().material.SetFloat("_Blend", 1f);
            gameObject.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.831f);
            gameObject.GetComponent<Renderer>().material.SetFloat("_Metallic", 0.543f);
        }


    }

    GameObject[] FindGameObjectsWithLayerMask(LayerMask layerMask)
    {
        // Get all the GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Create a list to store the GameObjects with the layers included in the layerMask
        System.Collections.Generic.List<GameObject> objectsWithLayer = new System.Collections.Generic.List<GameObject>();

        // Iterate through each GameObject
        foreach (GameObject gameObject in allObjects)
        {
            // Check if the GameObject's layer is included in the layerMask
            if ((layerMask.value & (1 << gameObject.layer)) > 0)
            {
                // Add the GameObject to the list
                objectsWithLayer.Add(gameObject);
            }
        }

        // Convert the list to an array
        GameObject[] objectsArray = objectsWithLayer.ToArray();

        return objectsArray;
    }
}
