using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Linq;
//using ParrelSync;
//�g����
//1. ���̃X�N���v�g���A�^�b�`���܂��B
//2. myPort�͎����̃|�[�g�AopponentPort�͑���̃|�[�g�ł��B�e�X�g�ł�2�̃N���C�A���g�Ō��݂ɂȂ�悤�ɓ��͂��܂��B
//3. player�͎����̃L�����N�^�[�AotherPlayerObjects�͑���̃L�����N�^�[�ł��B���炩���ߗp�ӂ��ăA�^�b�`���܂��傤�B
//4. ParrelSync�Ȃǂ��g��2�̃G�f�B�^�[�𗧂��グ�܂�(�r���h�ƃG�f�B�^�[�Ńf�o�b�O������@������܂����AParrelSync�Ȃǂ��g�������������I�Ȃ̂ł��X�X���ł�)�B
//5. �e�G�f�B�^�[��myPort��opponentPort�����݂ɂȂ��Ă���̂��m�F���܂�(�N���C�A���gA��3000, 3001�Ȃ�N���C�A���gB�ł�3001, 3000)�B
//6. �����̃G�f�B�^�[�Ńv���C�{�^���������A�X�N���v�g��context menu����Register�������܂��B
//7. ������Register�������Ɠ������J�n���܂��BPlayer�I�u�W�F�N�g�𓮂����Ĉ���̃G�f�B�^�[��otherPlayer�������Ă��邩�m�F���܂��傤�B
//8. enableSmoothing��p����ƃt���[���Ԃ̈ʒu����`�⊮���܂��B���ʁA���炩�Ɍ����܂��B

public class Multiplayer : MonoBehaviour
{
    [SerializeField] int myPort;
    [SerializeField] int opponentPort;
    [SerializeField] bool enableSmoothing = false;
    [SerializeField] Transform player;
    [SerializeField] List<GameObject> otherPlayerObjects;

    //UDP�ʐM�n
    int sendPerSecond = 20;
    UdpClient client;
    Thread receiveThread;
    Thread sendThread;
    bool isSendTiming = false;
    List<IPEndPoint> ackWaiting = new List<IPEndPoint>(4);
    List<IPEndPoint> connectedPlayerEPs = new List<IPEndPoint>(4);
    List<ReceivedUnit> messageStack = new List<ReceivedUnit>(15);
    //�Q�[�����
    List<PositionAndRotation> otherPlayerInfo = new List<PositionAndRotation>(3);

    void Start()
    {
        client = new UdpClient(new IPEndPoint(IPAddress.Any, myPort));
        receiveThread = new Thread(new ThreadStart(ThreadReceive));
        receiveThread.Start();
    }

    public void RegisterOpponentPort(string IP = "192.168.88.169", int port = 0)
    {
        port = opponentPort;
        byte[] message = UDPMessage.Ack.ToByte();
        IPEndPoint opponentEP = new IPEndPoint(IPAddress.Parse(IP), port);
        client.Send(message, message.Length, opponentEP);
        ackWaiting.Add(opponentEP);
        print($"IP: {IP}:{port} �ɐڑ��v��");
    }
    [ContextMenu("Register")]
    public void OnClickRegister()
    {
        RegisterOpponentPort(port: opponentPort);
    }

    /// <summary>
    /// ��M�p�̃X���b�h�B��M�����ۂɏ����X�^�b�N�ɕۑ����Ă����B
    /// </summary>
    void ThreadReceive()
    {
        while (true)
        {
            IPEndPoint senderEP = null;
            byte[] receivedBytes = client.Receive(ref senderEP);

            Debug.Log($"�󂯎�������b�Z�[�W��: {receivedBytes.Length}");
            messageStack.Add(new ReceivedUnit(senderEP, receivedBytes));
        }
    }
    /// <summary>
    /// ���M�p�̃X���b�h�B���M�^�C�~���O��isSendTiming��true�ɂ���B
    /// </summary>
    void ThreadSend()
    {
        while (true)
        {
            //OnUpdateSend();
            isSendTiming = true;
            Thread.Sleep(1000 / sendPerSecond);
        }
    }
    /// <summary>
    /// ��M�������b�Z�[�W�̓��e�ɂ���ď������s��
    /// </summary>
    /// <param name="unit">��M���</param>
    void Parse(ReceivedUnit unit)
    {
        //�Ȃ�ׂ����b�Z�[�W���ƂP�֐��Ŏ�������
        UDPMessage type = unit.message.ToUDPMessage();
        int ackRegisteredIndex = ackWaiting.IndexOfPort(unit.senderEP.Port);
        int connectedIndex = connectedPlayerEPs.IndexOfPort(unit.senderEP.Port);
        print("���b�Z�[�W����M");
        switch (type)
        {
            case UDPMessage.Ack:
                {
                    print(ackRegisteredIndex);
                    if (ackRegisteredIndex == -1) break;
                    connectedPlayerEPs.Add(unit.senderEP);
                    ackWaiting.RemoveAt(ackRegisteredIndex);
                    if (connectedPlayerEPs.Count == 1)
                    {
                        sendThread = new Thread(new ThreadStart(ThreadSend));
                        sendThread.Start();
                    }
                    print("���̐l����ڑ�������܂���");
                    byte[] message = UDPMessage.AckComplete.ToByte();
                    client.SendAsync(message, message.Length, unit.senderEP);
                    otherPlayerObjects[connectedPlayerEPs.Count - 1].SetActive(true);
                    break;
                }
            case UDPMessage.AckComplete:
                {
                    print(ackRegisteredIndex);
                    if (ackRegisteredIndex == -1) break;
                    connectedPlayerEPs.Add(unit.senderEP);
                    ackWaiting.RemoveAt(ackRegisteredIndex);
                    if (connectedPlayerEPs.Count == 1)
                    {
                        sendThread = new Thread(new ThreadStart(ThreadSend));
                        sendThread.Start();
                    }
                    print("���̐l����ڑ�������܂���");
                    otherPlayerObjects[connectedPlayerEPs.Count - 1].SetActive(true);
                    break;
                }
            case UDPMessage.PosUpdate:
                {
                    Vector3 pos = unit.message.ToVector3(4);
                    float Yrot = BitConverter.ToSingle(unit.message, 16);
                    if (otherPlayerInfo.Count <= connectedIndex)
                    {
                        //�v���C���[��SmoothedMovement���Ȃ��ꍇ
                        PositionAndRotation newPlayer = new PositionAndRotation(sendPerSecond);
                        newPlayer.UpdateInformation(pos, Yrot);
                        otherPlayerInfo.Add(newPlayer);
                    }
                    else
                    {
                        otherPlayerInfo[connectedIndex].UpdateInformation(pos, Yrot);
                    }
                    break;
                }
            default:
                {
                    print("malformed packet!!!!!!!!!");
                    break;
                }
        }
    }
    /// <summary>
    /// �ʐM����S���Ɏ����̏�Ԃ𑗂�
    /// </summary>
    void BroadcastStatus()
    {
        //�ω����Ȃ��ꍇ����Ȃ�
        Vector3 posThisFlame = player.position;
        float YthisFlame = player.eulerAngles.y;

        //�f�[�^�����
        byte[] message = UDPMessage.PosUpdate.ToByte();
        message = message.Concat(posThisFlame.ToByte()).Concat(BitConverter.GetBytes(YthisFlame)).ToArray();
        for (int i = 0; i < connectedPlayerEPs.Count; i++)
        {
            client.SendAsync(message, message.Length, connectedPlayerEPs[i]);
        }
    }

    private void Update()
    {
        //���M�^�C�~���O
        if (isSendTiming)
        {
            BroadcastStatus();
            isSendTiming = false;
        }

        //��M���b�Z�[�W������ꍇ
        for (int i = 0; i < messageStack.Count; i++)
        {
            Parse(messageStack[i]);
            messageStack.RemoveAt(i);
            i--;
        }

        //�e�v���C���[�̈ʒu�A�b�v�f�[�g
        for (int i = 0; i < connectedPlayerEPs.Count; i++)
        {
            if (i >= otherPlayerInfo.Count) return;
            if (enableSmoothing)
            {
                otherPlayerInfo[i].UpdateTime();
                otherPlayerObjects[i].transform.position = otherPlayerInfo[i].GetLerpPosition();
                otherPlayerObjects[i].transform.eulerAngles = new Vector3(0, otherPlayerInfo[i].GetLerpRotation(), 0);
            }
            else
            {
                otherPlayerObjects[i].transform.position = otherPlayerInfo[i].GetPosition();
                otherPlayerObjects[i].transform.eulerAngles = new Vector3(0, otherPlayerInfo[i].GetRotation(), 0);
            }
        }
    }

    /// <summary>
    /// ��M���b�Z�[�W�̏���ۑ�
    /// </summary>
    class ReceivedUnit
    {
        public IPEndPoint senderEP;
        public byte[] message;

        public ReceivedUnit(IPEndPoint Ep, byte[] Message)
        {
            senderEP = Ep;
            message = Message;
        }
    }

    /// <summary>
    /// �X���b�h�Z���p
    /// ���ꂪ�Ȃ��ƘA�����ăv���C���Ƀ|�[�g�̃o�C���h����������Ă��Ȃ�
    /// </summary>
    private void OnApplicationQuit()
    {
        if (sendThread != null) sendThread.Abort();
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
    /// <summary>
    /// �I�����C����GameObject����肷���̕��@
    /// �^�O�Ō�����肵�Apos�ŃI�u�W�F�N�g����ɓ��肷��
    /// </summary>
    /// <param name="tag">���肷��I�u�W�F�N�g�̃^�O</param>
    /// <param name="pos">���肷��I�u�W�F�N�g�̈ʒu</param>
    /// <param name="threshold">�������Ƃ���ʒu��臒l�A��قǂ̂��Ƃ��Ȃ�����0���Ǝv����</param>
    /// <returns></returns>
    public GameObject GetGameObjectWithTagAndPostion(string tag, Vector3 pos, float threshold = .5f)
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject i in candidates)
        {
            if ((i.transform.position - pos).magnitude < threshold) return (i);
        }
        return null;
    }
    /// <summary>
    /// ��M����Ԃ̈ʒu�E��]��⊮���邽�߂̃N���X
    /// �Ⴆ�Γ������[�g��1�b��15��̏ꍇ60fps�ł�4�t���[���̊Ԏ~�܂��ďu�Ԉړ����J��Ԃ��悤�Ɍ�����B
    /// ���������ē������[�g��2�̒l��4���������4�t���[���̊Ԃ��ړ����Ă���悤�Ɍ����邱�Ƃ��ł���B
    /// </summary>
    class PositionAndRotation
    {
        public List<Vector3> position;
        public List<float> rot;
        public float timeFromLastInformation;
        float timeToNextInformation;

        public PositionAndRotation(int tickPerSecond)
        {
            position = new List<Vector3>();
            rot = new List<float>();
            timeFromLastInformation = 0;
            timeToNextInformation = 1f / tickPerSecond;
        }
        /// <summary>
        /// �⊮��������X�V����
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Yrot"></param>
        public void UpdateInformation(Vector3 pos, float Yrot)
        {
            timeFromLastInformation = 0;
            if (position.Count == 2) position.RemoveAt(0);
            position.Add(pos);
            if (rot.Count == 0) rot.Add(Yrot);
            else
            {
                //1,2 �̏ꍇ��臒l�𒴂��Ă�����X�V�A����ȊO�͑O�̃p�����[�^�[����
                float lastRot = rot[rot.Count - 1];
                if (Mathf.Abs(Yrot - lastRot) < 45) rot.Add(lastRot);
                else rot.Add(Yrot);
            }
            if (rot.Count == 3) rot.RemoveAt(0);
        }
        /// <summary>
        /// ���Ԃ��X�V����A����ɂ��⊮�̃^�C�~���O���i��
        /// </summary>
        public void UpdateTime()
        {
            timeFromLastInformation += Time.deltaTime;
        }
        /// <summary>
        /// �⊮���ꂽ�ʒu��ς���
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLerpPosition()
        {
            if (position.Count == 0) return Vector3.zero;
            else if (position.Count == 1) return position[0];
            return LerpPos(position[0], position[1], timeFromLastInformation / timeToNextInformation);
        }
        public Vector3 GetPosition()
        {
            if (position.Count == 0) return Vector3.zero;
            else if (position.Count == 1) return position[0];
            return position[1];
        }
        /// <summary>
        /// �⊮���ꂽ��]��Ԃ�
        /// </summary>
        /// <returns></returns>
        public float GetLerpRotation()
        {
            if (rot.Count == 0) return 0;
            else if (rot.Count == 1) return rot[0];
            return Mathf.LerpAngle(rot[0], rot[1], timeFromLastInformation / timeToNextInformation);
        }
        public float GetRotation()
        {
            if (rot.Count == 0) return 0;
            else if (rot.Count == 1) return rot[0];
            return rot[1];
        }
        /// <summary>
        /// Vector3����`�⊮����
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        Vector3 LerpPos(Vector3 from, Vector3 end, float a)
        {
            float x = Mathf.Lerp(from.x, end.x, a);
            float z = Mathf.Lerp(from.z, end.z, a);
            float y = Mathf.Lerp(from.y, end.y, a);
            return new Vector3(x, y, z);
        }
    }


}


enum UDPMessage
{
    Ack = 100001,
    AckComplete,
    PosUpdate,
    AnimUpdate,
    RightDoorOpen,
    LeftDoorOpen,
}

static class MultiPlayerExt
{
    public static byte[] ToByte(this UDPMessage udpm)
    {
        return BitConverter.GetBytes((int)udpm);
    }

    public static UDPMessage ToUDPMessage(this byte[] b, int startIndex = 0)
    {
        int number = BitConverter.ToInt32(b, startIndex);
        return (UDPMessage)Enum.ToObject(typeof(UDPMessage), number);
    }
    public static byte[] ToByte(this Vector3 v)
    {
        byte[] x = BitConverter.GetBytes(v.x);
        byte[] y = BitConverter.GetBytes(v.y);
        byte[] z = BitConverter.GetBytes(v.z);
        return x.Concat(y).Concat(z).ToArray();
    }

    public static Vector3 ToVector3(this byte[] b, int startIndex)
    {
        float x = BitConverter.ToSingle(b, startIndex);
        float y = BitConverter.ToSingle(b, startIndex + 4);
        float z = BitConverter.ToSingle(b, startIndex + 8);
        return new Vector3(x, y, z);
    }
    public static int IndexOfPort(this List<IPEndPoint> eps, int targetPort)
    {
        int index = -1;
        for (int i = 0; i < eps.Count; i++)
        {
            if (eps[i].Port == targetPort) index = i;
        }
        return index;
    }

}