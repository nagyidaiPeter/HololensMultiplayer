using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using XRMultiplayer.Networking;

public class ServerMenu : MonoBehaviour
{

    private ServerHandler serverHandler;
    private ClientHandler clientHandler;

    public GameObject hostButton, stopButton;


    void Start()
    {
        serverHandler = FindObjectOfType<ServerHandler>();
        clientHandler = FindObjectOfType<ClientHandler>();

    }



    public void EnableServerMenu()
    {
        if (clientHandler.IsConnected && !serverHandler.IsServerRunning)
            return;

        if (clientHandler.IsConnected)
        {
            stopButton.SetActive(true);
        }
        else
        {
            hostButton.SetActive(true);
        }
    }

    public void DisableServerMenu()
    {
        stopButton.SetActive(false);
        hostButton.SetActive(false);
    }
}
