using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBox : MonoBehaviour, IHackable
{
    private bool IsHacked = false;
    private bool HasExploded = false;

    public void OnHack()
    {
        IsHacked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsHacked && !HasExploded)
        {
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                if (enemy != null)
                {
                    //kill Enemy
                    HasExploded = true;
                }
            }
        }
    }
}
