using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private PlayerCameraController cameraController;
    [SerializeField] private LayerMask deathGroundMask;
    [SerializeField] private Animator animator;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Image fillAmount;
    [SerializeField] private TextMeshProUGUI textFillAmount;
    [Networked(OnChanged = nameof(healthAmountChange))] private int currentHealthAmount { get; set; }

    private PlayerController playerController;
    private Collider2D coll2d;
    private static void healthAmountChange(Changed<PlayerHealthController> changed)
    {
        var currentHealth = changed.Behaviour.currentHealthAmount;

        changed.LoadOld();

        var oldHealth = changed.Behaviour.currentHealthAmount;

        if(currentHealth != oldHealth)
        {
            changed.Behaviour.updateVisuals(currentHealth);
            
            //We did not respawn
            if(currentHealth != changed.Behaviour.maxHealth)
            {
                changed.Behaviour.dealDamage(currentHealth);
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        if(!playerController.IsPlayerAlive || !Runner.IsServer) { return; }
        
        bool didHitDeathGround = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll2d.bounds.size,
            0, deathGroundMask);

        if (didHitDeathGround)
        {
            Rpc_reduceHealth(maxHealth);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_reduceHealth(int damage)
    {
        currentHealthAmount -= damage;
    }
    private void dealDamage(int healthAmount)
    {
        var isLocalPlayer = Object.HasInputAuthority;
        if(isLocalPlayer)
        {
            var shakeAmount = new Vector3(0.2f, 0.1f, 0);
            cameraController.ShakeCamera(shakeAmount);
            animator.Play("Hit");
        }

        if(healthAmount <= 0)
        {
            playerController.KillPlayer();
        }
    }
    private void updateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / maxHealth;

        fillAmount.fillAmount = num;
        textFillAmount.text = $"{healthAmount}/{maxHealth}";
    }

    public override void Spawned()
    {
        currentHealthAmount = maxHealth;
        coll2d = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
    }
    public void resetHealth()
    {
        currentHealthAmount = maxHealth;
        updateVisuals(currentHealthAmount);
    }
}
