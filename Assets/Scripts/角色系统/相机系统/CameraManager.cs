using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 相机管理系统 - 处理相机的各种功能控制
/// </summary>
public class CameraManager : Singleton<CameraManager>
{

    // 相机引用
    public Camera mainCamera;
    private Transform cameraTransform;

    // 镜头缩放参数
    [Header("镜头缩放设置")]
    public float minZoomDistance = 2.0f;
    public float maxZoomDistance = 10.0f;
    public float currentZoomDistance;
    public float zoomSpeed = 5.0f;
    public Vector3 targetOffset; // 目标偏移量

    // 相机震动参数
    [Header("相机震动设置")]
    public float shakeMagnitude = 0.5f; // 震动幅度
    public float shakeDamping = 1.0f;   // 震动衰减
    private Vector3 originalPosition;
    private bool isShaking = false;

    // 平滑移动参数
    [Header("平滑移动设置")]
    public float smoothSpeed = 0.125f;
    private Vector3 velocity = Vector3.zero;

    private PlayerInputAction input;

    // 跟随目标
    public Transform followTarget;
    public bool isFollowing = false;

    protected override void Awake()
    {
        base.Awake();
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        cameraTransform = mainCamera.transform;
        originalPosition = cameraTransform.position;
        currentZoomDistance = maxZoomDistance; // 初始化为最大距离

        input = new PlayerInputAction();

        input.Enable();
    }

    public void Init(Transform followTarget)
    {
        this.followTarget = followTarget;
    }

    void Update()
    {
        // 处理镜头缩放
        HandleZoomInput();

        // 平滑跟随目标
        if (isFollowing && followTarget != null)
        {
            FollowTarget();
        }
    }
    private void FollowTarget()
    {
        if (followTarget != null)
        {
            Vector3 targetPosition = followTarget.position + targetOffset;
            targetPosition -= cameraTransform.forward * currentZoomDistance;
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                targetPosition,
                ref velocity,
                smoothSpeed
            );
        }
    }

    #region 镜头控制功能

    /// <summary>
    /// 处理镜头缩放输入
    /// </summary>
    private void HandleZoomInput()
    {
        // 鼠标滚轮缩放
        float scroll = input.GamePlay.MouseRoll.ReadValue<float>();
        if (scroll != 0)
        {
            currentZoomDistance -= scroll * zoomSpeed;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);

            // 缩放时的相机位置调整
            AdjustCameraPosition();
        }
    }

    /// <summary>
    /// 调整相机位置（基于缩放距离）
    /// </summary>
    public void AdjustCameraPosition()
    {
        if (followTarget != null)
        {
            // 如果有跟随目标，根据目标位置和缩放距离调整相机
            Vector3 targetPosition = followTarget.position + targetOffset;
            targetPosition -= cameraTransform.forward * currentZoomDistance;
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                targetPosition,
                ref velocity,
                smoothSpeed
            );
        }
    }

    /// <summary>
    /// 设置相机跟随目标
    /// </summary>
    public void SetFollowTarget(Transform target, Vector3 offset = default, bool startFollowing = true)
    {
        followTarget = target;
        targetOffset = offset;
        isFollowing = startFollowing;

        if (isFollowing)
        {
            AdjustCameraPosition();
        }
    }

    /// <summary>
    /// 停止跟随目标
    /// </summary>
    public void StopFollowingTarget()
    {
        isFollowing = false;
        followTarget = null;
    }

    #endregion

    #region 相机震动功能

    /// <summary>
    /// 触发相机震动
    /// </summary>
    /// <param name="intensity">震动强度</param>
    /// <param name="duration">震动持续时间</param>
    public void ShakeCamera(float intensity = .2f, float duration = 0.1f)
    {
        if (isShaking)
        {
            StopCoroutine("CameraShakeCoroutine");
        }

        shakeMagnitude = intensity;
        StartCoroutine("CameraShakeCoroutine", duration);
    }

    /// <summary>
    /// 相机震动协程
    /// </summary>
    private IEnumerator CameraShakeCoroutine(float duration)
    {
        isShaking = true;
        originalPosition = cameraTransform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            shakeMagnitude *= Mathf.Clamp01(1 - (elapsed * shakeDamping / duration));

            yield return null;
        }

        cameraTransform.position = originalPosition;
        isShaking = false;
    }

    #endregion

    #region 公共接口

    /// <summary>
    /// 立即设置相机位置
    /// </summary>
    public void SetCameraPosition(Vector3 position, bool smooth = true)
    {
        if (smooth)
        {
            StartCoroutine(SmoothMoveToPosition(position));
        }
        else
        {
            cameraTransform.position = position;
        }
    }

    /// <summary>
    /// 平滑移动到指定位置
    /// </summary>
    private IEnumerator SmoothMoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = cameraTransform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * smoothSpeed;
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cameraTransform.position = targetPosition;
    }

    /// <summary>
    /// 设置相机旋转
    /// </summary>
    public void SetCameraRotation(Quaternion rotation, bool smooth = true)
    {
        if (smooth)
        {
            StartCoroutine(SmoothRotateToRotation(rotation));
        }
        else
        {
            cameraTransform.rotation = rotation;
        }
    }

    /// <summary>
    /// 平滑旋转到指定角度
    /// </summary>
    private IEnumerator SmoothRotateToRotation(Quaternion targetRotation)
    {
        Quaternion startRotation = cameraTransform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * smoothSpeed;
            cameraTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        cameraTransform.rotation = targetRotation;
    }


    public void SetCamera(Camera camera)
    {
        this.mainCamera = camera;
        this.cameraTransform = camera.transform;
    }

    #endregion

    protected override void OnDestroy()
    {
        input.Disable();
    }
}