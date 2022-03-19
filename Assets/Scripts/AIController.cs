using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private bool hasArrive = true;

    void Update()
    {
        if (GameManager.Instance.SpawnedPieces.Count > 0 && hasArrive)
        {
            agent.destination = GetRandomPieceTarget();
            hasArrive = false;
        }

        Debug.Log("Remaining Distance : " + agent.remainingDistance);

        if (agent.remainingDistance < 1f)
            hasArrive = true;
    }

    private Vector3 GetRandomPieceTarget()
    {
        return GameManager.Instance.SpawnedPieces[Random.Range(0, GameManager.Instance.SpawnedPieces.Count)].transform.position;
    }
}
