using ExileCore2.Shared.Helpers;
using ExileCore2.Shared.Nodes;
using ImGuiNET;
using System.Drawing;
using ImGuiVector4 = System.Numerics.Vector4;

namespace DangerousEffects
{
    public class ImGuiExtension
    {
        public static bool Checkbox(string labelString, bool boolValue)
        {
            ImGui.Checkbox(labelString, ref boolValue);
            return boolValue;
        }

        public static int IntDrag(string labelString, int intValue, int minValue, int maxValue, float speed)
        {
            ImGui.DragInt(labelString, ref intValue, speed, minValue, maxValue);
            return intValue;
        }

        public static float FloatDrag(string labelString, float floatValue, float speed = 1, float minValue = 0, float maxValue = float.MaxValue)
        {
            ImGui.DragFloat(labelString, ref floatValue, speed, minValue, maxValue);
            return floatValue;
        }

        public static float FloatSlider(string labelString, float floatValue, float minValue, float maxValue)
        {
            ImGui.SliderFloat(labelString, ref floatValue, minValue, maxValue);
            return floatValue;
        }

        public static Color ColorPicker(string labelName, Color inputColor)
        {
            var color = inputColor.ToImguiVec4();
            var colorToVect4 = new ImGuiVector4(color.X, color.Y, color.Z, color.W);

            if (ImGui.ColorEdit4(labelName, ref colorToVect4, ImGuiColorEditFlags.AlphaBar))
            {
                return Color.FromArgb((int)(colorToVect4.W * 255), (int)(colorToVect4.X * 255), (int)(colorToVect4.Y * 255), (int)(colorToVect4.Z * 255));
            }

            return inputColor;
        }
    }
}
