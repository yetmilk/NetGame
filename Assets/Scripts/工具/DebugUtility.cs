using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utility
{
    public static class DebugUtility
    {
        /// <summary>
        /// ���ƴ���ͷ������
        /// </summary>
        /// <param name="start">�������λ��</param>
        /// <param name="direction">���߷���</param>
        /// <param name="color">������ɫ</param>
        /// <param name="duration">���ߴ���ʱ��</param>
        public static void DrawArrow(Vector3 start, Vector3 direction, Color color, float duration = 0)
        {
            Debug.DrawRay(start, direction, color, duration);

            // ���Ƽ�ͷͷ��
            Vector3 end = start + direction;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 135, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -135, 0) * Vector3.forward;

            Debug.DrawRay(end, right * direction.magnitude * 0.2f, color, duration);
            Debug.DrawRay(end, left * direction.magnitude * 0.2f, color, duration);
        }
    }
}


