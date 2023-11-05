using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button returnToLobbyBtn;
    [SerializeField] private GameObject gameOverPanel;
    void Start()
    {
        GlobalManagers.Instance.GameManager.OnGameOver += onMatchIsOver;
        returnToLobbyBtn.onClick.AddListener(GlobalManagers.Instance.NetworkRunnerController.ShutDownRunner);
    }
    private void onMatchIsOver()
    {
        gameOverPanel.SetActive(true);
    }
    private void OnDestroy()
    {
        GlobalManagers.Instance.GameManager.OnGameOver -= onMatchIsOver;
    }
}
