using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<TetrisPiece> SpawnedPieces { get; set; }
    public List<TetrisPiecePlace> SpawnedPiecePlaces { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        SpawnedPieces = new List<TetrisPiece>();
        SpawnedPiecePlaces = new List<TetrisPiecePlace>();
    }
}
