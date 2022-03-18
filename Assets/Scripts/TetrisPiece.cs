using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    [SerializeField] private PieceType pieceType;
    public PieceType PieceType => pieceType;

    public bool IsCollect { get; set; }
}
