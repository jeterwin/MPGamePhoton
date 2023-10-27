using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject gunGO;

    private Vector3 gunGOLocalScale;
    private Vector3 originalLocalScale;
    private bool init = false;

    private void Start()
    {
        originalLocalScale = transform.localScale;
        gunGOLocalScale = gunGO.transform.localScale;

        init = true;
    }
    private readonly int isMovingHash = Animator.StringToHash("IsWalking");
    public void RendererVisuals(Vector2 velocity)
    {
        if(!init) { return; } 

        var isMoving = Mathf.Abs(velocity.x) > 0.01f;

        animator.SetBool(isMovingHash, isMoving);

        FlipCharacter(velocity);
    }

    public void FlipCharacter(Vector2 velocity)
    {
        if(velocity.x > 0.01f)
        {
            transform.localScale = new(originalLocalScale.x, transform.localScale.y, transform.localScale.z);
            gunGO.transform.localScale = new(gunGOLocalScale.x, gunGOLocalScale.y, gunGOLocalScale.z);
        }
        else if(velocity.x < -0.01f)
        {
            transform.localScale = new(-originalLocalScale.x, transform.localScale.y, transform.localScale.z);
            gunGO.transform.localScale = new(-gunGOLocalScale.x, gunGOLocalScale.y, gunGOLocalScale.z);
        }
    }
}
