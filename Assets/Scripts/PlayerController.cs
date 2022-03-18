using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Transform stackTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private int stackLimit;
    [SerializeField] private float moveForce;
    [SerializeField] private float clampValueX;
    [SerializeField] private float clampValueZ;

    private List<TetrisPiece> collectedPieces = new List<TetrisPiece>();
    private RaycastHit hit;

    void Update()
    {
        rigidbody.velocity = new Vector3(joystick.Horizontal * moveForce, rigidbody.velocity.y, joystick.Vertical * moveForce);

        if (rigidbody.velocity.magnitude > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z)),
                Time.deltaTime * 100f
            );

            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(transform.forward + (Vector3.up * 2f)), out hit, 5f))
        {
            if (hit.transform.CompareTag("TetrisPiecePlace"))
            {
                PlacePiece(hit.transform.GetComponent<TetrisPiecePlace>());
            }
        }

        ClampPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TetrisPiece"))
        {
            var tetrisPiece = other.GetComponent<TetrisPiece>();

            if (collectedPieces.Count >= stackLimit)
                RemovePieceFromStack(collectedPieces[0]);

            if (!tetrisPiece.IsCollect)
            {
                tetrisPiece.IsCollect = true;
                collectedPieces.Add(tetrisPiece);
                GameManager.Instance.SpawnedPieces.Remove(tetrisPiece);

                other.transform.parent = stackTransform;
                other.transform.DOLocalJump(Vector3.up * (collectedPieces.Count - 1), 2f, 1, 0.75f);
                other.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90f, 0f, 0f)), 0.5f);
            }
        }
    }

    private void ClampPosition()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -clampValueX, clampValueX), transform.position.y, Mathf.Clamp(transform.position.z, -clampValueZ, clampValueZ));
    }

    private void RemovePieceFromStack(TetrisPiece piece)
    {
        collectedPieces.Remove(piece);

        foreach (var p in collectedPieces)
        {
            piece.transform.DOLocalMoveY(collectedPieces.IndexOf(p), 0.5f);
        }
    }

    private void PlacePiece(TetrisPiecePlace place)
    {
        var piece = collectedPieces.Where(p => p.PieceType == place.PieceType).FirstOrDefault();

        if (piece == null || place.IsPlaced)
            return;

        place.IsPlaced = true;
        RemovePieceFromStack(piece);
        piece.transform.parent = place.PiecePlacePoint;
        piece.transform.DOLocalMove(Vector3.zero, 0.5f);
        piece.transform.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() => place.Destroy());
    }
}
