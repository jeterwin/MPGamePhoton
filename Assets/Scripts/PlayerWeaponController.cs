using Fusion;
using System;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    public Quaternion LocalQuaternion { get; private set; }

    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private float delayBetweenShots = 0.1f;

    [SerializeField] private ParticleSystem muzzleEffect;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform pivotToRotate;
    [SerializeField] private Transform firePointPos;

    [Networked] private Quaternion currentPlayerRotation { get; set; }

    [Networked] private NetworkButtons buttonsPrev { get; set; }

    [Networked(OnChanged = nameof(OnChangedMuzzleEffect))] private NetworkBool playMuzzleEffect { get; set; }

    [Networked, HideInInspector] public NetworkBool IsHoldingShoot { get; set; }

    [Networked] private TickTimer shootCooldown { get; set; }

    private PlayerController playerController;
    public override void Spawned()
    {
        playerController = GetComponent<PlayerController>();
    }
    public void BeforeUpdate()
    {
        if(Object.HasInputAuthority && playerController.AcceptAnyInput)
        {
	        Vector2 positionOnScreen = cam.WorldToViewportPoint(transform.position);
		
	        //Get the Screen position of the mouse
	        Vector2 mouseOnScreen = (Vector2)cam.ScreenToViewportPoint(Input.mousePosition);
		
	        //Get the angle between the points
	        float angle = AngleBetweenTwoPoints(mouseOnScreen, positionOnScreen);

	        LocalQuaternion = Quaternion.Euler(new Vector3(0f,0f,angle));
        }
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) 
    {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}
    public override void FixedUpdateNetwork()
    {
        //if(GetInput(out PlayerData input))
        if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if(playerController.AcceptAnyInput)
            {
                checkShootingInput(input);
                currentPlayerRotation = input.GunPivotRotation;

                buttonsPrev = input.NetworkButtons;
            }
            else
            {
                IsHoldingShoot = false;
                playMuzzleEffect = false;
                buttonsPrev = default;
            }
        }

        pivotToRotate.rotation = currentPlayerRotation;
    }
    private static void OnChangedMuzzleEffect(Changed<PlayerWeaponController> changed)
    {
        var currentState = changed.Behaviour.playMuzzleEffect;
        changed.LoadOld();

        var oldState = changed.Behaviour.playMuzzleEffect;
        if(currentState != oldState)
            changed.Behaviour.playOrStopMuzzleEffect(currentState);
    }
    private void playOrStopMuzzleEffect(bool play)
    {
        if(play)
            muzzleEffect.Play();
        else
            muzzleEffect.Stop();
    }
    private void checkShootingInput(PlayerData input)
    {
        var currentBts = input.NetworkButtons.GetPressed(buttonsPrev);

        IsHoldingShoot = currentBts.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot);

        if(currentBts.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot) && shootCooldown.ExpiredOrNotRunning(Runner))
        {
            playMuzzleEffect = true;
            shootCooldown = TickTimer.CreateFromSeconds(Runner, delayBetweenShots);
            Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
        }
        else
        {
            playMuzzleEffect = false;
            //turn off muzle
        }
    }
}
