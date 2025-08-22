using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public static class MouseUtility
    {
        // 缓存用于 UI 射线检测的参数（避免频繁创建）
        private static PointerEventData s_PointerEventData;
        private static List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

        /// <summary>
        /// 获取指定屏幕位置对应的3D物体表面坐标，会被UI遮挡
        /// </summary>
        public static Vector3 GetClickedPosition(Vector3 screenPosition, string layerMaskName, out bool checkSucc)
        {
            // 手动进行UI射线检测（当前帧状态）
            if (IsPointerOverUI(screenPosition))
            {
                checkSucc = false;
                return Vector3.zero;
            }

            int layerMask = string.IsNullOrEmpty(layerMaskName) ? -1 : LayerMask.GetMask(layerMaskName);


            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("主相机不存在！");
                checkSucc = false;
                return Vector3.zero;
            }

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                checkSucc = true;
                return hit.point;
            }

            checkSucc = false;
            return Vector3.zero;
        }

        /// <summary>
        /// 检测指定屏幕位置是否在UI上（当前帧状态）
        /// </summary>
        private static bool IsPointerOverUI(Vector3 screenPosition)
        {
            // 初始化事件数据（只创建一次）
            if (s_PointerEventData == null)
            {
                s_PointerEventData = new PointerEventData(EventSystem.current);
            }

            // 设置当前鼠标位置
            s_PointerEventData.position = screenPosition;

            // 清空上一次的检测结果
            s_RaycastResults.Clear();

            // 执行UI射线检测
            EventSystem.current.RaycastAll(s_PointerEventData, s_RaycastResults);

            // 如果有检测到UI元素，返回true
            return s_RaycastResults.Count > 0;
        }
    }

}

