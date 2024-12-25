using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using System.Drawing;

namespace DangerousEffects
{
    public class DangerousEffectsSettings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode MultiThreading { get; set; } = new ToggleNode(false);
        public RangeNode<int> MaxCircleDrawDistance { get; set; } = new RangeNode<int>(500, 1, 2000);

        public EffectConfig GroundEffects { get; set; } = new EffectConfig()
        {
            Enable = true,
            Colors = new EffectConfig.EffectColors
            {
                MapColor = Color.FromArgb(255, 255, 0, 0),
                MapAttackColor = Color.FromArgb(255, 255, 0, 0),
                WorldColor = Color.FromArgb(255, 255, 0, 0),
                WorldAttackColor = Color.FromArgb(255, 255, 0, 0),
            },
            World = new EffectConfig.WorldRender
            {
                Enable = true,
                DrawAttack = false,
                DrawAttackEndPoint = false,
                DrawDestination = false,
                DrawDestinationEndPoint = false,
                DrawLine = false,
                AlwaysRenderWorldUnit = true,
                DrawFilledCircle = false,
                DrawBoundingBox = false,
                RenderCircleThickness = 3,
                LineThickness = 5
            },
            Map = new EffectConfig.MapRender
            {
                Enable = false,
                DrawAttack = false,
                DrawDestination = false,
                LineThickness = 1
            }
        };

        public EffectConfig SpecialEffects { get; set; } = new EffectConfig()
        {
            Enable = true,
            Colors = new EffectConfig.EffectColors
            {
                MapColor = Color.FromArgb(255, 255, 128, 0),
                MapAttackColor = Color.FromArgb(255, 255, 0, 0),
                WorldColor = Color.FromArgb(255, 255, 128, 0),
                WorldAttackColor = Color.FromArgb(255, 255, 0, 0),
            },
            World = new EffectConfig.WorldRender
            {
                Enable = true,
                DrawAttack = false,
                DrawAttackEndPoint = false,
                DrawDestination = false,
                DrawDestinationEndPoint = false,
                DrawLine = false,
                AlwaysRenderWorldUnit = true,
                DrawFilledCircle = false,
                DrawBoundingBox = false,
                RenderCircleThickness = 3,
                LineThickness = 5
            },
            Map = new EffectConfig.MapRender
            {
                Enable = false,
                DrawAttack = false,
                DrawDestination = false,
                LineThickness = 1
            }
        };
    }

    public class EffectConfig
    {
        public bool Enable { get; set; }
        public EffectColors Colors { get; set; }
        public WorldRender World { get; set; }
        public MapRender Map { get; set; }

        public class EffectColors
        {
            public Color MapColor { get; set; }
            public Color MapAttackColor { get; set; }
            public Color WorldColor { get; set; }
            public Color WorldAttackColor { get; set; }
        }

        public class WorldRender
        {
            public bool Enable { get; set; }
            public bool DrawAttack { get; set; }
            public bool DrawAttackEndPoint { get; set; }
            public bool DrawDestination { get; set; }
            public bool DrawDestinationEndPoint { get; set; }
            public bool DrawLine { get; set; }
            public bool AlwaysRenderWorldUnit { get; set; }
            public bool DrawFilledCircle { get; set; }
            public bool DrawBoundingBox { get; set; }
            public float RenderCircleThickness { get; set; }
            public float LineThickness { get; set; }
        }

        public class MapRender
        {
            public bool Enable { get; set; }
            public bool DrawAttack { get; set; }
            public bool DrawDestination { get; set; }
            public float LineThickness { get; set; }
        }
    }
}
