using hololensMultiplayer;

using UnityEngine;
using Zenject;

using static NetworkObject;

public class ObjectSpawner : MonoBehaviour
{
    [Inject]
    private DataManager dataManager;

    [Inject]
    private ObjectFactory objectFactory;

    private void Start()
    {

    }

    public void SpawnObject(string name)
    {
        var networkObject = objectFactory.Create(Resources.Load($"Objects/{name}"));
        var newPlayerGO = networkObject.gameObject;
        newPlayerGO.SetActive(true);
        newPlayerGO.transform.parent = GameObject.Find("NetworkSpace").transform;
        dataManager.Objects.Add(networkObject.objectData.objectTransform.ObjectID, networkObject.objectData);
    }
}


