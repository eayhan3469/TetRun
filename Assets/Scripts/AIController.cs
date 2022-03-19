using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform stackTransform;
    [SerializeField] private Transform ejectTransform;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private int stackLimit;
    [SerializeField] private float speed;

    private List<TetrisPiece> collectedPieces = new List<TetrisPiece>();
    private bool hasArrive = true;
    private bool ejecting = false;
    private bool finishing = false;
    private float ejectWaitTime = 1.5f;
    private State currentState;

    private enum State
    {
        Collecting,
        Ejecting,
        Finishing
    }

    private void Start()
    {
        agent.speed = speed;
        GameManager.Instance.OnGameLose += OnGameFinished;
        GameManager.Instance.OnGameWin += OnGameFinished;
    }

    void Update()
    {
        if (GameManager.Instance.SpawnedPieces.Count > 0 && hasArrive && currentState == State.Collecting && GameManager.Instance.RivalPiecePlaces.Count > 0)
        {
            agent.destination = GetNearestPieceTarget();
            hasArrive = false;
        }

        if (agent.remainingDistance < 2f)
        {
            hasArrive = true;

            if (currentState == State.Ejecting)
            {
                ejectWaitTime -= Time.deltaTime;
                agent.isStopped = true;
            }

            if (ejectWaitTime <= 0f)
            {
                currentState = State.Collecting;
                agent.isStopped = false;
                ejectWaitTime = collectedPieces.Count > 5 ? 3f : 1.5f;
            }
        }

        if (GameManager.Instance.RivalPiecePlaces.Count == 0 && currentState != State.Finishing)
        {
            agent.destination = finishTransform.position;
            agent.isStopped = false;
            currentState = State.Finishing;
        }

        animator.SetBool("run", agent.velocity.magnitude > 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TetrisPiece"))
        {
            var tetrisPiece = other.GetComponent<TetrisPiece>();

            if (collectedPieces.Count >= stackLimit)
            {
                var removedPiece = collectedPieces[0];
                collectedPieces.Remove(removedPiece);

                removedPiece.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    Destroy(removedPiece);
                    ReorderCollectedPieces();
                });
            }

            if (!tetrisPiece.IsCollect)
            {
                tetrisPiece.IsCollect = true;
                collectedPieces.Add(tetrisPiece);
                GameManager.Instance.SpawnedPieces.Remove(tetrisPiece);

                if (GameManager.Instance.RivalPiecePlaces.Count > 0 && tetrisPiece.PieceType == GameManager.Instance.RivalPiecePlaces[0].PieceType)
                {
                    agent.destination = ejectTransform.position;
                    currentState = State.Ejecting;
                }

                other.transform.parent = stackTransform;
                other.transform.DOLocalJump(Vector3.up * (collectedPieces.Count - 1), 2f, 1, 0.75f);
                other.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), 0.5f);
            }
        }

        if (other.CompareTag("Finish"))
            GameManager.Instance.State = GameManager.GameState.Lose;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bridge") && GameManager.Instance.RivalPiecePlaces.Count > 0)
            PlacePiece(GameManager.Instance.RivalPiecePlaces[0]);
    }

    private Vector3 GetNearestPieceTarget()
    {
        var distance = 5000f;
        Vector3 nearestTargetPos = Vector3.zero;
        var firstPiecePlace = GameManager.Instance.RivalPiecePlaces[0];

        foreach (var piece in GameManager.Instance.SpawnedPieces)
        {
            if (piece.PieceType == firstPiecePlace.PieceType && Vector3.Distance(transform.position, firstPiecePlace.transform.position) < distance)
                nearestTargetPos = piece.transform.position;
        }

        if (nearestTargetPos == Vector3.zero)
            return GameManager.Instance.SpawnedPieces[Random.Range(0, GameManager.Instance.SpawnedPieces.Count)].transform.position;
        else
            return nearestTargetPos;
    }

    private void ReorderCollectedPieces()
    {
        foreach (var p in collectedPieces)
            p.transform.DOLocalMoveY(collectedPieces.IndexOf(p), 0.5f);
    }

    private void PlacePiece(TetrisPiecePlace place)
    {
        var piece = collectedPieces.Where(p => p.PieceType == place.PieceType).FirstOrDefault();

        if (piece == null || place.IsPlaced || place.HasPlayer)
            return;

        place.IsPlaced = true;
        collectedPieces.Remove(piece);
        ReorderCollectedPieces();
        piece.transform.parent = place.PiecePlacePoint;
        piece.transform.DOLocalMove(Vector3.zero, 0.5f);
        piece.transform.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() =>
        {
            place.Destroy();
        });
    }

    private void OnGameFinished()
    {
        agent.speed = 0;
    }
}
