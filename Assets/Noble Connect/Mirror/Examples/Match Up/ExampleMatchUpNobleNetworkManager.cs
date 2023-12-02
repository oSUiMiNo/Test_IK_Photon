using System.Collections.Generic;
using MatchUp;
using NobleConnect.Mirror;
using UnityEngine;
using Mirror;
using System;

namespace NobleConnect.Examples.Mirror
{
    // Example implementation of NobleNetworkManager utilizing Match Up for matchmaking
    // Look at ExampleMatchUpHUD for more information on how to use it. 
    public class ExampleMatchUpNobleNetworkManager : NobleNetworkManager
    {

        Matchmaker matchUp;

        override public void Start()
        {
            base.Start();
            matchUp = GetComponent<Matchmaker>();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Client connected.");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("Client disconnected.");
        }

        public override void OnClientError(Exception exception)
        {
            base.OnClientError(exception);
            // Handle client errors here
        }

        public override void OnServerError(Exception exception)
        {
            base.OnClientError(exception);
            // Handle server errors here
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            Debug.Log("Server received a client connection.");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log("Server lost a client.");
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            Debug.Log("Destroying match.");
            matchUp.DestroyMatch();
        }

        // OnServerPrepared is called when the host is listening and has received 
        // their HostEndPoint from the NobleConnect service.
        // This is a good time to create a match using Match Up.
        // The hostAddress and hostPort are stored as matchData for the newly created match.
        // Clients will select a match and retrieve the ip and port to connect to the host.
        // Look in ExampleMatchUpHUD.cs to see how that works.
        public override void OnServerPrepared(string hostAddress, ushort hostPort)
        {
            // Create the match data and add the host address and port
            var matchData = new Dictionary<string, MatchData>() {
                { "HostAddress", hostAddress },
                { "HostPort", (int)hostPort },
            };

            // Create the Match with the associated MatchData
            matchUp.CreateMatch(maxConnections + 1, matchData);

            Debug.Log("Creating match: " + hostAddress + ":" + hostPort);
        }
    }
}