using System;
using System.Numerics;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Cache;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;

namespace DangerousEffects
{
    public static class Helper
    {
        public static bool TryGetComponent<T>(this Entity entity, out T component) where T : Component, new()
        {
            component = entity?.GetComponent<T>();
            return component != null;
        }

        public static Vector2 GridToWorld(this Vector2 v, float deltaZ = 0)
        {
            return new Vector2(v.X * 10.869565f + 5.434783f, v.Y * 10.869565f + 5.434783f);
        }

        public static Vector2 WorldToGrid(this Vector2 v)
        {
            return new Vector2((v.X - 5.434783f) / 10.869565f, (v.Y - 5.434783f) / 10.869565f);
        }
    }
}
