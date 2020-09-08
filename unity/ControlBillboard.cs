using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControlBillboard : MonoBehaviour
{
    Camera mainCamera;


    byte[] ReadFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = this.mainCamera.transform.rotation;
        transform.Rotate(new Vector3(90, 0, 180));

        byte[] readBinary = ReadFile("./img.png");
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(readBinary);

        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
