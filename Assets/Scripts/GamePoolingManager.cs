using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GamePoolingManager : MonoBehaviour, INetworkObjectPool
{
    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        throw new System.NotImplementedException();
    }

}
