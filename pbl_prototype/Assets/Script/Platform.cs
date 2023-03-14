using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : HackableObject
{
    [SerializeField]
    private GameObject ObjectToMove;

    [SerializeField]
    private GameObject EndPoint;
    private Vector3 StartPoint;
    private Quaternion StartRotation;

    private float ElapsedTime = 0f;
    [SerializeField]
    private float TimeToPoint = 1f;
    private bool IsAtEnd = false;
    private bool IsHacking = false;
    
    [SerializeField]
    bool JustRotate = false;

    void Start()
    {
        base.Start();
        StartPoint = ObjectToMove.transform.position;
        StartRotation = ObjectToMove.transform.rotation;
        float distance = Vector3.Distance(StartPoint, EndPoint.transform.position);
    }
    
    void FixedUpdate()
    {
        if (ObjectToMove == null || EndPoint == null || !IsHacking)
        {
            return;
        }
        ElapsedTime += Time.deltaTime;

        float elapsedPercentage = ElapsedTime / TimeToPoint;

        if (!IsAtEnd)
        {
            if (!JustRotate)
                ObjectToMove.transform.position = Vector3.Lerp(StartPoint, EndPoint.transform.position, elapsedPercentage);
            ObjectToMove.transform.rotation = Quaternion.Lerp(StartRotation, EndPoint.transform.rotation, elapsedPercentage);
        }
        else
        {
            if (!JustRotate)
                ObjectToMove.transform.position = Vector3.Lerp(EndPoint.transform.position, StartPoint, elapsedPercentage);
            ObjectToMove.transform.rotation = Quaternion.Lerp(EndPoint.transform.rotation, StartRotation, elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            IsHacking = false;
            IsAtEnd = !IsAtEnd;
            ElapsedTime = 0f;
        }

    }
    
    public override void OnHack()
    {
        if (!canBeHackedDirectly)
            return;
        IsHacking = true;
    }
    
    public override bool CanHack()
    {
        return true;
    }
}