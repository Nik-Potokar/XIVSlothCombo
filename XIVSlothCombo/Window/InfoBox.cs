using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Numerics;

namespace XIVSlothCombo.Window
{
    internal class InfoBox
    {
        public Vector4 Color { get; set; } = Colors.White;
        public Action ContentsAction { get; set; } = () => ImGui.Text("Action Not Set");
        public float CurveRadius { get; set; } = 1f;
        public float ContentsOffset { get; set; } = 0f;
        public Vector2 Size { get; set; } = Vector2.Zero;
        public float BorderThickness { get; set; } = 2.0f;
        public int SegmentResolution { get; set; } = 10;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public string Label { get; set; } = "";
        public bool AutoResize { get; set; } = true;
        private static ImDrawListPtr DrawList => ImGui.GetWindowDrawList();
        private uint ColorU32 => ImGui.GetColorU32(Color);
        private Vector2 StartPosition { get; set; }
        public bool Debug { get; set; } = false;
        public bool HasMaxWidth { get; set; } = false;
        public bool IsSubBox { get; set; } = false;

        public void Draw()
        {
            StartPosition = ImGui.GetCursorScreenPos();
            StartPosition += Offset;

            if (Debug)
            {
                DrawList.AddCircleFilled(StartPosition, 2.0f, ImGui.GetColorU32(Colors.Purple));
            }

            DrawContents();

            if (Size == Vector2.Zero)
            {
                Size = ImGui.GetContentRegionAvail() with { Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + (CurveRadius * 2.0f) };
            }

            if (AutoResize)
            {
                Size = Size with { Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + (CurveRadius * 2.0f) };
            }

            if (HasMaxWidth)
            {
                if ((ImGui.GetItemRectMax().X - ImGui.GetItemRectMin().X + 15f) < ImGui.GetWindowSize().X - 60f)
                    Size = Size with { X = ImGui.GetItemRectMax().X - ImGui.GetItemRectMin().X + 15f };
            }

            DrawCorners();

            DrawBorders();
        }

        public void DrawCentered(float percentSize = 0.80f)
        {
            Vector2 region = ImGui.GetContentRegionAvail();
            Vector2 currentPosition = ImGui.GetCursorPos();
            Vector2 width = new(region.X * percentSize);
            ImGui.SetCursorPos(currentPosition with { X = (region.X / 2.0f) - (width.X / 2.0f) });

            Size = width;
            Draw();
        }

        private void DrawContents()
        {
            Vector2 topLeftCurveCenter = new(StartPosition.X + CurveRadius + ContentsOffset, StartPosition.Y + CurveRadius + ContentsOffset);

            ImGui.SetCursorScreenPos(topLeftCurveCenter);
            ImGui.PushTextWrapPos(Size.X);

            using (var group = ImRaii.Group())
            {
                ImGui.PushID(Label);
                ContentsAction();
                if (ContentsOffset > 0)
                    ImGuiHelpers.ScaledDummy(ContentsOffset);
                ImGui.PopID();
            }

            ImGui.PopTextWrapPos();
        }

        private void DrawCorners()
        {
            Vector2 topLeftCurveCenter = new(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);
            Vector2 topRightCurveCenter = new(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + CurveRadius);
            Vector2 bottomLeftCurveCenter = new(StartPosition.X + CurveRadius, StartPosition.Y + Size.Y - CurveRadius);
            Vector2 bottomRightCurveCenter = new(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + Size.Y - CurveRadius);

            DrawList.PathArcTo(topLeftCurveCenter, CurveRadius, DegreesToRadians(180), DegreesToRadians(270), SegmentResolution);
            DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(topRightCurveCenter, CurveRadius, DegreesToRadians(360), DegreesToRadians(270), SegmentResolution);
            DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(bottomLeftCurveCenter, CurveRadius, DegreesToRadians(90), DegreesToRadians(180), SegmentResolution);
            DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(bottomRightCurveCenter, CurveRadius, DegreesToRadians(0), DegreesToRadians(90), SegmentResolution);
            DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

            if (Debug)
            {
                DrawList.AddCircleFilled(topLeftCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Red));
                DrawList.AddCircleFilled(topRightCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Green));
                DrawList.AddCircleFilled(bottomLeftCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Blue));
                DrawList.AddCircleFilled(bottomRightCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Orange));
            }
        }

        private void DrawBorders()
        {
            uint color = Debug ? ImGui.GetColorU32(Colors.Red) : ColorU32;

            DrawList.AddLine(new Vector2(StartPosition.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius - 0.5f, StartPosition.Y + Size.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y + Size.Y - 0.5f), color, BorderThickness);

            Vector2 textSize = ImGui.CalcTextSize(Label);
            float textStartPadding;
            float textEndPadding;
            float textVerticalOffset;

            if (textSize.X > 0)
            {
                textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
                textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
                textVerticalOffset = textSize.Y / 2.0f;
            }

            else
            {
                textStartPadding = 0;
                textEndPadding = 0;
                textVerticalOffset = 0;
            }

            DrawList.AddText(new Vector2(StartPosition.X + CurveRadius + textStartPadding, StartPosition.Y - textVerticalOffset), ColorU32, Label);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius + textStartPadding + textSize.X + textEndPadding, StartPosition.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y - 0.5f), color, BorderThickness);
        }

        private static float DegreesToRadians(float degrees) => MathF.PI / 180 * degrees;
    }

    internal static class Colors
    {
        public static Vector4 Purple = new(176 / 255.0f, 38 / 255.0f, 236 / 255.0f, 1.0f);
        public static Vector4 Blue = new(37 / 255.0f, 168 / 255.0f, 1.0f, 1.0f);
        public static Vector4 ForestGreen = new(0.133f, 0.545f, 0.1333f, 1.0f);
        public static Vector4 White = new(1.0f, 1.0f, 1.0f, 1.0f);
        public static Vector4 Red = new(1.0f, 0.0f, 0.0f, 1.0f);
        public static Vector4 Green = new(0.0f, 1.0f, 0.0f, 1.0f);
        public static Vector4 Black = new(0.0f, 0.0f, 0.0f, 1.0f);
        public static Vector4 HealerGreen = new(33 / 255f, 193 / 255f, 0, 1.0f);
        public static Vector4 DPSRed = new(210 / 255f, 42 / 255f, 43 / 255f, 1.0f);
        public static Vector4 SoftRed = new(0.8f, 0.2f, 0.2f, 1.0f);
        public static Vector4 Grey = new(0.4f, 0.4f, 0.4f, 1.0f);
        public static Vector4 Orange = new(1.0f, 165.0f / 255.0f, 0.0f, 1.0f);
        public static Vector4 SoftGreen = new(0.2f, 0.8f, 0.2f, 1.0f);
    }
}
