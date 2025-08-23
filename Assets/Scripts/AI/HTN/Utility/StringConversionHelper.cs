using System;
using System.Reflection;

public class StringConversionHelper
{
    // 将string转换为目标类型（由boxedType示例对象指定）
    public static object ConvertStringToType(string input, object boxedTypeExample)
    {
        if (input == null)
            return null;

        // 获取目标类型
        Type targetType = boxedTypeExample.GetType();

        // 处理可空类型（如int?）
        if (targetType.IsGenericType &&
            targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(input))
                return null; // 空字符串转换为null

            // 获取可空类型的基础类型（如int?的基础类型是int）
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        try
        {
            // 使用Convert.ChangeType进行转换（适用于大多数基本类型）
            return Convert.ChangeType(input, targetType);
        }
        catch (Exception ex)
        {
            // 转换失败，记录错误
            Console.WriteLine($"转换失败: {ex.Message}");
            return null;
        }
    }

    // 重载方法：直接传入目标类型（不依赖示例对象）
    public static object ConvertStringToType(string input, Type targetType)
    {
        if (input == null)
            return null;

        // 处理可空类型
        if (targetType.IsGenericType &&
            targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(input))
                return null;

            targetType = Nullable.GetUnderlyingType(targetType);
        }

        try
        {
            // 使用Convert.ChangeType
            return Convert.ChangeType(input, targetType);
        }
        catch
        {
            // 尝试其他方法（如自定义类型的构造函数）
            if (TryCreateCustomType(input, targetType, out object result))
                return result;

            return null;
        }
    }

    // 尝试创建自定义类型（通过反射调用构造函数）
    private static bool TryCreateCustomType(string input, Type targetType, out object result)
    {
        result = null;

        // 检查是否有带string参数的构造函数
        ConstructorInfo constructor = targetType.GetConstructor(new[] { typeof(string) });
        if (constructor != null)
        {
            try
            {
                result = constructor.Invoke(new object[] { input });
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 其他自定义创建逻辑...
        return false;
    }
}
