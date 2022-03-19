using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += OnGameWin;
        GameManager.Instance.OnGameLose += OnGameLose;
    }

    private void OnGameWin()
    {
        Debug.Log("Game win ui");
    }

    private void OnGameLose()
    {
        Debug.Log("Game lose ui");
    }
}
