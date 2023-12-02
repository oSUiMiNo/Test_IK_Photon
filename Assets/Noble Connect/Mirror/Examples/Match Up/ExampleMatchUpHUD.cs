#if MATCH_UP
using MatchUp;
using NobleConnect.Mirror;
using System;
using UnityEngine;
using Mirror;

namespace NobleConnect.Examples.Mirror
{
    // A GUI HUD for use with NobleNetworkManager
    public class ExampleMatchUpHUD : MonoBehaviour
    {
        // The NetworkManager controller by the HUD
        NobleNetworkManager networkManager;

        // A reference to the Matchmaker component
        Matchmaker matchUp;

        // Used to determine which GUI to display
        bool isHost, isClient;

        // Get a reference to the NetworkManager
        public void Start()
        {
            // Cast from Unity's network manager to a punchthrough network manager.
            networkManager = (NobleNetworkManager)NetworkManager.singleton;
            matchUp = GetComponent<Matchmaker>();
        }

        // Draw the GUI
        private void OnGUI()
        {
            if (!networkManager.isNetworkActive)
            {
                // Host button
                if (GUI.Button(new Rect(10, 10, 100, 30), "Host"))
                {
                    isHost = true;
                    isClient = false;

                    networkManager.StartHost();
                }

                // Client button
                if (GUI.Button(new Rect(10, 50, 100, 30), "Client"))
                {
                    // Get a list of matches. OnMatchList is called with the results when they are received.
                    matchUp.GetMatchList(OnMatchList, 0, 10, null, true);
                    isHost = false;
                    isClient = true;
                }

                networkManager.forceRelayConnection = GUI.Toggle(new Rect(10, 90, 100, 30), networkManager.forceRelayConnection, " Force Relay");
            }
            else
            {
                // Host or client GUI
                if (isHost) GUIHost();
                else if (isClient) GUIClient();
            }
        }

        // Draw the host GUI
        void GUIHost()
        {
            if (networkManager.HostEndPoint == null)
            {
                GUI.Label(new Rect(10, 10, 150, 22), "Retreiving HostEndPoint");
            }
            else if (matchUp.currentMatch == null || matchUp.currentMatch.id == -1)
            {
                GUI.Label(new Rect(10, 10, 150, 22), "Creating Match");
            }
            else
            {
                GUI.Label(new Rect(10, 10, 400, 22), "Match created " + networkManager.HostEndPoint.ToString());
            }
            // Disconnect Button
            if (GUI.Button(new Rect(10, 81, 110, 30), "Disconnect"))
            {
                networkManager.StopHost();
                GetComponent<Matchmaker>().DestroyMatch();
                isHost = false;
            }

            if (!NobleServer.active) isHost = false;
        }

        // Draw the client GUI
        void GUIClient()
        {
            if (networkManager.isNetworkActive)
            {
                GUI.Label(new Rect(10, 10, 300, 22), "Connection type: " + networkManager.client.latestConnectionType);
                // Disconnect button
                if (GUI.Button(new Rect(10, 50, 110, 30), "Disconnect"))
                {
                    if (networkManager.client.isConnected)
                    {
                        // If we are already connected it is best to quit gracefully by sending
                        // a disconnect message to the host.
                        networkManager.client.connection.Disconnect();;
                    }
                    else
                    {
                        // If the connection is still in progress StopClient will cancel it
                        networkManager.StopClient();
                    }
                    isClient = false;
                }
            }
        }

        // Join the first match in the list
        // The HostAddress and HostPort are retrieved from the associated match data
        // Connecting to the host works just like Mirror: Set the networkAddress and networkPort and call StartClient()
        private void OnMatchList(bool success, Match[] matches)
        {
            if (success && matches != null && matches.Length != 0)
            {
                var matchData = matches[0].matchData;
                isClient = true;
                networkManager.networkAddress = matchData["HostAddress"];
                networkManager.networkPort = matchData["HostPort"];
                networkManager.StartClient();
            }
        }
    }
}
#endif