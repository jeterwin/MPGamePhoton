using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private GameObject localCamera;
    [SerializeField] private TextMeshProUGUI playerNickname;
    [SerializeField] private float movementSpeed = 1f;

    [Networked(OnChanged = nameof(onNicknameChange))] private NetworkString<_8> playerName { get; set; }

    [Networked] private NetworkButtons buttons { get; set; }
    private float horizontal;
    private Rigidbody2D rb;
    private PlayerWeaponController weaponController;
    private PlayerVisualController visualController;

    public float JumpForce = 5f;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponController = GetComponent<PlayerWeaponController>();
        visualController = GetComponent<PlayerVisualController>();

        if(Object.HasInputAuthority)
        {
            localCamera.SetActive(true);
            RpcSetNickname(GlobalManagers.Instance.networkRunnerController.LocalPlayerName);
        }
    }
    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickname(NetworkString<_8> name)
    {
        playerName = name;
    }
    public override void FixedUpdateNetwork()
    {
        //if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        if(GetInput(out PlayerData input))
        {
            rb.velocity = new Vector2(input.HorizontalInput * movementSpeed, rb.velocity.y);

            checkJumpInput(input);
        }
    }
    private static void onNicknameChange(Changed<PlayerController> changed)
    {
/*        changed.LoadNew();
        var newNickname = changed.Behaviour.playerNickname;

        changed.LoadOld();
        var oldNickname = changed.Behaviour.playerNickname;*/;
        changed.Behaviour.setPlayerNickname(changed.Behaviour.playerName);
    }
    private void setPlayerNickname(NetworkString<_8> nickname)
    {
        playerNickname.text = nickname + "/" + Object.InputAuthority.PlayerId;
    }
    public void BeforeUpdate()
    {
        if(Runner.IsPlayerValid(Runner.LocalPlayer))
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
    }
    private void checkJumpInput(PlayerData input)
    {
        var pressed = input.NetworkButtons.GetPressed(buttons);
        if(pressed.WasPressed(buttons, PlayerInputButtons.Jump))
        {
            rb.AddForce(JumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        buttons = input.NetworkButtons;
    }
    public override void Render()
    {
        visualController.RendererVisuals(rb.velocity);
    }
    public PlayerData GetPlayerNetworkData()
    {
        PlayerData data = new();
        data.HorizontalInput = horizontal;
        data.GunPivotRotation = weaponController.LocalQuaternion;
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
        return data;
    }
    public enum PlayerInputButtons
    {
        None,
        Jump
    }
}
