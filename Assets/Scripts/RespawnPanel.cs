using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class RespawnPanel : SimulationBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private TextMeshProUGUI respawnTimerText;
    [SerializeField] private GameObject panel;

    public override void FixedUpdateNetwork()
    {
        if(!playerController.RespawnTimer.IsRunning || !Object.HasInputAuthority) 
        { 
            panel.SetActive(false);
            return;
        }

        var timeRemaining = playerController.RespawnTimer.RemainingTime(Runner).Value;
        respawnTimerText.text = Mathf.RoundToInt(timeRemaining).ToString();
        panel.SetActive(true);
    }
}
