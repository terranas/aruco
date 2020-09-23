using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class CameraControl : MonoBehaviour
{
    static string data;
    static string resMsg1;
    static string resMsg2;
    System.Net.Sockets.NetworkStream ns;
    System.Net.Sockets.TcpClient tcp;

    Vector3 pos = new Vector3(0, 0, 0);
    Vector3 rot = new Vector3(0, 0, 0);
    int t = 0;

    public bool flag = true;

    public byte[] img = new byte[8];

    public void WriteText(string txt)
    {
        StreamWriter sw = new StreamWriter("./LogData.txt", false); //true=追記 false=上書き
        sw.WriteLine(txt);
        sw.Flush();
        sw.Close();
    }

    // Use this for initialization
    void Start()
    {
        string ipOrHost = "localhost";
        int port = 5900;

        //TcpClientを作成し、サーバーと接続
        tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
        Debug.Log("接続しました。" +
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
            data = t.ToString();
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            //送信
            byte[] sendBytes = enc.GetBytes(data + '\n');
            ns.Write(sendBytes, 0, sendBytes.Length);

            //受信
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[1048576];
            int resSize = 1048576;
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            ms.Write(resBytes, 0, resSize);

            // get num
            resMsg1 = enc.GetString(ms.GetBuffer(), 0, 8);
            int num = int.Parse(resMsg1);

            // get XYZRPY
            resMsg2 = enc.GetString(ms.GetBuffer(), 8, num);

            // get img
            byte[] temp = ms.GetBuffer(); //, 8+num, (int)ms.Length-(8+num));
            img = new byte[(int)ms.Length - (8 + num)];
            Array.Copy(temp, 8 + num, img, 0, (int)ms.Length - (8 + num));
            File.WriteAllBytes("./img.png", img);

            ms.Close();
            string[] arr = resMsg2.Split();

            // Debug.Log(arr.Length);
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
            flag = true;
        }
        catch (FormatException e)
        {
            Debug.Log(e);
        }
        catch (OverflowException e)
        {
            Debug.Log(e);
        }
    }
}