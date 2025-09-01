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

    // 新增：旋转处理遮挡参数
    [Header("遮挡旋转设置")]
    public float minXRotation = 10f; // 最小X轴旋转角度（向上限制）
    public float maxXRotation = 60f; // 最大X轴旋转角度（向下限制）
    public float defaultXRotation = 30f; // 默认X轴旋转角度
    public float currentXRotation; // 当前X轴旋转角度
    public float rotationSpeed = 5f; // 旋转插值速度
    public LayerMask occluderLayers; // 遮挡物层级

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
        currentXRotation = defaultXRotation; // 初始化旋转角度

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

        // 平滑跟随目标并处理遮挡
        if (isFollowing && followTarget != null)
        {
            HandleOcclusion(); // 处理遮挡
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (followTarget != null)
        {
            // 应用当前X轴旋转角度（Y轴保持不变）
            cameraTransform.rotation = Quaternion.Euler(currentXRotation, cameraTransform.eulerAngles.y, 0);

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

    #region 遮挡处理逻辑
    /// <summary>
    /// 处理遮挡物：通过调整X轴旋转避免遮挡
    /// </summary>
    private void HandleOcclusion()
    {
        if (followTarget == null) return;

        Vector3 targetPoint = followTarget.position + targetOffset;
        Vector3 currentCameraPos = cameraTransform.position;

        // 先检查当前路径是否有遮挡
        bool isBlocked = IsPathBlockedByRay(targetPoint, currentCameraPos);

        // 如果当前有遮挡，尝试找到最佳角度
        if (isBlocked)
        {
            float bestRotation = FindBestRotationAngle(targetPoint);
            currentXRotation = Mathf.Lerp(currentXRotation, bestRotation, Time.deltaTime * rotationSpeed);
            currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);
        }
        else
        {
            // 无遮挡时，先检查默认角度是否可用
            Vector3 defaultAnglePos = CalculateCameraPosition(targetPoint, defaultXRotation);
            bool isDefaultAngleBlocked = IsPathBlockedByRay(targetPoint, defaultAnglePos);

            // 只有默认角度无遮挡时才恢复，否则保持当前有效角度
            if (!isDefaultAngleBlocked)
            {
                currentXRotation = Mathf.Lerp(currentXRotation, defaultXRotation, Time.deltaTime * rotationSpeed);
                currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);
            }
        }
    }

    /// <summary>
    /// 寻找最佳旋转角度（无遮挡）
    /// </summary>
    private float FindBestRotationAngle(Vector3 targetPoint)
    {
        // 先尝试向上旋转（减小X轴角度）- 更自然的遮挡规避方向
        for (float angle = currentXRotation; angle >= minXRotation; angle -= 0.5f)
        {
            Vector3 testPos = CalculateCameraPosition(targetPoint, angle);
            if (!IsPathBlockedByRay(targetPoint, testPos))
            {
                return angle;
            }
        }

        // 再尝试向下旋转（增大X轴角度）
        for (float angle = currentXRotation; angle <= maxXRotation; angle += 0.5f)
        {
            Vector3 testPos = CalculateCameraPosition(targetPoint, angle);
            if (!IsPathBlockedByRay(targetPoint, testPos))
            {
                return angle;
            }
        }

        // 如果所有角度都有遮挡，保持当前角度
        return currentXRotation;
    }

    /// <summary>
    /// 计算基于指定旋转角度的相机位置
    /// </summary>
    private Vector3 CalculateCameraPosition(Vector3 targetPoint, float xRotation)
    {
        Quaternion tempRotation = Quaternion.Euler(xRotation, cameraTransform.eulerAngles.y, 0);
        return targetPoint - tempRotation * Vector3.forward * currentZoomDistance;
    }

    /// <summary>
    /// 射线检测两点之间是否有遮挡
    /// </summary>
    private bool IsPathBlockedByRay(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // 射线检测（忽略目标和相机自身）
        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, occluderLayers))
        {
            // 忽略跟随目标和相机自身的碰撞体
            if (hit.collider.transform == followTarget || hit.collider.transform == cameraTransform)
                return false;

            return true;
        }
        return false;
    }
    #endregion

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
    /// 调整相机位置（基于缩放距离和旋转角度）
    /// </summary>
    public void AdjustCameraPosition()
    {
        if (followTarget != null)
        {
            // 应用当前旋转角度
            cameraTransform.rotation = Quaternion.Euler(currentXRotation, cameraTransform.eulerAngles.y, 0);

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
            currentXRotation = rotation.eulerAngles.x; // 同步当前旋转角度
        }
    }

    /// <summary>
    /// 平滑旋转到指定角度
    /// </summary>
    private IEnumerator SmoothRotateToRotation(Quaternion targetRotation)
    {
        Quaternion startRotation = cameraTransform.rotation;
        float targetX = targetRotation.eulerAngles.x;
        float startX = currentXRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * smoothSpeed;
            currentXRotation = Mathf.Lerp(startX, targetX, t);
            currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);
            cameraTransform.rotation = Quaternion.Euler(currentXRotation, cameraTransform.eulerAngles.y, 0);
            yield return null;
        }

        cameraTransform.rotation = targetRotation;
        currentXRotation = targetX;
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