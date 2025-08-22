using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utility
{
    public static class DebugUtility
    {
        /// <summary>
        /// 绘制带箭头的射线
        /// </summary>
        /// <param name="start">射线起点位置</param>
        /// <param name="direction">射线方向</param>
        /// <param name="color">射线颜色</param>
        /// <param name="duration">射线存在时间</param>
        public static void DrawArrow(Vector3 start, Vector3 direction, Color color, float duration = 0)
        {
            Debug.DrawRay(start, direction, color, duration);

            // 绘制箭头头部
            Vector3 end = start + direction;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 135, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -135, 0) * Vector3.forward;

            Debug.DrawRay(end, right * direction.magnitude * 0.2f, color, duration);
            Debug.DrawRay(end, left * direction.magnitude * 0.2f, color, duration);
        }
    }
}


