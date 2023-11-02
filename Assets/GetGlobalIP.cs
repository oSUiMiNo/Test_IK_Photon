using System.IO;
using UnityEngine;
using System.Net;
using System.Text;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;

public class GetGlobalIP : MonoBehaviourPunCallbacks
{
    private async void Start()
    {
        //await GlobalIP();
        await LocalIP();
    }

    public static async UniTask<Dictionary<string, string>> LocalIP()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3));

        string hostname = Dns.GetHostName();

        IPAddress[] adArray = Dns.GetHostAddresses(hostname);
        Dictionary<string, string> adDict = new Dictionary<string, string>();

        adDict.Add("IP6", adArray[0].ToString());
        adDict.Add("IP4", adArray[1].ToString());
        
        foreach (IPAddress address in adArray)
        {
            //Debug.Log($"IP�A�h���X  {address.ToString()}");
        }
        return adDict;
    }

    public static async UniTask<string> GlobalIP()
    {
        string responseText = "�Ȃ�";

        using (UnityWebRequest request = new UnityWebRequest("https://ipinfo.io/ip", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3));

            try
            {
                await request.SendWebRequest();
            }
            catch
            {
                Debug.Log("�L���b�`");
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                        request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("�G���[");
                Debug.LogError(request.error);
                if (request.error == "HTTP/1.1 429 Too Many Requests")
                {
                    Debug.Log("���N�G�X�g������");
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    await GlobalIP();
                }
                Debug.Log("1-UseAPI");
                request.Dispose();
                Debug.Log("2-UseAPI");
                
                //throw new Exception();
            }
            else
            {
                responseText = request.downloadHandler.text;
                request.Dispose();
                Debug.Log($"IP�A�h���X���������I�@{responseText}");
            }
            return responseText;
        };
    }

}