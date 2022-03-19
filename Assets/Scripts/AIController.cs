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
    [SerializeField] private Animator animator;
    [SerializeField] private int stackLimit;
    [SerializeField] private float speed;

    private List<TetrisPiece> collectedPieces = new List<TetrisPiece>();
    private bool hasArrive = true;
    private bool isGoingEject = false;

    private void Start()
    {
        agent.speed = speed;
    }

    void Update()
    {
        if (GameManager.Instance.SpawnedPieces.Count > 0 && hasArrive)
        {
            agent.destination = GetRandomPieceTarget();
            hasArrive = false;
        }

        if (agent.remainingDistance < 1f)
        {
            hasArrive = true;
            isGoingEject = false;
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

                if (tetrisPiece.PieceType == GameManager.Instance.RivalPiecePlaces[0].PieceType)
                {
                    agent.destination = GameManager.Instance.RivalPiecePlaces[0].transform.position;
                    isGoingEject = true;
                }

                other.transform.parent = stackTransform;
                other.transform.DOLocalJump(Vector3.up * (collectedPieces.Count - 1), 2f, 1, 0.75f);
                other.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), 0.5f);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bridge"))
            PlacePiece(GameManager.Instance.RivalPiecePlaces[0]);
    }

    private Vector3 GetRandomPieceTarget()
    {
        return GameManager.Instance.SpawnedPieces[Random.Range(0, GameManager.Instance.SpawnedPieces.Count)].transform.position;
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
}
