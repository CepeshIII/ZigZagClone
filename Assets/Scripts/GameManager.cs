using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PathGenerator tileManager;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] ScoreManager scoreManager;

    private bool isGameOver = false;

    private void OnEnable()
    {
        if (player == null) 
            GameObject.FindGameObjectWithTag("Player")
                .TryGetComponent<Player>(out player);

        if (tileManager == null)
            GameObject.FindGameObjectWithTag("PathGenerator")
                .TryGetComponent<PathGenerator>(out tileManager);

        if (scoreManager == null)
            GameObject.FindGameObjectWithTag("ScoreManager")
                .TryGetComponent<ScoreManager>(out scoreManager);
        
    }

    private void Update()
    {
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(player.transform.position.y < -20 && !isGameOver)
        {
            GameOver();
        }
    }

    private void SavePlayerStats()
    {
        var bestResult = PlayerPrefs.GetInt("BestScore");
        var currentResult = scoreManager.Score;

        if(bestResult < currentResult)
        {
            PlayerPrefs.SetInt("BestScore", currentResult);
        }

        PlayerPrefs.SetInt("LastScore", currentResult);

    }

    private void GameOver()
    {
        SavePlayerStats();
        sceneLoader.LoadMenuScene();
        tileManager.Destroy();
        isGameOver = true;
    }
}