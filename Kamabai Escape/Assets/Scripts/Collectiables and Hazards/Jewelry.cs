using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewelry : MonoBehaviour
{

    [SerializeField] float scoreValue;
    private void OnTriggerEnter(Collider other)
    {
        var playerTag = other.gameObject.tag;
        if (playerTag == "Player" || playerTag == "Drill" || playerTag == "Vortex")
        {
            Debug.Log("Collected Jewelry");
            //TODO: insert Jewelry functionality here
            //AudioManager.instance.PickupSFX();
            //GameplayManager.Instance.OnLinkCollected();
            //ScoreManager.Instance.OnCollectibleCollected(ScoreManager.COLLECTIBLE_TYPE.JEWELRY);
            Destroy(this.gameObject);
        }
    }
}
