using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class CameraControl : MonoBehaviour
{
    public static string data;
    public static string resMsg;
    public static string resMsg1;
    public static string resMsg2;
    public static string resMsg3;
    System.Net.Sockets.NetworkStream ns;
    System.Net.Sockets.TcpClient tcp;

    Vector3 pos = new Vector3(0, 0, 0);
    Vector3 rot = new Vector3(0, 0, 0);

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
            //resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            //Debug.Log((int)ms.Length);
            //ms.Close();
            //resMsg = resMsg.TrimEnd('\n');

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
            // string text = System.Text.Encoding.UTF8.GetString(img);
            // WriteText(text);

            ms.Close();
            // パース
            // string[] separatingStrings = {"    "};
            // string[] arr = resMsg2.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            string[] arr = resMsg2.Split();

            Debug.Log(arr.Length);
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

                //Debug.Log(arr[6]);
                //WriteText(arr[6]);
            }
        }
        catch (FormatException e)
        {
            Debug.Log(e);
        }
        finally
        {
            Debug.Log("finally");
        }
    }
}