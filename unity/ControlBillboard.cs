using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBillboard : MonoBehaviour
{
    Camera mainCamera;

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
    }
}
