using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiecePlace : MonoBehaviour
{
    [SerializeField] private PieceType pieceType;
    public PieceType PieceType => pieceType;

    [SerializeField] private Transform piecePlacePoint;
    public Transform PiecePlacePoint => piecePlacePoint;

    public bool IsPlaced { get; set; }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
