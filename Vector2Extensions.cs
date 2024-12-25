using System;
using System.Numerics;

namespace DangerousEffects
{
    public static class Vector2Extensions
    {
        public static float Distance(this Vector2 endPoint, Vector2 startPoint)
        {
            return (float)Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var radians = degrees * (float)(Math.PI / 180f);
            var sin = (float)Math.Sin(radians);
            var cos = (float)Math.Cos(radians);

            var tx = v.X;
            var ty = v.Y;

            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }

        public static Vector2 Normalize(this Vector2 v)
        {
            var length = v.Length();
            return length > float.Epsilon ? v / length : Vector2.Zero;
        }

        public static float Length(this Vector2 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)(Math.Atan2(vector.Y, vector.X) * (180 / Math.PI));
        }

        public static Vector2 TranslateToScreen(this Vector2 vec, float scale, Vector2 center)
        {
            return new Vector2(vec.X * scale + center.X, vec.Y * scale + center.Y);
        }

        public static Vector2 TranslateToWorld(this Vector2 vec, float scale, Vector2 center)
        {
            return new Vector2((vec.X - center.X) / scale, (vec.Y - center.Y) / scale);
        }

        public static Vector2 Translate(this Vector2 vec, Vector2 offset)
        {
            return vec + offset;
        }

        public static Vector2 Scale(this Vector2 vec, float scale)
        {
            return vec * scale;
        }

        public static Vector2 Negate(this Vector2 vec)
        {
            return -vec;
        }

        public static Vector2 PerpendicularClockwise(this Vector2 vec)
        {
            return new Vector2(vec.Y, -vec.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vec)
        {
            return new Vector2(-vec.Y, vec.X);
        }

        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float Dot(this Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 Lerp(this Vector2 start, Vector2 end, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return start + (end - start) * t;
        }

        public static bool IsZero(this Vector2 vec)
        {
            return Math.Abs(vec.X) < float.Epsilon && Math.Abs(vec.Y) < float.Epsilon;
        }

        public static Vector2 ClampMagnitude(this Vector2 vec, float maxLength)
        {
            var sqrMagnitude = vec.LengthSquared();
            if (sqrMagnitude > maxLength * maxLength)
            {
                var magnitude = (float)Math.Sqrt(sqrMagnitude);
                var normalized = vec / magnitude;
                return normalized * maxLength;
            }
            return vec;
        }

        public static Vector2 Reflect(this Vector2 vec, Vector2 normal)
        {
            normal = normal.Normalize();
            return vec - 2 * vec.Dot(normal) * normal;
        }

        public static Vector2 Project(this Vector2 vec, Vector2 onNormal)
        {
            onNormal = onNormal.Normalize();
            return onNormal * vec.Dot(onNormal);
        }

        public static Vector2 ProjectOnPlane(this Vector2 vec, Vector2 planeNormal)
        {
            return vec - vec.Project(planeNormal);
        }

        public static float SignedAngle(this Vector2 from, Vector2 to)
        {
            var unsigned = (float)Math.Acos(Math.Clamp(from.Normalize().Dot(to.Normalize()), -1f, 1f)) * (180f / (float)Math.PI);
            var sign = Math.Sign(from.X * to.Y - from.Y * to.X);
            return unsigned * sign;
        }
    }
}
