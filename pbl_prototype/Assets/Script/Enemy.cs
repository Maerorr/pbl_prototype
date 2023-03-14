using System.Collections;
using UnityEngine;

enum EnemyState
{
    Patrolling,
    Distracted,
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
    private Vector3 distraction;
    public bool continuePatrollingAfterDistraction = true;
    
    private bool seenPlayer = false;
    private bool prevSawPlayer = false;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject interactionIndicator;
    
    private EnemyState state;
    
    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Patrolling;
        current = 0;
        interactionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints.Length == 0) return;

        if (state == EnemyState.Patrolling)
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
                    LookAtPoint(patrolPoints[current].position);
                }
                else
                {
                    MoveTowardsPoint(patrolPoints[current].position);
                }
            }
            else
            {
                current = (current + 1) % patrolPoints.Length;
                isLookingAtTarget = false;
            }
        }
        else if (state == EnemyState.Distracted)
        {
            if (!isLookingAtTarget)
            {
                LookAtPoint(distraction);
            }
            else if (Vector3.Distance(this.transform.position, distraction) > 2.0f)
            {
                MoveTowardsPoint(distraction);
            }
            else if (continuePatrollingAfterDistraction)
            {
                StartCoroutine(ReturnToPatrolling());
            }
        }
    }

    public void OnDeath()
    {
        state = EnemyState.Dead;
        gameObject.SetActive(false);
    }

    public void OnDistracted(GameObject newDistraction)
    {
        Vector3 vectorToDistraction = newDistraction.transform.position - transform.position;
        var ray = new Ray(transform.position, vectorToDistraction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, visionRange))
        {
            Debug.Log(hit.transform.gameObject);
            if (hit.transform.gameObject == newDistraction)
            {
                state = EnemyState.Distracted;
                isLookingAtTarget = false;
                this.distraction = newDistraction.transform.position;
            }
        }
    }

    IEnumerator CheckForPlayer()
    {
        seenPlayer = false;
        
        Vector3 playerPosition = player.transform.position;
        Vector3 vectorToPlayer = playerPosition - transform.position;
        float differenceInHeight = transform.position.y - playerPosition.y;
        playerPosition.y = transform.position.y;
        Vector3 vectorToPlayerNoY = playerPosition - transform.position;
        
        //check if enemy can see player
        if (Vector3.Distance(transform.position, playerPosition) <= visionRange &&
            Vector3.Angle(transform.forward, vectorToPlayerNoY) <= visionAngle)
        {
            var ray = new Ray(transform.position, vectorToPlayer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange + differenceInHeight * 5f))
            {
                Debug.Log(hit.transform.gameObject);
                if (hit.transform.gameObject == player)
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.green);
                    Player.AddDetection(Time.deltaTime);
                    Player.SetEnemyDetection(true);
                    seenPlayer = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.red);
                }
            }
        }
        
        if (!seenPlayer && prevSawPlayer)
        {
            Player.SetEnemyDetection(false);
        }
        
        if (seenPlayer)
        {
            prevSawPlayer = true;
        }
        else
        {
            prevSawPlayer = false;
        }
        
        yield return new WaitForSeconds(.1f);
    }

    IEnumerator ReturnToPatrolling()
    {
        yield return new WaitForSeconds(2.0f);
        state = EnemyState.Patrolling;
        isLookingAtTarget = false;
    }

    void LookAtPoint(Vector3 point)
    {
        var currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        var enemyPosition = new Vector3(point.x, 0, point.z);

        // lerp the rotation
        var targetWithoutY = new Vector3(enemyPosition.x, currentPosition.y, enemyPosition.z);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(targetWithoutY - currentPosition), 0.01f);
        
        if (Vector3.Dot(transform.forward.normalized,(enemyPosition - currentPosition).normalized) > 0.99 )
        {
            isLookingAtTarget = true;
        }
    }
    
    void MoveTowardsPoint(Vector3 point)
    {
        // Move towards target
        var targetWithoutY = new Vector3(point.x, transform.position.y, point.z);
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
        if (state == EnemyState.Patrolling)
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
