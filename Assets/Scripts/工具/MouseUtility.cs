using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public static class MouseUtility
    {
        // �������� UI ���߼��Ĳ���������Ƶ��������
        private static PointerEventData s_PointerEventData;
        private static List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

        /// <summary>
        /// ��ȡָ����Ļλ�ö�Ӧ��3D����������꣬�ᱻUI�ڵ�
        /// </summary>
        public static Vector3 GetClickedPosition(Vector3 screenPosition, string layerMaskName, out bool checkSucc)
        {
            // �ֶ�����UI���߼�⣨��ǰ֡״̬��
            if (IsPointerOverUI(screenPosition))
            {
                checkSucc = false;
                return Vector3.zero;
            }

            int layerMask = string.IsNullOrEmpty(layerMaskName) ? -1 : LayerMask.GetMask(layerMaskName);


            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("����������ڣ�");
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
        /// ���ָ����Ļλ���Ƿ���UI�ϣ���ǰ֡״̬��
        /// </summary>
        private static bool IsPointerOverUI(Vector3 screenPosition)
        {
            // ��ʼ���¼����ݣ�ֻ����һ�Σ�
            if (s_PointerEventData == null)
            {
                s_PointerEventData = new PointerEventData(EventSystem.current);
            }

            // ���õ�ǰ���λ��
            s_PointerEventData.position = screenPosition;

            // �����һ�εļ����
            s_RaycastResults.Clear();

            // ִ��UI���߼��
            EventSystem.current.RaycastAll(s_PointerEventData, s_RaycastResults);

            // ����м�⵽UIԪ�أ�����true
            return s_RaycastResults.Count > 0;
        }
    }

}

