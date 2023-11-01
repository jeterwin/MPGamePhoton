using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private int damage;

    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 3f;

    [Networked] private NetworkBool hitSomething { get; set; }

    private Collider2D collider;
    [Networked] private TickTimer lifetimeTimer { get; set; }
    
    public override void Spawned()
    {
        collider = GetComponent<Collider2D>();
        lifetimeTimer = TickTimer.CreateFromSeconds(Runner, bulletLifetime);
    }
    public override void FixedUpdateNetwork()
    {
        if(!hitSomething)
        {
            checkIfHitGround();
            checkIfHitPlayer();
        }

        if(!lifetimeTimer.ExpiredOrNotRunning(Runner) && !hitSomething)
        {
            transform.Translate(bulletSpeed * Runner.DeltaTime * transform.right, Space.World);
        }

        if(lifetimeTimer.Expired(Runner) || hitSomething)
        {
            lifetimeTimer = TickTimer.None;

            Runner.Despawn(Object);
        }
    }

    private void checkIfHitGround()
    {
        var groundCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, collider.bounds.size, 0, groundMask);

        if(groundCollider != default)
        {
            hitSomething = true;
        }
    }

    private List<LagCompensatedHit> hits = new();
    private void checkIfHitPlayer()
    {
        Runner.LagCompensation.OverlapBox(transform.position, collider.bounds.size, Quaternion.identity, 
            Object.InputAuthority, hits, playerMask);

        if(hits.Count > 0)
        {
            foreach(LagCompensatedHit hitPlayer in hits)
            {
                if(hitPlayer.Hitbox != null)
                {
                    var player = hitPlayer.Hitbox.GetComponentInParent<PlayerController>();
                    bool didNotHitMyself = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                    if(didNotHitMyself && player.IsPlayerAlive)
                    {
                        if(Runner.IsServer)
                        {
                            //do damage
                            Debug.Log("Hit player");
                            player.GetComponent<PlayerHealthController>().Rpc_reduceHealth(damage);
                        }

                        hitSomething = true;
                        break;
                    }
                }
            }
        }
    }
}
