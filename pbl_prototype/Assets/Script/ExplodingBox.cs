using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBox : HackableObject
{
    private bool HasExploded = false;
    
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private ParticleSystem explosionParticles;

    public override void OnHack()
    {
        HasExploded = true;

        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        explosionParticles.Play();
        
        for (int i = 0; i < colliders.Length; i++)
        {
            var currentCollider = colliders[i];
            
            if (currentCollider.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath();
            }
        }
    }
    
    public override bool CanHack()
    {
        return !HasExploded;
    }
    
}
