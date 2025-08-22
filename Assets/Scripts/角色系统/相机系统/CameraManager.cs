using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// �������ϵͳ - ��������ĸ��ֹ��ܿ���
/// </summary>
public class CameraManager : Singleton<CameraManager>
{

    // �������
    public Camera mainCamera;
    private Transform cameraTransform;

    // ��ͷ���Ų���
    [Header("��ͷ��������")]
    public float minZoomDistance = 2.0f;
    public float maxZoomDistance = 10.0f;
    public float currentZoomDistance;
    public float zoomSpeed = 5.0f;
    public Vector3 targetOffset; // Ŀ��ƫ����

    // ����𶯲���
    [Header("���������")]
    public float shakeMagnitude = 0.5f; // �𶯷���
    public float shakeDamping = 1.0f;   // ��˥��
    private Vector3 originalPosition;
    private bool isShaking = false;

    // ƽ���ƶ�����
    [Header("ƽ���ƶ�����")]
    public float smoothSpeed = 0.125f;
    private Vector3 velocity = Vector3.zero;

    private PlayerInputAction input;

    // ����Ŀ��
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
        currentZoomDistance = maxZoomDistance; // ��ʼ��Ϊ������

        input = new PlayerInputAction();

        input.Enable();
    }

    public void Init(Transform followTarget)
    {
        this.followTarget = followTarget;
    }

    void Update()
    {
        // ����ͷ����
        HandleZoomInput();

        // ƽ������Ŀ��
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

    #region ��ͷ���ƹ���

    /// <summary>
    /// ����ͷ��������
    /// </summary>
    private void HandleZoomInput()
    {
        // ����������
        float scroll = input.GamePlay.MouseRoll.ReadValue<float>();
        if (scroll != 0)
        {
            currentZoomDistance -= scroll * zoomSpeed;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);

            // ����ʱ�����λ�õ���
            AdjustCameraPosition();
        }
    }

    /// <summary>
    /// �������λ�ã��������ž��룩
    /// </summary>
    public void AdjustCameraPosition()
    {
        if (followTarget != null)
        {
            // ����и���Ŀ�꣬����Ŀ��λ�ú����ž���������
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
    /// �����������Ŀ��
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
    /// ֹͣ����Ŀ��
    /// </summary>
    public void StopFollowingTarget()
    {
        isFollowing = false;
        followTarget = null;
    }

    #endregion

    #region ����𶯹���

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="intensity">��ǿ��</param>
    /// <param name="duration">�𶯳���ʱ��</param>
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
    /// �����Э��
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

    #region �����ӿ�

    /// <summary>
    /// �����������λ��
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
    /// ƽ���ƶ���ָ��λ��
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
    /// ���������ת
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
    /// ƽ����ת��ָ���Ƕ�
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