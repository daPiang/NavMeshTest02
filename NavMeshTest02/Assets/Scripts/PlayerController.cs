using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private AStar aStar;
    private List<Node> currentPath;
    private int currentPathIndex;

    private void Start()
    {
        aStar = GetComponent<AStar>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the player clicked on a valid location.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.GetComponent<Node>().walkable)
                {
                    // Get the clicked position and find a path to it.
                    Vector3 targetPosition = hit.point;
                    FindPathToTarget(targetPosition);
                }
            }
        }

        // Move the player along the path if there is one.
        MoveAlongPath();
    }

    private void FindPathToTarget(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        currentPath = aStar.FindPath(startPosition, targetPosition);
        currentPathIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (currentPath != null && currentPathIndex < currentPath.Count)
        {
            Node nextNode = currentPath[currentPathIndex];
            Vector3 targetPosition = nextNode.worldPosition;

            // Calculate the distance between the player and the next node.
            float distanceToNode = Vector3.Distance(transform.position, targetPosition);

            // Move the player towards the next node.
            float moveSpeed = 5f;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the player has reached the next node with a small threshold.
            if (distanceToNode < 0.1f)
            {
                // Center the player precisely on the node.
                transform.position = targetPosition;

                currentPathIndex++;

                // If the player reached the end of the path, clear it.
                if (currentPathIndex >= currentPath.Count)
                {
                    currentPath = null;
                }
            }
            Debug.DrawLine(transform.position, targetPosition, Color.red); // Draw a red line between player and target node.
            Debug.Log("Player Position: " + transform.position);
            Debug.Log("Target Node Position: " + targetPosition);
        }
    }
}
