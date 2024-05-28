using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using LumiSoft.Net.STUN.Client;

public class Test_SynchronizedValue : MonoBehaviour
{
    public int BindPort = 60570;
    public bool isGlobal;

    private IPEndPoint MyIPEndPoint;

    [SerializeField]
    private TextMeshProUGUI ChatLog;
    [SerializeField]
    private Button ButtonEnterChat;
    [SerializeField]
    private TMP_InputField InputChat;
    [SerializeField]
    private TMP_InputField InputAddress;
    [SerializeField]
    private TMP_InputField InputPort;
    [SerializeField]
    private Button ButtonEnterAddress;

    [SerializeField]
    private TextMeshProUGUI MyEndPoint;

    private UdpClient UdpClient;

    private IPEndPoint ConnectIPEndPoint = new IPEndPoint(0, 0);

    [SerializeField]
    private bool isConnect;

    private async void Start()
    {
        UdpClient = new UdpClient(BindPort);

        if (!isGlobal)
        {
            Debug.Log("Is Local");
            //Private IP ������i�@�������傢�������@����
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IPv6HopByHopOptions))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                MyIPEndPoint = endPoint;
                MyIPEndPoint.Port = BindPort;

                Debug.Log(MyIPEndPoint);
                MyEndPoint.text = MyIPEndPoint.ToString();
            }
        }

        if (isGlobal)
        {
            //STUN(UdpClient);
        }


        ButtonEnterAddress.onClick.AddListener(Connect);
        ButtonEnterChat.onClick.AddListener(Send);
        await ReceiveLoop();
    }

    private void STUN(UdpClient client)
    {
        // Query STUN server
        //Google��STUN�T�[�o�[���玩���̃O���[�o��IP�A�h���X��NAPT�ŕϊ����ꂽ�|�[�g���擾����
        STUN_Result result = STUN_Client.Query("stun2.l.google.com", 19302, client.Client);
        //���������̃l�b�g����UDP���u���b�N����ꍇ
        if (result.NetType == STUN_NetType.UdpBlocked)
        {
            Debug.LogError("UDP blocked");
        }
        else
        {
            //�擾�ɐ��������ꍇ
            Debug.Log(result.NetType.ToString());
            IPEndPoint publicEP = result.PublicEndPoint;
            Debug.Log("Global IP:" + publicEP.Address);
            Debug.Log("NAT Port:" + publicEP.Port);
            MyIPEndPoint = publicEP;
        }
    }

    private async Task ReceiveLoop()
    {
        for (; ; )
        {
            var result = await UdpClient.ReceiveAsync();
            //�ڑ���ȊO�̃p�P�b�g������Ă�����u���b�N����
            if (result.RemoteEndPoint.Address != ConnectIPEndPoint.Address)
            {
                //�ڑ��v���ł���΂��̂܂܋�����
                if (Encoding.UTF8.GetString(result.Buffer) == "e:Connect")
                {
                    ConnectIPEndPoint = result.RemoteEndPoint;
                    ChatLog.text += "\n Connect Request";
                    var bytesData = Encoding.UTF8.GetBytes("e:Accept");
                    UdpClient.Send(bytesData, bytesData.Length, ConnectIPEndPoint);
                    isConnect = true;
                }
                // �ڑ����ł���΂��̂܂ܒʐM���͂��߂�
                else if (Encoding.UTF8.GetString(result.Buffer) == "e:Accept")
                {
                    ConnectIPEndPoint = result.RemoteEndPoint;
                    ChatLog.text += "\n Connect Start";
                    isConnect = true;
                }
                else
                {
                    Debug.Log("Block");
                    ReceiveCallback(result.Buffer);
                }
            }
            else
            {
                Debug.Log(result.RemoteEndPoint.ToString());
                ReceiveCallback(result.Buffer);
            }
        }
    }

    private async void Connect()
    {
        while (!isConnect)
        {
            Debug.Log("Connecting...");
            var bytesData = Encoding.UTF8.GetBytes("e:Connect");
            UdpClient.Send(bytesData, bytesData.Length, new IPEndPoint(IPAddress.Parse(InputAddress.text), int.Parse(InputPort.text)));
            await Task.Delay(1000);
        }
    }

    public async void Send()
    {
        var bytesData = Encoding.UTF8.GetBytes(InputChat.text);
        await UdpClient.SendAsync(bytesData, bytesData.Length, ConnectIPEndPoint);
        Debug.Log("Send:" + InputChat.text);
        ChatLog.text += "\n My:" + InputChat.text;
        InputChat.text = "";
    }

    public void ReceiveCallback(byte[] receiveBytes)
    {
        string receiveString = Encoding.UTF8.GetString(receiveBytes);
        if (receiveString == null || receiveString == "")
        {
            return;
        }
        if (receiveString == "e:Accept")
        {
            Debug.Log("Accept: " + receiveString);
            ChatLog.text += "\n Connect Accept";
        }
        Debug.Log("Received: " + receiveString);

        ChatLog.text += "\n Other:" + receiveString;
    }
}
