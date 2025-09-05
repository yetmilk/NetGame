using UnityEngine;
using System.Collections.Generic;
using System;

public class CollisionDetector : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("��Ҫ����Ŀ��ͼ��")]
    public LayerMask targetLayers = ~0;
    [Tooltip("�����ʱ��(��)��0Ϊÿ֡���")]
    public float detectInterval = 0;

    [Header("�����״ѡ��")]
    [Tooltip("�������μ��")]
    public bool useSphere = true;
    [Tooltip("���ú��μ��")]
    public bool useBox = false;
    [Tooltip("�������߼��")]
    public bool useRay = false;

    [Header("���μ�����")]
    public float sphereRadius = 0.5f;

    [Header("���μ�����")]
    public Vector3 boxSize = new Vector3(1, 1, 1);
    public Vector3 boxCenter = Vector3.zero;
    public Vector3 boxRotation = Vector3.zero;

    [Header("���߼�����")]
    public Vector3 rayDirection = Vector3.forward;
    public float rayDistance = 5f;
    public float rayWidth = 0f; // 0Ϊ�����ߣ�>0Ϊ����������

    // ��ײ������ݽṹ
    public class DetectionInfo
    {
        public Collider target;
        public Vector3 collisionPoint;
        public Vector3 collisionNormal;

        public DetectionInfo(Collider target, Vector3 point, Vector3 normal)
        {
            this.target = target;
            collisionPoint = point;
            collisionNormal = normal;
        }
    }

    // ʵʱ����б�
    public List<DetectionInfo> CurrentDetections { get; private set; } = new List<DetectionInfo>();

    // ����б�仯�¼�
    public event Action<List<DetectionInfo>> OnDetectionUpdated;

    // �ڲ�״̬����
    private HashSet<Collider> _processedColliders = new HashSet<Collider>();
    private List<Collider> _ignoreList = new List<Collider>();
    private bool _isInitialized = false;
    private float _lastDetectTime;

    /// <summary>
    /// ��ʼ�������
    /// </summary>
    public void Init(GameObject owner, List<Collider> ignoreColliders = null)
    {
        _ignoreList.Clear();
        if (owner != null)
        {
            _ignoreList.AddRange(owner.GetComponents<Collider>());
        }
        if (ignoreColliders != null)
            _ignoreList.AddRange(ignoreColliders);

        _isInitialized = true;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void ClearDetectionResults()
    {
        CurrentDetections.Clear();

        OnDetectionUpdated?.Invoke(CurrentDetections);
    }
    public void ResetDetection()
    {
        _processedColliders.Clear();
    }

    private void Update()
    {
        if (!_isInitialized) return;


    }

    /// <summary>
    /// ִ����ײ���
    /// </summary>
    public List<DetectionInfo> PerformDetection()
    {

        ClearDetectionResults();

        if (useSphere)
            DetectSphere();

        if (useBox)
            DetectBox();

        if (useRay)
            DetectRay();

        return CurrentDetections;
    }

    #region ��״���ʵ��
    private void DetectSphere()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, targetLayers);
        ProcessColliders(colliders, DetectionShape.Sphere);
    }

    private void DetectBox()
    {
        Quaternion rotation = Quaternion.Euler(boxRotation);
        Collider[] colliders = Physics.OverlapBox(
            transform.TransformPoint(boxCenter),
            boxSize / 2,
            rotation * transform.rotation,
            targetLayers
        );
        ProcessColliders(colliders, DetectionShape.Box);
    }

    private void DetectRay()
    {
        if (rayWidth <= 0)
        {
            // �����߼��
            if (Physics.Raycast(transform.position, transform.TransformDirection(rayDirection),
                out RaycastHit hit, rayDistance, targetLayers))
            {
                ProcessRayHit(hit);
            }
        }
        else
        {
            // ����������(������)���
            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.TransformDirection(rayDirection) * rayDistance;
            RaycastHit[] hits = Physics.CapsuleCastAll(start, end, rayWidth / 2,
                transform.TransformDirection(rayDirection), 0, targetLayers);

            foreach (var hit in hits)
            {
                ProcessRayHit(hit);
            }
        }
    }

    private void ProcessColliders(Collider[] colliders, DetectionShape shape)
    {
        foreach (var collider in colliders)
        {
            if (_ignoreList.Contains(collider) || _processedColliders.Contains(collider))
                continue;

            if (TryGetCollisionInfo(collider, shape, out Vector3 point, out Vector3 normal))
            {
                _processedColliders.Add(collider);
                CurrentDetections.Add(new DetectionInfo(collider, point, normal));
            }
        }
    }

    private void ProcessRayHit(RaycastHit hit)
    {
        if (_ignoreList.Contains(hit.collider) || _processedColliders.Contains(hit.collider))
            return;

        _processedColliders.Add(hit.collider);
        CurrentDetections.Add(new DetectionInfo(hit.collider, hit.point, hit.normal));
    }
    #endregion

    #region ��������
    private bool IsLayerIncluded(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private bool TryGetCollisionInfo(Collider other, DetectionShape shape, out Vector3 point, out Vector3 normal)
    {
        point = Vector3.zero;
        normal = Vector3.zero;

        // ���ݲ�ͬ��״��ȡ��ȷ��ײ��
        switch (shape)
        {
            case DetectionShape.Sphere:
                return GetSphereCollisionInfo(other, out point, out normal);
            case DetectionShape.Box:
                return GetBoxCollisionInfo(other, out point, out normal);
            default:
                return false;
        }
    }

    private bool GetSphereCollisionInfo(Collider other, out Vector3 point, out Vector3 normal)
    {
        point = other.bounds.ClosestPoint(transform.position);
        normal = (transform.position - point).normalized;
        return true;
    }

    private bool GetBoxCollisionInfo(Collider other, out Vector3 point, out Vector3 normal)
    {
        Quaternion rotation = Quaternion.Euler(boxRotation) * transform.rotation;
        Vector3 center = transform.TransformPoint(boxCenter);

        point = other.bounds.ClosestPoint(center);
        normal = (center - point).normalized;
        return true;
    }

    private enum DetectionShape
    {
        Sphere,
        Box,
        Ray
    }
    #endregion

    #region Gizmo����
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // �������μ�ⷶΧ
        if (useSphere)
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }

        // ���ƺ��μ�ⷶΧ
        if (useBox)
        {
            Gizmos.matrix = Matrix4x4.TRS(
                transform.TransformPoint(boxCenter),
                Quaternion.Euler(boxRotation) * transform.rotation,
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = Matrix4x4.identity; // ���þ���
        }

        // �������߼�ⷶΧ
        if (useRay)
        {
            Vector3 direction = transform.TransformDirection(rayDirection);
            Vector3 endPoint = transform.position + direction * rayDistance;

            if (rayWidth <= 0)
            {
                // ������
                Gizmos.DrawLine(transform.position, endPoint);
                Gizmos.DrawWireSphere(endPoint, 0.1f); // �����յ���
            }
            else
            {
                // ����������
                Gizmos.DrawWireSphere(transform.position, rayWidth / 2);
                Gizmos.DrawWireSphere(endPoint, rayWidth / 2);
                Gizmos.DrawLine(
                    transform.position + direction * (rayWidth / 2),
                    endPoint - direction * (rayWidth / 2)
                );
                Gizmos.DrawLine(
                    transform.position - direction * (rayWidth / 2),
                    endPoint + direction * (rayWidth / 2)
                );
            }
        }

        // ���Ƶ�ǰ��⵽����ײ��
        Gizmos.color = Color.red;
        foreach (var detection in CurrentDetections)
        {
            Gizmos.DrawWireSphere(detection.collisionPoint, 0.15f);
            Gizmos.DrawRay(detection.collisionPoint, detection.collisionNormal * 0.5f);
        }
    }
    #endregion
}