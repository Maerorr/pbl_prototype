using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : HackableObject
{
    [SerializeField]
    private float Velocity = 1f;

    [SerializeField]
    private GameObject ObjectToMove;

    [SerializeField]
    private GameObject EndPoint;
    private Vector3 StartPoint;

    private float ElapsedTime = 0f;
    private float TimeToPoint = 0f;
    private bool IsAtEnd = false;
    private bool IsHacking = false;

    void Start()
    {
        base.Start();
        StartPoint = ObjectToMove.transform.position;
        float distance = Vector3.Distance(StartPoint, EndPoint.transform.position);
        ElapsedTime = 0f;
        TimeToPoint = distance / Velocity;
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
            ObjectToMove.transform.position = Vector3.Lerp(StartPoint, EndPoint.transform.position, elapsedPercentage);
        }
        else
        {
            ObjectToMove.transform.position = Vector3.Lerp(EndPoint.transform.position, StartPoint, elapsedPercentage);
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
        IsHacking = true;
    }
    
    public override bool CanHack()
    {
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(ObjectToMove.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}