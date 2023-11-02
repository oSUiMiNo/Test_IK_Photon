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
using EVMC4U;
using UnityEditor.Animations;
using Cysharp.Threading.Tasks;

public class RoomGate : MonoBehaviourPunCallbacks
{
    [SerializeField, Label("同期方法")]
    public SynchronizeType synchronizeType;


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }



    public override async void OnJoinedRoom()
    {
        await InitAvatar();
    }

    async UniTask InitAvatar()
    {
        GameObject Avatar = null;
        if (synchronizeType == SynchronizeType.IK)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Avatar = LoadNetWorkObject("Avatar_IK", new Vector3(0, 0, 0), Quaternion.identity);
                Destroy(Avatar.GetComponent<Test_IK>());
                Avatar.GetComponent<Animator>().runtimeAnimatorController = null;
                GameObject ExternalReceiver = GameObject.Find("ExternalReceiver");
                Avatar.transform.parent = ExternalReceiver.transform;
                ExternalReceiver.GetComponent<ExternalReceiver>().Model = Avatar;
            }
            else
            {
                Debug.Log("マスターではない");
                await UniTask.Delay(TimeSpan.FromSeconds(4));
                Avatar = GameObject.Find("Avatar_IK(Clone)");
                GameObject.Find("IKMarker").transform.parent = Avatar.transform;
                Destroy(Avatar.GetComponent<MarkerController>());
            }
        }
        else 
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Avatar = LoadNetWorkObject("Avatar_OSC", new Vector3(0, 0, 0), Quaternion.identity);
                Avatar.GetComponent<Animator>().runtimeAnimatorController = null;
                GameObject ExternalReceiver = GameObject.Find("ExternalReceiver");
                Avatar.transform.parent = ExternalReceiver.transform;
                ExternalReceiver.GetComponent<ExternalReceiver>().Model = Avatar;
            }
            else
            {
                Debug.Log("マスターではない");
                await UniTask.Delay(TimeSpan.FromSeconds(4));
                Avatar = GameObject.Find("Avatar_OSC(Clone)");
                Avatar.GetComponent<Animator>().runtimeAnimatorController = null;
                GameObject ExternalReceiver = GameObject.Find("ExternalReceiver");
                Avatar.transform.parent = ExternalReceiver.transform;
                ExternalReceiver.GetComponent<ExternalReceiver>().Model = Avatar;
            }
        }
    }

    


    public async override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("誰か入ってきた");
        await UniTask.Delay(TimeSpan.FromSeconds(4));
        //string IP = await GetGlobalIP.UseAPI();
        //Debug.Log(IP);
        photonView.RPC(nameof(ShareIP), RpcTarget.AllBuffered, await GetGlobalIP.UseAPI());
    }


    [PunRPC]
    private void ShareIP(string GlobalIP)
    {
        Debug.Log($"IPはこれ {GlobalIP}");
    }


    GameObject LoadNetWorkObject(string name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PhotonNetwork.Instantiate(name, position, rotation);
        //obj.name = obj.name.Replace("(Clone)", "");
        return obj;
    }
}
    
public enum SynchronizeType
{
    IK,
    OSC
}