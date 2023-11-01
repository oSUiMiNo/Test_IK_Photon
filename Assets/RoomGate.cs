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
using Unity.VisualScripting;

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
        StartCoroutine(InitAvatar());
    }

    IEnumerator InitAvatar()
    {
        GameObject Avatar = null;
        if (PhotonNetwork.IsMasterClient)
        {
            Avatar = LoadNetWorkObject("Avatar", new Vector3(0, 0, 0), Quaternion.identity);
            Destroy(Avatar.GetComponent<Test_IK>());
        }
        else
        {
            Debug.Log("マスターではない");
            yield return new WaitForSeconds(4);
            Avatar = GameObject.Find("Avatar(Clone)");
            GameObject.Find("IKMarker").transform.parent = Avatar.transform;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("誰か入ってきた");
    }


    GameObject LoadNetWorkObject(string name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PhotonNetwork.Instantiate(name, position, rotation);
        //obj.name = obj.name.Replace("(Clone)", "");
        return obj;
    }
}
    
public enum PhotonStates
{
    Default,
    TryingConectingToMasterServer,
    ConectedToMasterServer,
    Ready
}