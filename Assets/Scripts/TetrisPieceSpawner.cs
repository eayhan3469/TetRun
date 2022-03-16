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
        minPos = new Vector2(spawnArea.bounds.min.x, spawnArea.bounds.min.z);
        maxPos = new Vector2(spawnArea.bounds.max.x, spawnArea.bounds.max.z);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            var selectedPiece = tetrisPieces[Random.Range(0, tetrisPieces.Count)];

            var spawnPos = new Vector3(Random.Range(minPos.x, maxPos.x), 1f, Random.Range(minPos.y, maxPos.y));

            foreach (var piece in GameManager.Instance.SpawnedPieces)
                if (Vector3.Distance(spawnPos, piece.transform.position) < 3f)
                    return;

            var spawnedPiece = Instantiate(selectedPiece, spawnPos, Quaternion.Euler(new Vector3(90f, 0f, 0f)), null).GetComponent<TetrisPiece>();
            GameManager.Instance.SpawnedPieces.Add(spawnedPiece);

            spawnedPiece.transform.DOScale(1.2f, 0.25f).SetEase(Ease.InOutCubic).OnComplete(() => spawnedPiece.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

        }).SetLoops(-1);
    }
}
