using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static string data;
    public static string resMsg;
    System.Net.Sockets.NetworkStream ns;
    System.Net.Sockets.TcpClient tcp;

    Vector3 pos = new Vector3(0, 0, 0);
    Vector3 rot = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start()
    {
        string ipOrHost = "localhost";
        int port = 5900;

        //TcpClientを作成し、サーバーと接続
        tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
        Debug.Log("サーバー({0}:{1})と接続しました。" +
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address + "," +
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port + "," +
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address + "," +
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);

        //NetworkStreamを取得
        ns = tcp.GetStream();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            data = Time.time.ToString();

            //送信
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(data + '\n');
            ns.Write(sendBytes, 0, sendBytes.Length);

            //受信
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 256;
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            ms.Write(resBytes, 0, resSize);
            resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();
            resMsg = resMsg.TrimEnd('\n');

            // パース
            // string[] separatingStrings = {"    "};
            // string[] arr = resMsg.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            string[] arr = resMsg.Split();


            // Debug.Log(arr.Length);
            // Debug.Log(arr[0]);
            if (arr.Length == 6)
            {
                pos.x = -float.Parse(arr[0]);
                pos.y = float.Parse(arr[1]);
                pos.z = float.Parse(arr[2]);
                rot.x = float.Parse(arr[3]);
                rot.y = -float.Parse(arr[4]);
                rot.z = -float.Parse(arr[5]) + 180.0f;
                transform.position = pos;
                transform.rotation = Quaternion.Euler(rot);
            }
            // System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            // byte[] resBytes2 = new byte[10000000];
            // int resSize2 = 10000000;
            // resSize2 = ns.Read(resBytes2, 0, resBytes2.Length);
            // ms2.Write(resBytes2, 0, resSize2);
            // resMsg = enc.GetString(ms2.GetBuffer(), 0, (int)ms2.Length);
            // ms2.Close();
            // resMsg = resMsg.TrimEnd('\n');
        }
        finally
        {
            Debug.Log("?????");
        }
    }
}