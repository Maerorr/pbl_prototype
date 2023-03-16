using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBox : HackableObject
{
    private bool HasExploded = false;
    
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float distactionRadius = 10f;
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
        
        var collidersDistraction = Physics.OverlapSphere(transform.position, distactionRadius);
        for (int i = 0; i < collidersDistraction.Length; i++)
        {
            var currentCollider = collidersDistraction[i];
            
            if (currentCollider.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDistracted(this.gameObject);
            }
        }
        
        Invoke(nameof(DisableGameObject), explosionParticles.main.duration);
    }
    
    public override bool CanHack()
    {
        return !HasExploded;
    }

    private void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
    
}
