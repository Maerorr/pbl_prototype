using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


enum EnemyState
{
    Alive,
    Dead
}

public class Enemy : MonoBehaviour, IInteractable
{
    public Transform[] patrolPoints;
    private int current;
    public float speed = 10;
    public float visionRange = 10;
    public float visionAngle = 30;
    private bool isLookingAtTarget = false;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject interactionIndicator;
    
    private EnemyState state;
    
    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Alive;
        current = 0;
        interactionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints.Length == 0) return;

        if (state == EnemyState.Alive)
        {
            StartCoroutine(CheckForPlayer());
            Debug.DrawRay(transform.position,
                Quaternion.AngleAxis(visionAngle, Vector3.up) * transform.forward * visionRange, Color.white);
            Debug.DrawRay(transform.position,
                Quaternion.AngleAxis(-visionAngle, Vector3.up) * transform.forward * visionRange, Color.white);
            
            var currentPos = new Vector3(transform.position.x, 0, transform.position.z);
            var nextPatrolPoint = new Vector3(patrolPoints[current].position.x, 0, patrolPoints[current].position.z);
            
            //patrol between given points
            if (Vector3.Distance(currentPos, nextPatrolPoint) > 0.1f)
            {
                if (!isLookingAtTarget)
                {
                    LookAtCurrentPoint();
                }
                else
                {
                    MoveTowardsCurrentPoint();
                }
            }
            else
            {
                current = (current + 1) % patrolPoints.Length;
                isLookingAtTarget = false;
            }
        }

    }

    public void OnDeath()
    {
        state = EnemyState.Dead;
        gameObject.SetActive(false);
    }

    void OnAlerted()
    {
        Debug.Log("Spotted Player");
        GameOver.RestartGame();
    }

    IEnumerator CheckForPlayer()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 vectorToPlayer = playerPosition - transform.position;
        //check if enemy can see player
        if (Vector3.Distance(transform.position, playerPosition) <= visionRange &&
            Vector3.Angle(transform.forward, vectorToPlayer) <= visionAngle)
        {
            var ray = new Ray(transform.position, vectorToPlayer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                if (hit.transform.gameObject == player)
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.green);
                    OnAlerted();
                }
                else
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.red);
                }
            }
        }
        yield return new WaitForSeconds(.1f);
    }

    void LookAtCurrentPoint()
    {
        var currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        var enemyPosition = new Vector3(patrolPoints[current].position.x, 0, patrolPoints[current].position.z);

        // lerp the rotation
        var targetWithoutY = new Vector3(enemyPosition.x, currentPosition.y, enemyPosition.z);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(targetWithoutY - currentPosition), 0.01f);
        
        if (Vector3.Dot(transform.forward.normalized,(enemyPosition - currentPosition).normalized) > 0.99 )
        {
            isLookingAtTarget = true;
        }
    }
    
    void MoveTowardsCurrentPoint()
    {
        // Move towards target
        var targetWithoutY = new Vector3(patrolPoints[current].position.x, transform.position.y, patrolPoints[current].position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetWithoutY, speed * Time.deltaTime);
    }

    public void OnHover()
    {
        interactionIndicator.SetActive(true);
    }

    public void OnUnhover()
    {
        interactionIndicator.SetActive(false);
    }

    public bool CanInteract()
    {
        if (state == EnemyState.Alive)
        {
            return true;
        }
        return false;
    }

    public void Interact()
    {
        OnDeath();
    }
}
