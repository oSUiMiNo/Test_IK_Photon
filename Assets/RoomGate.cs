using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.UIElements;

public class RoomGate : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        GameObject Avatar = null;
        if (PhotonNetwork.IsMasterClient)
        {
            Avatar = PhotonNetwork.Instantiate("Avatar", new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Debug.Log("マスターではない");
            GameObject.Find("IKMarker").transform.parent = Avatar.transform;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("誰か入ってきた");
    }
}

public enum PhotonStates
{
    Default,
    TryingConectingToMasterServer,
    ConectedToMasterServer,
    Ready
}