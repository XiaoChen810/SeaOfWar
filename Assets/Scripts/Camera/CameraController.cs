using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public float speed = 5; // 摄像机移动速度
    public float zoomSpeed = 10; // 视野缩放速度
    public float minZoom = 5; // 最小视野
    public float maxZoom = 50; // 最大视野

    public float dragSpeed = 2; // 拖动速度
    public float rotateSpeed = 5; // 旋转速度
    public float smoothTime = 0.2f; // 平滑时间
    public Vector2 boundaryX = new Vector2(-50, 50); // X轴边界
    public Vector2 boundaryZ = new Vector2(-50, 50); // Z轴边界

    private Camera _camera;
    private Plane _plane;

    private Vector3 dragOrigin;
    private Vector3 velocity = Vector3.zero;
    private bool isFocusing = false;


    private void Update()
    {
        if (!isFocusing)
        {
            MoveCameraByKeys();
            ZoomCamera();
            DragCamera();
        }

        // 限制摄像机位置在边界内
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, boundaryX.x, boundaryX.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, boundaryZ.x, boundaryZ.y);
        transform.position = clampedPosition;
    }

    private void MoveCameraByKeys()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = transform.right * horizontal + transform.forward * vertical;
            direction.y = 0; // 防止摄像机在Y轴上移动
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.fieldOfView -= scroll * zoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
        }
    }

    private void DragCamera()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

            transform.position -= move;
            dragOrigin = Input.mousePosition;
        }
    }

    private void Start()
    {
        _camera = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
    }

    public void FocusOnTarget(Transform target)
    {
        FocusOnTarget(target.position);
    }

    public void FocusOnTarget(Vector3 pos)
    {
        var cp = CalcScreenCenterPosOnPanel();
        var tp = pos;
        MoveCamera(cp, tp);
    }

    private Vector3 CalcScreenCenterPosOnPanel()
    {
        var ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (_plane.Raycast(ray, out var distance))
        {
            return ray.GetPoint(distance);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void MoveCamera(Vector3 cp, Vector3 tp)
    {
        _camera.transform.DOMove(_camera.transform.position + (tp - cp), 0.5f);
    }

}