using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SERVER.Processors;
using hololensMultiplayer;

using HRM;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class NetworkObject : MonoBehaviour
{
    public int OwnerID = -1;
    public float InterVel = 35;
    public ObjectData objectData = null;

    [Inject]
    private DataManager dataManager;

    [Inject]
    private ClientObjectProcessor objectProcessor;

    void Start()
    {
        var interactable = GetComponent<NearInteractionTouchableVolume>();
    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(objectData.Position.x, objectData.Position.y, objectData.Position.z), transform.localPosition) > 1.5f)
        {
            transform.localPosition = objectData.Position;
            transform.localRotation = objectData.Rotation;
            transform.localScale = objectData.Scale;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectData.Position, InterVel * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, objectData.Rotation, InterVel * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, objectData.Scale, InterVel * Time.deltaTime);
        }
    }

    public void ClaimObject()
    {
        OwnerID = dataManager.LocalPlayer.ID;
    }

    public class ObjectFactory : PlaceholderFactory<UnityEngine.Object, NetworkObject>
    {
        public static int ID;

        readonly DiContainer _container;

        private static Dictionary<string, List<GameObject>> ObjectPools = new Dictionary<string, List<GameObject>>();

        public ObjectFactory(DiContainer container)
        {
            _container = container;
            ObjectPools.Clear();
        }

        public void AddToPool(NetworkObject networkObject)
        {
            string ObjectKey = networkObject.name;
            if (!ObjectPools.ContainsKey(ObjectKey))
            {
                ObjectPools.Add(ObjectKey, new List<GameObject>());
            }

            ObjectPools[ObjectKey].Add(networkObject.gameObject);
            networkObject.gameObject.SetActive(false);
        }

        public override NetworkObject Create(UnityEngine.Object prefab)
        {
            if (int.MaxValue == ID)
            {
                ID = 0;
            }

            ID++;

            NetworkObject instance;

            if (prefab is GameObject gameObject)
            {
                var prefabScript = gameObject.GetComponent<NetworkObject>();

                if (ObjectPools[prefab.name].Any())
                {
                    var newBug = ObjectPools[prefab.name].First();
                    ObjectPools[prefab.name].Remove(newBug);
                    instance = newBug.GetComponent<NetworkObject>();
                }
                else
                {
                    instance = _container.InstantiatePrefabForComponent<NetworkObject>(prefab, new Vector3(0, 0, 0), new Quaternion(), null);
                }

                instance.objectData.ID = ID;

                return instance;
            }

            return null;
        }
    }
}
