using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableDoor : HackableObject
{
    private Animator animator;
    private bool isOpen = false;
    private int hackCooldown = 0;
    void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hackCooldown > 0) hackCooldown--;
    }

    public override bool CanHack()
    {
        return true;
    }

    public override void OnHack()
    {
        if (hackCooldown > 0) return;
        isOpen = !isOpen;
        animator.SetBool("open", isOpen);
        hackCooldown = 60;
    }

}
