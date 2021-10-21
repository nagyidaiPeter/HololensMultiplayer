using Assets.Scripts.SERVER;

using hololensMultiplayer;

using Lidgren.Network;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class ServerHandler : MonoBehaviour
{
    private Server server;

    public bool RunningServer = false;

    //In SEC
    public float UpdateTime = 0.00833f;

    [Inject]
    private DataManager dataManager;

    [Inject]
    public void Init(Server ser)
    {
        server = ser;
    }

    void Start()
    {
        StartCoroutine(ServerUpdate());
    }

    public void StartServer()
    {
        Debug.Log("Starting server..");
        server.StartServer();
        RunningServer = true;
        dataManager.IsServer = true;

        GetComponent<ObjectSpawner>().SpawnObject("CoffeeCup");
        GetComponent<ObjectSpawner>().SpawnObject("Cheese");
    }

    private void OnDestroy()
    {
        StopServer();
    }
    public void StopServer()
    {
        Debug.Log("Stopping server..");
        RunningServer = false;
        dataManager.IsServer = false;        
        StopAllCoroutines();
        server.StoptServer();
    }

    private IEnumerator ServerUpdate()
    {
        while (true)
        {
            if (RunningServer)
            {
                server.ReadAndProcess();
                server.Reactions();
            }

            yield return new WaitForSeconds(UpdateTime);
        }
    }

}