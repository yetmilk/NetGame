
using UnityEngine;
namespace Utility
{
    public static class DiractionUtility
    {
        // 角度容差（度），例如45度表示允许目标向量在基准方向左右45度范围内
        private const float ANGLE_TOLERANCE = 45f;

        public static Diraction GetDiractionByVector2(Vector2 dir)
        {
            if (dir == Vector2.left)
            {
                return Diraction.Left;
            }
            else if (dir == Vector2.right)
            {
                return Diraction.Right;
            }
            else if (dir == Vector2.up)
            {
                return Diraction.Up;
            }
            else if (dir == Vector2.down)
            {
                return Diraction.Down;
            }
            else if ((dir.x < 0) && (dir.y < 0))
            {
                return Diraction.LeftDown;
            }
            else if (((dir.x > 0) && dir.y < 0))
            {
                return Diraction.RightDown;
            }
            else if ((dir.x < 0) && (dir.y > 0))
            {
                return Diraction.LeftUp;
            }
            else if (dir.x > 0 && dir.y > 0)
            {
                return Diraction.RightUp;
            }
            else
                return Diraction.None;
        }

        public static Vector2 GetVector2ByDiraction(Diraction diraction)
        {
            switch (diraction)
            {
                case Diraction.Up:
                    return Vector2.up;

                case Diraction.Down:
                    return Vector2.down;
                case Diraction.Left:
                    return Vector2.left;
                case Diraction.Right:
                    return Vector2.right;
                case Diraction.LeftUp:
                    return Vector2.left + Vector2.up;
                case Diraction.RightUp:
                    return Vector2.right + Vector2.up;
                case Diraction.LeftDown:
                    return Vector2.left + Vector2.down;
                case Diraction.RightDown:
                    return Vector2.right + Vector2.down;
                case Diraction.None:
                    return Vector2.zero;
                default:
                    break;
            }
            return Vector2.zero;
        }

        public static Vector3 GetVector3ByDiraction(Diraction diraction)
        {
            switch (diraction)
            {
                case Diraction.Up:
                    return Vector3.forward;

                case Diraction.Down:
                    return Vector3.back;
                case Diraction.Left:
                    return Vector3.left;
                case Diraction.Right:
                    return Vector3.right;
                case Diraction.LeftUp:
                    return Vector3.left + Vector3.forward;
                case Diraction.RightUp:
                    return Vector3.right + Vector3.forward;
                case Diraction.LeftDown:
                    return Vector3.left + Vector3.back;
                case Diraction.RightDown:
                    return Vector3.right + Vector3.back;
                case Diraction.None:
                    return Vector3.zero;
                default:
                    break;
            }
            return Vector3.zero;
        }

        public static Diraction GetDirectionByV3(Vector3 dir)
        {
            // 归一化输入向量，确保长度为1
            if (dir == Vector3.zero)
                return Diraction.Right; // 默认返回右方向（可根据需求调整）

            dir = dir.normalized;

            // 定义8个基准方向及其对应枚举值
            (Vector3 direction, Diraction diraction)[] baseDirections = new[]
            {
        (Vector3.right, Diraction.Right),
        (Vector3.left, Diraction.Left),
        (Vector3.forward, Diraction.Up),
        (Vector3.back, Diraction.Down),
        (new Vector3(1, 0, 1).normalized, Diraction.RightUp),
        (new Vector3(1, 0, -1).normalized, Diraction.RightDown),
        (new Vector3(-1, 0, 1).normalized, Diraction.LeftUp),
        (new Vector3(-1, 0, -1).normalized, Diraction.LeftDown)
    };

            // 计算输入向量与每个基准方向的夹角，找出最小夹角对应的方向
            float minAngle = float.MaxValue;
            Diraction bestMatch = Diraction.Right; // 默认值，会在循环中被覆盖

            foreach (var (baseDir, diraction) in baseDirections)
            {
                float angle = Vector3.Angle(dir, baseDir);

                if (angle < minAngle)
                {
                    minAngle = angle;
                    bestMatch = diraction;
                }
            }

            return bestMatch; // 始终返回最接近的方向
        }
    }

    }
