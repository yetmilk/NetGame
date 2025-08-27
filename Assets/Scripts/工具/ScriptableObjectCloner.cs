using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility
{
    public static class ObjectCloner
    {
        // ��¡ScriptableObject������ԭ���ܣ�
        public static T Clone<T>(T original) where T : ScriptableObject
        {
            if (original == null)
            {
                Debug.LogError("Original ScriptableObject is null!");
                return null;
            }

            T clone = ScriptableObject.CreateInstance<T>();
            CopyFields(original, clone);
            return clone;
        }

        // ��¡��ͨ�ࣨ����ͨ�����������
        public static T CloneObject<T>(T original) where T : class, new()
        {
            if (original == null)
            {
                Debug.LogError("Original object is null!");
                return null;
            }

            T clone = new T();
            CopyFields(original, clone);
            return clone;
        }

        // �����ֶθ����߼����ݹ鴦���������ͣ�
        private static void CopyFields(object source, object target)
        {
            if (source == null || target == null) return;

            Type type = source.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // ������̬�ֶκͳ���
                if (field.IsStatic || field.IsLiteral)
                    continue;

                object value = field.GetValue(source);
                if (value == null)
                {
                    field.SetValue(target, null);
                    continue;
                }

                // ��������
                if (field.FieldType.IsArray)
                {
                    Array originalArray = (Array)value;
                    Array clonedArray = (Array)Activator.CreateInstance(field.FieldType, originalArray.Length);

                    // �ݹ鸴������Ԫ��
                    for (int i = 0; i < originalArray.Length; i++)
                    {
                        object element = originalArray.GetValue(i);
                        clonedArray.SetValue(CloneElement(element), i);
                    }

                    field.SetValue(target, clonedArray);
                }
                // ����ֵ���ͣ�ֱ�Ӹ��ƣ�
                else if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    field.SetValue(target, value);
                }
                // �����������ͣ��ݹ��¡��
                else
                {
                    object clonedValue = CloneElement(value);
                    field.SetValue(target, clonedValue);
                }
            }
        }

        // ��¡����Ԫ�أ�����ֵ���ͺ��������ͣ�
        private static object CloneElement(object element)
        {
            if (element == null) return null;

            Type elementType = element.GetType();

            // ֵ���ͻ��ַ���ֱ�ӷ��أ�ֵ���ͻ��Զ����ƣ�
            if (elementType.IsValueType || elementType == typeof(string))
            {
                return element;
            }

            // �������ͣ������Ĭ�Ϲ��캯���򴴽���ʵ���������ֶ�
            ConstructorInfo ctor = elementType.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                object cloned = Activator.CreateInstance(elementType);
                CopyFields(element, cloned);
                return cloned;
            }

            // ��֧���޲ι��캯�������ͣ���ĳЩϵͳ�ࣩ
            Debug.LogWarning($"��֧�ֿ�¡���� {elementType.Name}��ȱ���޲ι��캯����");
            return element;
        }
    }
}