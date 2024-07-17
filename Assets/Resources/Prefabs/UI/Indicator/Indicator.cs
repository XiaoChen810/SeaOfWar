using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    // 摄像机的引用
    public Camera mainCamera;

    void Start()
    {
        // 如果未在Inspector中指定摄像机，自动获取主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // 使指示器朝向摄像机
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}

