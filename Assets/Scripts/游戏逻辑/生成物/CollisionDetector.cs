using UnityEngine;
using System.Collections.Generic;
using System;

public class CollisionDetector : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("需要检测的目标图层")]
    public LayerMask targetLayers = ~0;
    [Tooltip("检测间隔时间(秒)，0为每帧检测")]
    public float detectInterval = 0;

    [Header("检测形状选择")]
    [Tooltip("启用球形检测")]
    public bool useSphere = true;
    [Tooltip("启用盒形检测")]
    public bool useBox = false;
    [Tooltip("启用射线检测")]
    public bool useRay = false;

    [Header("球形检测参数")]
    public float sphereRadius = 0.5f;

    [Header("盒形检测参数")]
    public Vector3 boxSize = new Vector3(1, 1, 1);
    public Vector3 boxCenter = Vector3.zero;
    public Vector3 boxRotation = Vector3.zero;

    [Header("射线检测参数")]
    public Vector3 rayDirection = Vector3.forward;
    public float rayDistance = 5f;
    public float rayWidth = 0f; // 0为线射线，>0为胶囊体射线

    // 碰撞检测数据结构
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

    // 实时检测列表
    public List<DetectionInfo> CurrentDetections { get; private set; } = new List<DetectionInfo>();

    // 检测列表变化事件
    public event Action<List<DetectionInfo>> OnDetectionUpdated;

    // 内部状态变量
    private HashSet<Collider> _processedColliders = new HashSet<Collider>();
    private List<Collider> _ignoreList = new List<Collider>();
    private bool _isInitialized = false;
    private float _lastDetectTime;

    /// <summary>
    /// 初始化检测器
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
    /// 清除检测结果
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
    /// 执行碰撞检测
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

    #region 形状检测实现
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
            // 线射线检测
            if (Physics.Raycast(transform.position, transform.TransformDirection(rayDirection),
                out RaycastHit hit, rayDistance, targetLayers))
            {
                ProcessRayHit(hit);
            }
        }
        else
        {
            // 胶囊体射线(宽射线)检测
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

    #region 辅助方法
    private bool IsLayerIncluded(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private bool TryGetCollisionInfo(Collider other, DetectionShape shape, out Vector3 point, out Vector3 normal)
    {
        point = Vector3.zero;
        normal = Vector3.zero;

        // 根据不同形状获取精确碰撞点
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

    #region Gizmo绘制
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // 绘制球形检测范围
        if (useSphere)
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }

        // 绘制盒形检测范围
        if (useBox)
        {
            Gizmos.matrix = Matrix4x4.TRS(
                transform.TransformPoint(boxCenter),
                Quaternion.Euler(boxRotation) * transform.rotation,
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = Matrix4x4.identity; // 重置矩阵
        }

        // 绘制射线检测范围
        if (useRay)
        {
            Vector3 direction = transform.TransformDirection(rayDirection);
            Vector3 endPoint = transform.position + direction * rayDistance;

            if (rayWidth <= 0)
            {
                // 线射线
                Gizmos.DrawLine(transform.position, endPoint);
                Gizmos.DrawWireSphere(endPoint, 0.1f); // 射线终点标记
            }
            else
            {
                // 胶囊体射线
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

        // 绘制当前检测到的碰撞点
        Gizmos.color = Color.red;
        foreach (var detection in CurrentDetections)
        {
            Gizmos.DrawWireSphere(detection.collisionPoint, 0.15f);
            Gizmos.DrawRay(detection.collisionPoint, detection.collisionNormal * 0.5f);
        }
    }
    #endregion
}