using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBox : MonoBehaviour, IHackable
{
    private bool IsHacked = false;
    private bool HasExploded = false;
    
    [SerializeField] private float explosionRadius = 5f;

    public void OnHack()
    {
        IsHacked = true;

        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        for (int i = 0; i < colliders.Length; i++)
        {
            var currentCollider = colliders[i];
            
            if (currentCollider.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath();
            }
        }
    }
    
}
