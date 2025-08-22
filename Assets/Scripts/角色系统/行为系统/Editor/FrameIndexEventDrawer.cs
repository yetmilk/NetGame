using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(FrameIndexEvent), true)]
public class FrameIndexEventDrawer : PropertyDrawer
{
    private string[] eventNames;
    private bool initialized = false;
    private const float VERTICAL_SPACING = 20f;

    // 初始化事件名称列表
    private void Initialize()
    {
        if (initialized) return;

        // 从EventCenter获取所有事件ID
        Type functionIdType = typeof(FunctionId);
        if (functionIdType == null)
        {
            Debug.LogError("EventCenter type not found.");
            return;
        }


        // 尝试获取EventId字段
        FieldInfo[] eventIdFields = functionIdType.GetFields(BindingFlags.Public | BindingFlags.Static);
        if (eventIdFields.Length > 0)
        {
            eventNames = new string[eventIdFields.Length];
            for (int i = 0; i < eventIdFields.Length; i++)
            {
                eventNames[i] = (string)eventIdFields[i].GetValue(null);
            }
        }
        else
        {
            Debug.LogError("EventId fields not found in EventCenter.");
            return;
        }


        Debug.Log($"Initialized event names: {string.Join(", ", eventNames)}");
        initialized = true;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize();
        if (eventNames == null || eventNames.Length == 0)
        {
            EditorGUI.LabelField(position, "Event names not initialized");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = position;

        // 绘制折叠标题
        contentPosition.height = EditorGUIUtility.singleLineHeight;
        property.isExpanded = EditorGUI.Foldout(contentPosition, property.isExpanded, label);
        contentPosition.y += contentPosition.height + VERTICAL_SPACING;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // 绘制事件生命周期范围
            DrawPropertyField(ref contentPosition, property, "eventLifeRange", "事件生命周期范围");

            // 绘制事件名称下拉菜单
            DrawEventNameDropdown(ref contentPosition, property);

            // 当事件名称改变时，创建相应的EventParam
            if (GUI.changed)
            {
                SerializedProperty eventNameProp = property.FindPropertyRelative("eventName");
                if (eventNameProp != null)
                {
                    CreateEventParam(property, eventNameProp.stringValue);
                }
            }

            // 绘制事件参数（直接使用类型名作为字段名，需确保字段名与类型名一致）
            string paramTypeName = GetEventParamTypeName(property);
            if (!string.IsNullOrEmpty(paramTypeName))
            {
                SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
                if (paramProp != null)
                {
                    contentPosition.height = GetEventParamHeight(paramProp);
                    EditorGUI.PropertyField(contentPosition, paramProp, new GUIContent("事件参数"), true);
                    contentPosition.y += contentPosition.height + VERTICAL_SPACING;
                }
                else
                {
                    EditorGUI.LabelField(contentPosition, $"未找到参数属性: {paramTypeName}");
                    contentPosition.y += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING;
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawPropertyField(ref Rect position, SerializedProperty property, string propName, string label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        SerializedProperty prop = property.FindPropertyRelative(propName);
        if (prop != null)
        {
            EditorGUI.PropertyField(position, prop, new GUIContent(label));
        }
        else
        {
            EditorGUI.LabelField(position, $"未找到属性: {propName}");
        }
        position.y += position.height + VERTICAL_SPACING;
    }

    private void DrawEventNameDropdown(ref Rect position, SerializedProperty property)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        SerializedProperty eventNameProp = property.FindPropertyRelative("actionName");
        if (eventNameProp != null)
        {
            int selectedIndex = Array.IndexOf(eventNames, eventNameProp.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(position, "事件名称", selectedIndex, eventNames);
            eventNameProp.stringValue = eventNames[selectedIndex];
        }
        else
        {
            EditorGUI.LabelField(position, "未找到事件名称属性");
        }
        position.y += position.height + VERTICAL_SPACING;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!initialized) Initialize();
        if (eventNames == null || eventNames.Length == 0) return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // 折叠标题

        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // 生命周期范围
            height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // 事件名称下拉菜单

            string paramTypeName = GetEventParamTypeName(property);
            if (!string.IsNullOrEmpty(paramTypeName))
            {
                SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
                if (paramProp != null)
                {
                    height += GetEventParamHeight(paramProp) + VERTICAL_SPACING;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING;
                }
            }
        }

        return height;
    }

    private float GetEventParamHeight(SerializedProperty paramProp)
    {
        return EditorGUI.GetPropertyHeight(paramProp, true);
    }

    private void CreateEventParam(SerializedProperty property, string eventName)
    {
        Type paramType = FindEventParamType(eventName);
        if (paramType == null)
        {
            Debug.LogWarning($"未找到事件 '{eventName}' 对应的EventParam类型");
            return;
        }

        // 直接使用类型名作为字段名（需确保FrameIndexEvent中存在该名称的字段）
        string paramTypeName = paramType.Name;
        SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
        if (paramProp != null)
        {
            // 创建新实例并序列化
            FunctionParam newParam = (FunctionParam)Activator.CreateInstance(paramType);
            string json = JsonUtility.ToJson(newParam);
            JsonUtility.FromJsonOverwrite(json, paramProp.serializedObject.targetObject);
            paramProp.serializedObject.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning($"未找到参数属性: {paramTypeName}，请确保FrameIndexEvent中存在该字段");
        }
    }

    private Type FindEventParamType(string eventName)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(FunctionParam));
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            if (type.IsSubclassOf(typeof(FunctionParam)) && !type.IsAbstract)
            {
                FunctionParamAttribute attribute = type.GetCustomAttribute<FunctionParamAttribute>();
                if (attribute != null && attribute.FunctionName == eventName)
                {
                    return type;
                }
            }
        }
        return null;
    }

    private string GetEventParamTypeName(SerializedProperty property)
    {
        SerializedProperty eventNameProp = property.FindPropertyRelative("actionName");
        if (eventNameProp != null)
        {
            Type paramType = FindEventParamType(eventNameProp.stringValue);
            if (paramType != null)
            {
                return paramType.Name; // 直接使用类型名作为字段名
            }
        }
        return "";
    }
}