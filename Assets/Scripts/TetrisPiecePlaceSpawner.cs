using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiecePlaceSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrisPiecePlaces;

    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            var selectedPiecePlace = tetrisPiecePlaces[Random.Range(0, tetrisPiecePlaces.Count)];
            var piecePlace = Instantiate(selectedPiecePlace).GetComponent<TetrisPiecePlace>();
            piecePlace.transform.parent = transform;
            piecePlace.transform.localPosition = new Vector3(0f, 1f, (1f * GameManager.Instance.SpawnedPiecePlaces.Count) + 5f);
            piecePlace.PiecePlaceHolder.SetActive(i == 0);

            GameManager.Instance.SpawnedPiecePlaces.Add(piecePlace);
        }
    }
}
