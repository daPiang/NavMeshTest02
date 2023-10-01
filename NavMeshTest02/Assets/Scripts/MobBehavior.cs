using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobBehavior : MonoBehaviour
{
    public Transform target; // The player to queue behind
    private NavMeshAgent agent;
    public float queueDistance = 2f; // Distance to maintain from the player to form a queue
    public float startQueueDistance = 5f; // Distance at which the AI will start forming a queue
    public float queueSpacing = 2f; // Spacing between AI characters in the queue
    private bool isQueueing = false;

    void Start()
    {
        target = FindObjectOfType<NavmeshPlayerController>().transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (IsPlayerCloseEnough())
        {
            isQueueing = true;
            QueueBehindTheTarget();
        }
        // else
        // {
        //     isQueueing = false;
        // }
    }

    private bool IsPlayerCloseEnough()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        return distanceToPlayer <= startQueueDistance;
    }

    private void QueueBehindTheTarget()
    {
        if (target != null && isQueueing)
        {
            // Calculate the position behind the player
            Vector3 queuePosition = target.position - target.forward * queueDistance;

            // Use NavMesh to find a valid position to queue
            NavMesh.SamplePosition(queuePosition, out NavMeshHit navHit, 10, NavMesh.AllAreas);

            // Set the AI's destination to the queue position with spacing
            int queueIndex = FindQueueIndex();
            Vector3 finalQueuePosition = queuePosition + target.right * (queueSpacing * queueIndex);
            agent.SetDestination(finalQueuePosition);
        }
    }

    private int FindQueueIndex()
    {
        // Raycast forward to find other AI characters in the queue
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, queueDistance))
        {
            MobBehavior otherQueueBehavior = hit.transform.GetComponent<MobBehavior>();
            if (otherQueueBehavior != null && otherQueueBehavior.isQueueing)
            {
                // Calculate the index based on the spacing
                int queueIndex = Mathf.FloorToInt(Vector3.Distance(transform.position, otherQueueBehavior.transform.position) / queueSpacing);
                return queueIndex;
            }
        }
        return 0;
    }
}
