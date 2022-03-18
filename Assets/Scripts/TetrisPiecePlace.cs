using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiecePlace : MonoBehaviour
{
    [SerializeField] private PieceType pieceType;
    public PieceType PieceType => pieceType;
}
