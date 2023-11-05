using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineConfiner2D confiner;

    private void Start()
    {
        confiner.m_BoundingShape2D = GlobalManagers.Instance.GameManager.cameraConfiner;
    }
    public void ShakeCamera(Vector3 shakeAmount)
    {
        impulseSource.GenerateImpulse(shakeAmount);
    }
}
