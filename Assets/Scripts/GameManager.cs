using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<TetrisPiece> SpawnedPieces { get; set; }
    public List<TetrisPiecePlace> PlayersPiecePlaces { get; set; }
    public List<TetrisPiecePlace> RivalPiecePlaces { get; set; }

    public GameState State { get; set; }

    public enum GameState
    {
        Idle,
        Playing,
        Win,
        Lose
    }

    public event UnityAction OnGameWin;
    public event UnityAction OnGameLose;

    private bool isGameActive = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        SpawnedPieces = new List<TetrisPiece>();
        PlayersPiecePlaces = new List<TetrisPiecePlace>();
        RivalPiecePlaces = new List<TetrisPiecePlace>();
        State = GameState.Idle;
    }

    private void Update()
    {
        if (isGameActive)
        {
            switch (State)
            {
                case GameState.Playing:
                    break;
                case GameState.Win:
                    {
                        OnGameWin?.Invoke();
                        isGameActive = false;
                    }
                    break;
                case GameState.Lose:
                    {
                        OnGameLose?.Invoke();
                        isGameActive = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
