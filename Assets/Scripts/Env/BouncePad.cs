using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField,Tag] private string PlayerTag;
    [SerializeField] private float BounceDuration = 1;
    [SerializeField] private float BounceForce = 20;
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(PlayerTag))
        {
            var player = collider.GetComponent<PlayerMovement>();
            player.AddForce(transform.up,BounceForce,BounceDuration);
        }
    }
}
