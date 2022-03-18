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

    [SerializeField] private Material colorReferenceMat;
    [SerializeField] private Transform cubesParent;

    public bool IsPlaced { get; set; }

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

        DOVirtual.DelayedCall(0.75f, () =>
        {
            gameObject.SetActive(false);
            GameManager.Instance.SpawnedPiecePlaces.Remove(this);

            if (GameManager.Instance.SpawnedPiecePlaces.Count > 0)
                GameManager.Instance.SpawnedPiecePlaces[0].PiecePlaceHolder.SetActive(true);
        });
    }
}
