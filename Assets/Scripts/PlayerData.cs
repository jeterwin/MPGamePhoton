using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData : INetworkInput
{
    public float HorizontalInput;
    public Quaternion GunPivotRotation;
    public NetworkButtons NetworkButtons;
}
