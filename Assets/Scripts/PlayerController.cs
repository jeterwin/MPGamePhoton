using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    public bool AcceptAnyInput => IsPlayerAlive && !GameManager.MatchIsOver;

    [SerializeField] private GameObject groundCheckPoint;
    [SerializeField] private float groundCheckHeight;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject localCamera;
    [SerializeField] private TextMeshProUGUI playerNickname;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float respawnTimer = 5f;
    [Networked(OnChanged = nameof(onNicknameChange))] private NetworkString<_8> playerName { get; set; }

    [Networked] public NetworkBool IsPlayerAlive { get; private set; }

    [Networked] public TickTimer RespawnTimer { get; set; }

    [Networked] private NetworkButtons buttons { get; set; }

    private bool isGrounded = false;
    private float horizontal;
    private Rigidbody2D rb;
    private PlayerWeaponController weaponController;
    private PlayerVisualController visualController;
    private PlayerHealthController healthController;

    public float JumpForce = 5f;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponController = GetComponent<PlayerWeaponController>();
        visualController = GetComponent<PlayerVisualController>();
        healthController = GetComponent<PlayerHealthController>();

        if(Object.IsValid == Object.HasInputAuthority)
        {
            localCamera.transform.SetParent(null);
            localCamera.SetActive(true);
            RpcSetNickname(GlobalManagers.Instance.NetworkRunnerController.LocalPlayerName);
        }
        else
        {
            GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
        }
        IsPlayerAlive = true;
    }
    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickname(NetworkString<_8> name)
    {
        playerName = name;
    }
    private static void onNicknameChange(Changed<PlayerController> changed)
    {
        changed.Behaviour.setPlayerNickname(changed.Behaviour.playerName);
    }
    private void setPlayerNickname(NetworkString<_8> nickname)
    {
        playerNickname.text = nickname + "/" + Object.InputAuthority.PlayerId;
    }
    private void checkJumpInput(PlayerData input)
    {
        isGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox(groundCheckPoint.transform.position,
            groundCheckPoint.transform.localScale, 0, groundMask);

        if(!isGrounded) { return; }

        var pressed = input.NetworkButtons.GetPressed(buttons);
        if(pressed.WasPressed(buttons, PlayerInputButtons.Jump))
        {
            rb.AddForce(JumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPoint.transform.position, groundCheckPoint.transform.localScale);
    }
    public override void FixedUpdateNetwork()
    {
        checkRespawnTimer();

        //if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        if(GetInput(out PlayerData input) && AcceptAnyInput)
        {
            rb.velocity = new Vector2(input.HorizontalInput * movementSpeed, rb.velocity.y);

            checkJumpInput(input);

            buttons = input.NetworkButtons;
        }
    }
    public void BeforeUpdate()
    {
        if(Object.HasInputAuthority && AcceptAnyInput)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
    }
    public void KillPlayer()
    {
        IsPlayerAlive = false;
        rb.simulated = false;
        visualController.TriggerDeath();

        RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnTimer);
    }
    public void RespawnPlayer()
    {
        IsPlayerAlive = true;
        rb.simulated = true;
        healthController.resetHealth();
        visualController.TriggerRespawn();
    }
    private void checkRespawnTimer()
    {
        if(IsPlayerAlive) { return; }

        if(RespawnTimer.Expired(Runner))
        {
            RespawnTimer = TickTimer.None;
            RespawnPlayer();
        }
    }
    public override void Render()
    {
        visualController.RendererVisuals(rb.velocity, weaponController.IsHoldingShoot);
    }
    public PlayerData GetPlayerNetworkData()
    {
        PlayerData data = new()
        {
            HorizontalInput = horizontal,
            GunPivotRotation = weaponController.LocalQuaternion
        };
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
        data.NetworkButtons.Set(PlayerInputButtons.Shoot, Input.GetButton("Fire1"));
        return data;
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GlobalManagers.Instance.PoolingManager.RemoveNetworkObject(Object);
        Destroy(gameObject);
    }
    public enum PlayerInputButtons
    {
        None,
        Jump,
        Shoot
    }
}
