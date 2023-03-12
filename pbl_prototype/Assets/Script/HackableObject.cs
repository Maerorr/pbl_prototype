using System.Collections;
using UnityEngine;

public abstract class HackableObject : MonoBehaviour
{
    protected UnityEngine.Color originalColor;
    protected Renderer renderer;
    
    [SerializeField]
    bool canBeHackedDirectly = true;
    
    protected void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }
    
    public void OnHover()
    {
        if (!canBeHackedDirectly) return;
        
        renderer.material.color = Color.green;
        StopAllCoroutines();
        StartCoroutine(Unhover());
    }

    private IEnumerator Unhover()
    {
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = originalColor;
    }
    
    public abstract bool CanHack();

    public abstract void OnHack();
}