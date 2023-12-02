using MatchUp;
using NobleConnect.NetCodeForGameObjects;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NobleConnect.Examples.NetCodeForGameObjects
{
    public class ExampleMatchUpHUD : MonoBehaviour
    {
        public Matchmaker matchmaker;
        public GameObject[] joinMatchButtons;
        public TMPro.TMP_Text hostStatusText;
        public TMPro.TMP_Text clientStatusText;
        public TMPro.TMP_Text connectionStatusText;

        public GameObject startPanel;
        public GameObject clientPanel;
        public GameObject clientConnectedPanel;
        public GameObject hostPanel;

        Match[] matchList;

        NobleUnityTransport transport;

        private void Start()
        {
            transport = (NobleUnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            transport.OnServerPreparedCallback += OnServerPrepared;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
            if (transport)
            {
                transport.OnServerPreparedCallback -= OnServerPrepared;
            }
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();

            startPanel.SetActive(false);
            hostPanel.SetActive(true);
        }

        private void OnServerPrepared(string relayAddress, ushort relayPort)
        {
            var matchData = new Dictionary<string, MatchData>();
            matchData["name"] = "Layla's Match " + relayAddress + ":" + relayPort;
            matchData["ip"] = relayAddress;
            matchData["port"] = (int)relayPort;
            matchmaker.CreateMatch(10, matchData, OnMatchCreated);
        }

        private void OnMatchCreated(bool success, Match theMatch)
        {
            hostStatusText.text = "Match created";
        }

        public void ShowStartPanel()
        {
            startPanel.SetActive(true);
            clientPanel.SetActive(false);
            hostPanel.SetActive(false);
            clientConnectedPanel.SetActive(false);
        }

        public void ShowClientPanel()
        {
            startPanel.SetActive(false);
            clientPanel.SetActive(true);

            clientStatusText.text = "Fetching matches..";

            matchmaker.GetMatchList(OnMatchListReceived);
        }

        private void OnMatchListReceived(bool success, Match[] matches)
        {
            clientStatusText.text = "Match list received";

            matchList = matches;

            for (int i = 0; i < joinMatchButtons.Length; i++)
            {
                joinMatchButtons[i].SetActive(false);
            }

            for (int i = 0; i < matches.Length; i++)
            {
                string matchName = matches[i].matchData["name"];
                joinMatchButtons[i].GetComponentInChildren<TMPro.TMP_Text>().text = matchName;
                joinMatchButtons[i].SetActive(true);
            }
        }

        public void JoinMatch(int matchIndex)
        {
            connectionStatusText.text = "Joining match..";

            Match match = matchList[matchIndex];
            string address = match.matchData["ip"];
            int port = match.matchData["port"];

            transport.ConnectionData.Address = address;
            transport.ConnectionData.Port = (ushort)port;

            NetworkManager.Singleton.StartClient();

            clientPanel.SetActive(false);
            clientConnectedPanel.SetActive(true);
        }

        private void OnClientConnected(ulong clientID)
        {
            connectionStatusText.text = "Connected via " + transport.LatestConnectionType.ToString();
        }

        private void OnClientDisconnected(ulong clientID)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                ShowStartPanel();
            }
        }

        public void StopHost()
        {
            NetworkManager.Singleton.Shutdown();

            matchmaker.DestroyMatch();

            hostPanel.SetActive(false);
            startPanel.SetActive(true);
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();

            clientConnectedPanel.SetActive(false);
            startPanel.SetActive(true);
        }
    }
}