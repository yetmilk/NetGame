using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlag : MonoBehaviour
{
    private void Start()
    {
        Camera camera = GetComponent<Camera>();
        CameraManager.Instance.SetCamera(camera);
    }
}
