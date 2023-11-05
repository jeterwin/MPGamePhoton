using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class GamePoolingManager : MonoBehaviour, INetworkObjectPool
{
    //The key is the prefab aka the bullet and the network object is the instantiated bullet
    private Dictionary<NetworkObject, List<NetworkObject>> instantiatedPrefabs = new();
    private void Start()
    {
        if(GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.PoolingManager = this;

        }
    }
    //Called once when runner.spawn is called
    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject networkObject = null;
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
        instantiatedPrefabs.TryGetValue(prefab, out var networkObjects);

        bool foundMatch = false;
        if(networkObjects?.Count > 0)
        {
            foreach(var item in networkObjects)
            {
                if(item != null && !item.gameObject.activeSelf)
                {
                    networkObject = item;
                    foundMatch = true;

                    break;
                }
            }
        }

        if(!foundMatch)
        {
            networkObject = createObjectInstance(prefab);
        }
        
        return networkObject;
    }
    private NetworkObject createObjectInstance(NetworkObject prefab)
    {
        var obj = Instantiate(prefab);

        if(instantiatedPrefabs.TryGetValue(prefab, out var instantiatedObject))
        {
            instantiatedObject.Add(obj);
        }
        else
        {
            var list = new List<NetworkObject>() { obj };
            instantiatedPrefabs.Add(prefab, list);
        }

        return obj;
    }

    //Called once when runner.despawn is called
    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        instance.gameObject.SetActive(false);
    }

    public void RemoveNetworkObject(NetworkObject obj)
    {
        if(instantiatedPrefabs.Count > 0)
        {
            foreach(var item in instantiatedPrefabs)
            {
                foreach(var networkObject in item.Value.Where(networkObject => networkObject == obj))
                {
                    item.Value.Remove(networkObject);
                    break;
                }
            }
        }
    }
}
