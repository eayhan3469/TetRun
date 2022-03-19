using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
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
        if (joystick != null)
        {
            var ang = Mathf.Atan2(joystick.Horizontal, joystick.Vertical);

            if (ang < 0)
                ang += Mathf.PI * 2f;

            ang += Mathf.PI / 6f;

            var axisX = Mathf.Cos(ang);
            var axisY = Mathf.Sin(ang);

            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                rigidBody.velocity = new Vector3(axisY * moveForce, rigidBody.velocity.y, axisX * moveForce);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z)),
                    Time.deltaTime * 100f
                );

                animator.SetBool("run", true);
            }
            else
            {
                rigidBody.velocity = Vector3.zero;
            }

            animator.SetBool("run", rigidBody.velocity.magnitude > 0.2f);
        }

        ClampPosition();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bridge"))
            PlacePiece(GameManager.Instance.PlayersPiecePlaces[0]);
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

    private void ReorderCollectedPieces()
    {
        foreach (var p in collectedPieces)
            p.transform.DOLocalMoveY(collectedPieces.IndexOf(p), 0.5f);
    }

    private void PlacePiece(TetrisPiecePlace place)
    {
        var piece = collectedPieces.Where(p => p.PieceType == place.PieceType).FirstOrDefault();

        if (piece == null || place.IsPlaced || !place.HasPlayer)
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
