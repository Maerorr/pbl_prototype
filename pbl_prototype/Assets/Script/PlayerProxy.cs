using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProxy : MonoBehaviour, IInteractable
{
    [SerializeField]
    private HackableObject target;

    private Color originalColor;
    
    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void OnHover()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void OnUnhover()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

    public bool CanInteract()
    {
        return target.CanHack();
    }

    public void Interact()
    {
        target.OnHack();
    }
}
