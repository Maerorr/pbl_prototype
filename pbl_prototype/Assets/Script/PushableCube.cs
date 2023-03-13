using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableCube : MonoBehaviour, IInteractable
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform player;
    
    public void OnHover()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void OnUnhover()
    {
        GetComponent<Renderer>().material.color = Color.gray;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        // Add force from player to cube
        rb.AddForce(player.transform.forward * 1000);
    }
}
