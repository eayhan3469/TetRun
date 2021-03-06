using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiecePlace : MonoBehaviour
{
    [SerializeField] private PieceType pieceType;
    public PieceType PieceType => pieceType;

    [SerializeField] private Transform piecePlacePoint;
    public Transform PiecePlacePoint => piecePlacePoint;

    [SerializeField] private GameObject piecePlaceHolder;
    public GameObject PiecePlaceHolder => piecePlaceHolder;

    [SerializeField] private BoxCollider collider;
    public BoxCollider Collider => collider;

    [SerializeField] private Material colorReferenceMat;
    [SerializeField] private Transform cubesParent;

    public bool IsPlaced { get; set; }
    public bool HasPlayer { get; set; }

    public void Destroy()
    {
        piecePlaceHolder.SetActive(false);

        for (int i = 0; i < cubesParent.childCount; i++)
        {
            var cube = cubesParent.GetChild(i);
            cube.GetComponent<MeshRenderer>().material.DOColor(colorReferenceMat.color, 0.25f);
            cube.DOScale(0f, 0.5f).SetEase(Ease.OutBounce);
        }

        for (int i = 0; i < piecePlacePoint.GetChild(1).childCount; i++)
        {
            var cube = piecePlacePoint.GetChild(1).GetChild(i);
            cube.DOScale(0f, 0.5f).SetEase(Ease.OutBounce);
        }

        if (HasPlayer)
        {
            if (GameManager.Instance.PlayersPiecePlaces.Count > 1)
                GameManager.Instance.PlayersPiecePlaces[1].PiecePlaceHolder.SetActive(true);

            DOVirtual.DelayedCall(0.75f, () =>
            {
                gameObject.SetActive(false);
                GameManager.Instance.PlayersPiecePlaces.Remove(this);

            });
        }
        else
        {
            if (GameManager.Instance.RivalPiecePlaces.Count > 1)
                GameManager.Instance.RivalPiecePlaces[1].PiecePlaceHolder.SetActive(true);

            DOVirtual.DelayedCall(0.75f, () =>
            {
                gameObject.SetActive(false);
                GameManager.Instance.RivalPiecePlaces.Remove(this);
            });
        }

    }
}
