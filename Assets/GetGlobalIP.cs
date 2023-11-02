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
            //Debug.Log($"IPアドレス  {address.ToString()}");
        }
        return adDict;
    }

    public static async UniTask<string> GlobalIP()
    {
        string responseText = "なし";

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
                Debug.Log("キャッチ");
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                        request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("エラー");
                Debug.LogError(request.error);
                if (request.error == "HTTP/1.1 429 Too Many Requests")
                {
                    Debug.Log("リクエスト多すぎ");
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
                Debug.Log($"IPアドレス分かった！　{responseText}");
            }
            return responseText;
        };
    }

}