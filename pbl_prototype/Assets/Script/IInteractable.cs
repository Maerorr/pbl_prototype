using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void OnHover();
    public void OnUnhover();
    
    public bool CanInteract();
    public void Interact();
}
