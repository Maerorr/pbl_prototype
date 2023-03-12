using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proxy : HackableObject
{
    [SerializeField]
    HackableObject target;

    void Start()
    {
        base.Start();
    }
    
    public override void OnHack()
    {
        target.OnHack();
    }
    
    public override bool CanHack()
    {
        return target.CanHack();
    }
}
