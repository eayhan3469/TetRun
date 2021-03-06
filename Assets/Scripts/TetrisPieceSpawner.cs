using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TetrisPieceSpawner : MonoBehaviour
{
    [SerializeField]
    private BoxCollider spawnArea;

    [SerializeField]
    private List<GameObject> tetrisPieces;

    private Vector2 minPos;
    private Vector2 maxPos;

    private void Start()
    {
        minPos = new Vector2(spawnArea.bounds.min.x + 2f, spawnArea.bounds.min.z + 2f);
        maxPos = new Vector2(spawnArea.bounds.max.x - 2f, spawnArea.bounds.max.z - 2f);

        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (GameManager.Instance.State == GameManager.GameState.Playing)
            {
                var selectedPiece = tetrisPieces[Random.Range(0, tetrisPieces.Count)];

                var spawnPos = new Vector3(Random.Range(minPos.x, maxPos.x), 1f, Random.Range(minPos.y, maxPos.y));

                foreach (var piece in GameManager.Instance.SpawnedPieces)
                    if (Vector3.Distance(spawnPos, piece.transform.position) < 3f)
                        return;

                var spawnedPiece = Instantiate(selectedPiece, spawnPos, Quaternion.Euler(new Vector3(90f, 0f, 0f)), transform).GetComponent<TetrisPiece>();
                GameManager.Instance.SpawnedPieces.Add(spawnedPiece);

                spawnedPiece.transform.DOScale(1.2f, 0.25f).SetEase(Ease.InOutCubic).OnComplete(() => spawnedPiece.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

            }
        }).SetLoops(-1);
    }
}
