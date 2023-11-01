using System.IO;
using System.Diagnostics;
using UnityEngine;
using System.Net;
using System.Text;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class GetGlobalIP : MonoBehaviourPunCallbacks
{
    private async void Start()
    {
        await UseAPI();
    }

    public static async UniTask<string> UseAPI()
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
                UnityEngine.Debug.Log("�L���b�`");
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                        request.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.Log("�G���[");
                UnityEngine.Debug.LogError(request.error);
                if (request.error == "HTTP/1.1 429 Too Many Requests")
                {
                    UnityEngine.Debug.Log("���N�G�X�g������");
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    await UseAPI();
                }
                UnityEngine.Debug.Log("1-UseAPI");
                request.Dispose();
                UnityEngine.Debug.Log("2-UseAPI");
                
                //throw new Exception();
            }
            else
            {
                responseText = request.downloadHandler.text;
                request.Dispose();
                UnityEngine.Debug.Log($"IP�A�h���X���������I�@{responseText}");
            }
            return responseText;
        };
    }

}