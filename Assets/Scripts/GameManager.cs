using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;

public class GameManager : NetworkBehaviour
{
    public event Action OnGameOver;
    public Transform MessageGrid;

    public static bool MatchIsOver { get; set; }
    [field: SerializeField] public Collider2D cameraConfiner { get; private set; }

    [SerializeField] private GameObject cam;
    [SerializeField] private TextMeshProUGUI timerText;
    [Tooltip("Time in seconds")]
    [SerializeField] private float matchTimerAmount = 60;
    [Networked] private TickTimer matchTimer { get; set; }

    private void Awake()
    {
        if(GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }
    public override void Spawned()
    {
        MatchIsOver = false;

        cam.SetActive(false);
        matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    }

    public override void FixedUpdateNetwork()
    {
        if(!matchTimer.Expired(Runner) && matchTimer.RemainingTime(Runner).HasValue)
        {
            var timeSpan = TimeSpan.FromSeconds(matchTimer.RemainingTime(Runner).Value);
            string output = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            timerText.text = output;
        }
        else if(matchTimer.Expired(Runner))
        {
            MatchIsOver = true;
            matchTimer = TickTimer.None;

            OnGameOver?.Invoke();
        }
    }
}
