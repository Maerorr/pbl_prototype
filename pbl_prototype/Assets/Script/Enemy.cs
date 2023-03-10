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

public class Enemy : MonoBehaviour
{
    public Transform[] patrolPoints;
    private int current;
    public float speed = 10;
    public float visionRange = 10;
    public float visionAngle = 30;

    [SerializeField] private GameObject player;
    
    private EnemyState state;
    
    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Alive;
        current = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (state == EnemyState.Alive)
        {
            StartCoroutine(CheckForPlayer());
            Debug.DrawRay(transform.position,
                Quaternion.AngleAxis(visionAngle, Vector3.up) * transform.forward * visionRange, Color.white);
            Debug.DrawRay(transform.position,
                Quaternion.AngleAxis(-visionAngle, Vector3.up) * transform.forward * visionRange, Color.white);
            
            //patrol between given points
            if (transform.position != patrolPoints[current].position)
            {
                MoveTowardsCurrentPoint();
            }
            else
            {
                current = (current + 1) % patrolPoints.Length;
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
        //OnDeath();
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
                Debug.Log($"cokolwiek:{hit.transform.gameObject}");
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
        var currentPosition = transform.position;
        var enemyPosition = patrolPoints[current].position;

        // lerp the rotation
        var targetWithoutY = new Vector3(enemyPosition.x, currentPosition.y, enemyPosition.z);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(targetWithoutY - currentPosition), 0.01f);
    }
    
    void MoveTowardsCurrentPoint()
    {
        // Move towards target
        var targetWithoutY = new Vector3(patrolPoints[current].position.x, transform.position.y, patrolPoints[current].position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetWithoutY, speed * Time.deltaTime);
    }

}
