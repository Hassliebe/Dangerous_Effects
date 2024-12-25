using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using ExileCore2.Shared.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Map = ExileCore2.PoEMemory.Elements.Map;
using RectangleF = ExileCore2.Shared.RectangleF;

namespace DangerousEffects
{
    public class DangerousEffects : BaseSettingsPlugin<DangerousEffectsSettings>
    {
        private enum EntityCategory
        {
            GroundEffect,
            SpecialEffect,
            Monster
        }

        private class EntityDefinition
        {
            public string Name { get; set; }               
            public EntityCategory Category { get; set; }   
            public string PathMatch { get; set; }          
            public string MetadataMatch { get; set; }      
            public string AnimatedObjectMatch { get; set; } 
            public string BuffMatch { get; set; }          
            public string DisplayText { get; set; }        
            public float SizeMultiplier { get; set; }      
            public Color? OverrideColor { get; set; }      
        }

        private readonly List<EntityDefinition> entityDefinitions = new()
        {
            // Special Effects - these are checked first
            new EntityDefinition 
            { 
                Name = "Water Vortex",
                Category = EntityCategory.SpecialEffect,
                AnimatedObjectMatch = "WaterVortex",
                DisplayText = "Vortex",
                SizeMultiplier = 2.0f
            },
            new EntityDefinition
            {
                Name = "Drowning Orb",
                Category = EntityCategory.Monster,
                PathMatch = "DrowningOrbs/DrowningOrb",
                DisplayText = "Orb",
                SizeMultiplier = 1.0f
            },
            new EntityDefinition
            {
                Name = "Volatile Orb",
                Category = EntityCategory.Monster,
                PathMatch = "VolatileRocks",
                DisplayText = "EXPLODES",
                SizeMultiplier = 1.0f
            },
            new EntityDefinition
            {
                Name = "Ice Explosion",
                Category = EntityCategory.SpecialEffect,
                AnimatedObjectMatch = "ice_beacons",
                DisplayText = "ICE EXPLOSION",
                SizeMultiplier = 7.7f,
                OverrideColor = Color.FromArgb(255, 0, 191, 255) // Deep Sky Blue
            },
            new EntityDefinition
            {
                Name = "Lightning Clone",
                Category = EntityCategory.Monster,
                PathMatch = "RangerLightningClone",
                DisplayText = "Chasing you!",
                SizeMultiplier = 1.0f
            },
            new EntityDefinition
            {
                Name = "Cenobite Bloater",
                Category = EntityCategory.Monster,
                PathMatch = "CenobiteBloater",
                DisplayText = "EXPLODES !!",
                SizeMultiplier = 1.0f
            },
            new EntityDefinition
            {
                Name = "Bloater Explosion",
                Category = EntityCategory.SpecialEffect,
                AnimatedObjectMatch = "Bloater_Death_Explode",
                DisplayText = "Explodes!!",
                SizeMultiplier = 1.0f
            },
            new EntityDefinition
            {
                Name = "Lightning Death Monster",
                Category = EntityCategory.Monster,
                PathMatch = "OnDeathLightningExplosion",
                DisplayText = "LIGHTNING EXPLOSION",
                SizeMultiplier = 2.0f,
                OverrideColor = Color.FromArgb(255, 255, 255, 0) // Yellow for lightning
            },
            new EntityDefinition
            {
                Name = "Fire Death Monster",
                Category = EntityCategory.Monster,
                PathMatch = "OnDeathFireExplosion",
                DisplayText = "FIRE EXPLOSION",
                SizeMultiplier = 2.0f,
                OverrideColor = Color.FromArgb(255, 255, 69, 0) // Red-Orange for fire
            },
            new EntityDefinition
            {
                Name = "Ice Death Monster",
                Category = EntityCategory.Monster,
                PathMatch = "OnDeathIceExplosion",
                DisplayText = "ICE EXPLOSION",
                SizeMultiplier = 2.0f,
                OverrideColor = Color.FromArgb(255, 0, 191, 255) // Deep Sky Blue for ice
            },
            new EntityDefinition
            {
                Name = "Chilled Ground",
                Category = EntityCategory.GroundEffect,
                AnimatedObjectMatch = "grd_Chilled",
                DisplayText = "CHILLED",
                SizeMultiplier = 5.0f,
                OverrideColor = Color.FromArgb(80, 173, 216, 230) // Light blue with low opacity
            },
            // Ground effects - these are checked last
            new EntityDefinition
            {
                Name = "Burned Ground",
                Category = EntityCategory.GroundEffect,
                AnimatedObjectMatch = "burned_ground",
                DisplayText = "BURNING",
                SizeMultiplier = 5.0f,
                OverrideColor = Color.FromArgb(255, 255, 140, 0) // Bright orange for fire
            },
            new EntityDefinition
            {
                Name = "Fire Ground Effect",
                Category = EntityCategory.GroundEffect,
                BuffMatch = "fire|burning",
                DisplayText = "Fire",
                SizeMultiplier = 1.0f,
                OverrideColor = Color.FromArgb(255, 255, 0, 0) // Red
            },
            new EntityDefinition
            {
                Name = "Cold Ground Effect",
                Category = EntityCategory.GroundEffect,
                BuffMatch = "cold",
                DisplayText = "Cold",
                SizeMultiplier = 1.0f,
                OverrideColor = Color.FromArgb(255, 0, 128, 255) // Blue
            },
            new EntityDefinition
            {
                Name = "Lightning Ground Effect",
                Category = EntityCategory.GroundEffect,
                BuffMatch = "lightning",
                DisplayText = "Lightning",
                SizeMultiplier = 1.0f,
                OverrideColor = Color.FromArgb(255, 255, 255, 0) // Yellow
            },
            new EntityDefinition
            {
                Name = "Chaos Ground Effect",
                Category = EntityCategory.GroundEffect,
                BuffMatch = "chaos",
                DisplayText = "Chaos",
                SizeMultiplier = 1.0f,
                OverrideColor = Color.FromArgb(255, 128, 0, 128) // Purple
            },
            new EntityDefinition
            {
                Name = "Ground Explosions",
                Category = EntityCategory.GroundEffect,
                //PathMatch = "explode_on_death",  // Match the entity's path
                //MetadataMatch = "ground_metadata", // Match the entity's metadata
                AnimatedObjectMatch = "ice_beacons", // Match animated objects
                //BuffMatch = "ground_buff",         // Match buff names
                DisplayText = "ICE EXPLOSION",  // Text to display
                SizeMultiplier = 4f,             // Size of the warning circle
                OverrideColor = Color.FromArgb(255, 0, 191, 255) // Deep Sky Blue
            }
        };

        private Map MapWindow => GameController?.IngameState?.IngameUi?.Map;

        private void DrawCircleInWorldPos(bool drawFilledCircle, Vector3 position, float radius, int thickness, Color color)
        {
            var screensize = new RectangleF
            {
                X = 0,
                Y = 0,
                Width = GameController.Window.GetWindowRectangleTimeCache.Size.X,
                Height = GameController.Window.GetWindowRectangleTimeCache.Size.Y
            };

            var entityPos = RemoteMemoryObject.TheGame.IngameState.Camera.WorldToScreen(position);
            if (IsEntityWithinScreen(entityPos, screensize, 50))
            {
                if (drawFilledCircle)
                {
                    Graphics.DrawFilledCircleInWorld(position, radius, color, 32);
                }
                else
                {
                    Graphics.DrawCircleInWorld(position, radius, color, thickness);
                }
            }
        }

        private bool IsEntityWithinScreen(Vector2 entityPos, RectangleF screensize, float allowancePX)
        {
            var leftBound = screensize.Left - allowancePX;
            var rightBound = screensize.Right + allowancePX;
            var topBound = screensize.Top - allowancePX;
            var bottomBound = screensize.Bottom + allowancePX;

            return entityPos.X >= leftBound && entityPos.X <= rightBound && entityPos.Y >= topBound && entityPos.Y <= bottomBound;
        }

        public override void Render()
        {
            if (!Settings.Enable.Value || !GameController.InGame) return;

            var player = GameController?.Player;
            if (player == null) return;

            player.TryGetComponent<Positioned>(out var playerPositioned);
            if (playerPositioned == null) return;
            var playerPos = playerPositioned.GridPos;

            player.TryGetComponent<Render>(out var playerRender);
            if (playerRender == null) return;

            var posZ = GameController.Player.Pos.Z;

            if (MapWindow == null) return;
            var mapWindowLargeMapZoom = MapWindow.LargeMapZoom;

            var entityLists = new List<IEnumerable<Entity>>
            {
                GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.None] ?? Enumerable.Empty<Entity>(),
                GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.ServerObject] ?? Enumerable.Empty<Entity>(),
                GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Effect] ?? Enumerable.Empty<Entity>(),
                GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Monster] ?? Enumerable.Empty<Entity>(),
                GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Player] ?? Enumerable.Empty<Entity>()
            };

            var entityList = entityLists.SelectMany(list => list).ToList();
            if (entityList == null) return;

            foreach (var entity in entityList)
            {
                if (entity == null) continue;

                var drawSettings = new EffectConfig();

                switch (entity.Type)
                {
                    case EntityType.Monster:
                        // First check for special monster definitions
                        var monsterDef = entityDefinitions.FirstOrDefault(def =>
                            def.Category == EntityCategory.Monster && 
                            ((!string.IsNullOrEmpty(def.PathMatch) && entity.Path?.Contains(def.PathMatch) == true) ||
                             (!string.IsNullOrEmpty(def.MetadataMatch) && entity.Metadata?.Contains(def.MetadataMatch) == true)));

                        if (monsterDef != null)
                        {
                            if (entity.IsAlive)
                            {
                                drawSettings = Settings.SpecialEffects;
                                if (drawSettings.Enable && drawSettings.World.Enable)
                                {
                                    entity.TryGetComponent<Render>(out var render);
                                    if (render != null)
                                    {
                                        var size = render.Bounds.X * monsterDef.SizeMultiplier;
                                        DrawCircleInWorldPos(
                                            drawSettings.World.DrawFilledCircle, 
                                            entity.Pos, 
                                            size,
                                            (int)drawSettings.World.RenderCircleThickness,
                                            monsterDef.OverrideColor ?? drawSettings.Colors.WorldColor);

                                        if (!string.IsNullOrEmpty(monsterDef.DisplayText))
                                        {
                                            var screenPos = RemoteMemoryObject.TheGame.IngameState.Camera.WorldToScreen(entity.Pos);
                                            var textWidth = Graphics.MeasureText(monsterDef.DisplayText, 15).X;
                                            screenPos.X -= textWidth / 2;
                                            Graphics.DrawText(monsterDef.DisplayText, screenPos, monsterDef.OverrideColor ?? drawSettings.Colors.WorldColor);
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                        break;

                    case EntityType.ServerObject:
                    case EntityType.Effect:
                        // Check for ground effects and special effects
                        var effectDef = entityDefinitions.FirstOrDefault(def =>
                            (def.Category == EntityCategory.GroundEffect || def.Category == EntityCategory.SpecialEffect) && 
                            ((!string.IsNullOrEmpty(def.PathMatch) && entity.Path?.Contains(def.PathMatch) == true) ||
                             (!string.IsNullOrEmpty(def.MetadataMatch) && entity.Metadata?.Contains(def.MetadataMatch) == true) ||
                             (!string.IsNullOrEmpty(def.AnimatedObjectMatch) && 
                              (entity.Path?.Contains(def.AnimatedObjectMatch) == true ||
                               entity.GetComponent<Animated>()?.BaseAnimatedObjectEntity?.Path?.Contains(def.AnimatedObjectMatch) == true))));

                        if (effectDef != null)
                        {
                            drawSettings = effectDef.Category == EntityCategory.GroundEffect ? Settings.GroundEffects : Settings.SpecialEffects;
                            if (drawSettings.Enable && drawSettings.World.Enable)
                            {
                                entity.TryGetComponent<Render>(out var render);
                                if (render != null)
                                {
                                    var size = render.Bounds.X * effectDef.SizeMultiplier;
                                    DrawCircleInWorldPos(
                                        drawSettings.World.DrawFilledCircle, 
                                        entity.Pos, 
                                        size,
                                        (int)drawSettings.World.RenderCircleThickness,
                                        effectDef.OverrideColor ?? drawSettings.Colors.WorldColor);

                                    if (!string.IsNullOrEmpty(effectDef.DisplayText))
                                    {
                                        var screenPos = RemoteMemoryObject.TheGame.IngameState.Camera.WorldToScreen(entity.Pos);
                                        var textWidth = Graphics.MeasureText(effectDef.DisplayText, 15).X;
                                        screenPos.X -= textWidth / 2;
                                        Graphics.DrawText(effectDef.DisplayText, screenPos, effectDef.OverrideColor ?? drawSettings.Colors.WorldColor);
                                    }
                                }
                            }
                            continue;
                        }
                        break;
                }
            }
        }

        public override void DrawSettings()
        {
            Settings.Enable.Value = ImGuiExtension.Checkbox("Plugin Enabled", Settings.Enable.Value);
            Settings.MultiThreading.Value = ImGuiExtension.Checkbox("Multi Threading", Settings.MultiThreading.Value);
            Settings.MaxCircleDrawDistance.Value = ImGuiExtension.IntDrag("Max Draw Distance", Settings.MaxCircleDrawDistance.Value, 1, 1000, 1);

            // Ground Effects settings
            if (ImGui.CollapsingHeader("Ground Effects", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Indent();
                Settings.GroundEffects.Enable = ImGuiExtension.Checkbox("Ground Effects Enabled", Settings.GroundEffects.Enable);
                if (ImGui.TreeNode("Ground Colors"))
                {
                    Settings.GroundEffects.Colors.WorldColor = ImGuiExtension.ColorPicker("World Color", Settings.GroundEffects.Colors.WorldColor);
                    ImGui.Spacing();
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Ground World"))
                {
                    Settings.GroundEffects.World.Enable = ImGuiExtension.Checkbox("World Drawing Enabled##Ground", Settings.GroundEffects.World.Enable);
                    Settings.GroundEffects.World.DrawFilledCircle = ImGuiExtension.Checkbox("Draw Filled Circle##Ground", Settings.GroundEffects.World.DrawFilledCircle);
                    Settings.GroundEffects.World.RenderCircleThickness = ImGuiExtension.IntDrag("Circle Thickness##Ground", (int)Settings.GroundEffects.World.RenderCircleThickness, 1, 100, 1);
                    ImGui.Spacing();
                    ImGui.TreePop();
                }
                ImGui.Unindent();
            }

            // Special Effects settings
            if (ImGui.CollapsingHeader("Special Effects", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Indent();
                Settings.SpecialEffects.Enable = ImGuiExtension.Checkbox("Special Effects Enabled", Settings.SpecialEffects.Enable);
                if (ImGui.TreeNode("Special Colors"))
                {
                    Settings.SpecialEffects.Colors.WorldColor = ImGuiExtension.ColorPicker("World Color", Settings.SpecialEffects.Colors.WorldColor);
                    ImGui.Spacing();
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Special World"))
                {
                    Settings.SpecialEffects.World.Enable = ImGuiExtension.Checkbox("World Drawing Enabled##Special", Settings.SpecialEffects.World.Enable);
                    Settings.SpecialEffects.World.DrawFilledCircle = ImGuiExtension.Checkbox("Draw Filled Circle##Special", Settings.SpecialEffects.World.DrawFilledCircle);
                    Settings.SpecialEffects.World.RenderCircleThickness = ImGuiExtension.IntDrag("Circle Thickness##Special", (int)Settings.SpecialEffects.World.RenderCircleThickness, 1, 100, 1);
                    ImGui.Spacing();
                    ImGui.TreePop();
                }
                ImGui.Unindent();
            }
        }

        public override bool Initialise() => true;
    }
}
