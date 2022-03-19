using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiecePlaceSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrisPiecePlaces;
    [SerializeField] private int count;
    [SerializeField] private Type type;

    private enum Type
    {
        Player,
        Rival
    }

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var selectedPiecePlace = tetrisPiecePlaces[Random.Range(0, tetrisPiecePlaces.Count)];
            var piecePlace = Instantiate(selectedPiecePlace).GetComponent<TetrisPiecePlace>();
            piecePlace.transform.parent = transform;
            piecePlace.HasPlayer = type == Type.Player;
            piecePlace.PiecePlaceHolder.SetActive(i == 0);

            if (type == Type.Player)
            {
                piecePlace.transform.localPosition = new Vector3(0f, 1f, (1f * GameManager.Instance.PlayersPiecePlaces.Count) + 5f);
                GameManager.Instance.PlayersPiecePlaces.Add(piecePlace);
            }
            else
            {
                piecePlace.transform.localPosition = new Vector3(0f, 1f, (1f * GameManager.Instance.RivalPiecePlaces.Count) + 5f);
                GameManager.Instance.RivalPiecePlaces.Add(piecePlace);
            }
        }
    }
}
