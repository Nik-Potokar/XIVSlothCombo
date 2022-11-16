using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface.Components;
using ImGuiNET;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using System.Linq;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Core;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Functions
{
    public static class CommonFunctions
    {
        /// <summary>
        /// Round (i) Image with a tooltip
        /// </summary>
        /// <param name="desc">What gets shown on hover</param>
        public static void HelpMarker(string desc)
        {
            if (desc.Length == 0)
                return;

            ImGuiComponents.HelpMarker(desc);
        }

        /// <summary>
        /// Draw a text with a Font Awesome Icon
        /// </summary>
        /// <param name="icon"></param>
        public static void FontAwesomeIcon(Dalamud.Interface.FontAwesomeIcon icon)
        {
            ImGui.PushFont(Dalamud.Interface.UiBuilder.IconFont);
            ImGui.Text(Dalamud.Interface.FontAwesomeExtensions.ToIconString(icon));
            ImGui.PopFont();
        }

        /// <summary>
        /// Shows a tooltip if the mouse is hovering over the last item.
        /// </summary>
        /// <param name="desc">Text</param>
        public static void ToolTip(string desc)
        {
            if (desc.Length == 0)
                return;

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.Text(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        /// <summary>
        /// Shows a tooltip if the mouse is hovering over the last item.
        /// </summary>
        /// <param name="desc">Text</param>
        /// <param name="start">Tooltip hover Rectange start pos</param>
        /// <param name="end">Tooltip hover Rectange end pos</param>
        public static void ToolTip(string desc, Vector2 start, Vector2 end)
        {
            if (desc.Length == 0)
                return;

            if (ImGui.IsMouseHoveringRect(start, end))
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.Text(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        /// <summary>
        /// A Button with a FontAwesome Icon
        /// </summary>
        /// <param name="icon">The FontAwsome Icon</param>
        /// <param name="ID">The ID/Name of the Button (e.g. ##Neko)</param>
        /// <returns>If Button pressed</returns>
        public static bool IconButton(Dalamud.Interface.FontAwesomeIcon icon, string ID = "")
        {
            ImGui.PushFont(Dalamud.Interface.UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Button, 0);
            var ret = ImGui.Button(Dalamud.Interface.FontAwesomeExtensions.ToIconString(icon) + ID);
            ImGui.PopStyleColor();
            ImGui.PopFont();
            return ret;
        }

      /*/// <summary>
        /// Shows a Notification to the user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public static void Notification(string text, Dalamud.Interface.Internal.Notifications.NotificationType type = default) =>
                Plugin.PluginInterface.UiBuilder.AddNotification(text, "Neko Fans", type); */

        /// <summary>
        /// Only used for <see cref="TextWithColorsWrapped"/> to store the color and the text
        /// </summary>
        public struct Segment
        {
            public string Text;
            public Vector4 Color;

            public Segment(string text, Vector4 color)
            {
                Text = text;
                Color = color;
            }

            public Segment(string text)
            {
                Text = text;
                Color = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
            }
        }

        /// <summary>
        /// Draws a text wrapped with multiple colors.
        /// </summary>
        /// <param name="segments">Text Segments</param>
        public static void TextWithColorsWrapped(Segment[] segments)
        {
            var wrapWidth = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
            var space = ImGui.CalcTextSize(" ").X - 2f;

            // Setup Spacing
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

            foreach (var seg in segments)
            {
                // Set Text Color
                ImGui.PushStyleColor(ImGuiCol.Text, seg.Color);
                var split = seg.Text.Split(' ');

                for (var i = 0; i < split.Length; i++)
                {
                    var wordWidth = ImGui.CalcTextSize(split[i]).X;
                    var cursorPos = ImGui.GetCursorPos();

                    // If the word is too long to fit on the line, just print it on the next line
                    if (cursorPos.X + wordWidth > wrapWidth)
                        ImGui.NewLine();

                    ImGui.Text(split[i]);
                    ImGui.SameLine();

                    // Remove space at the end
                    if (i < split.Length - 1)
                        ImGui.SetCursorPosX(ImGui.GetCursorPos().X + space);
                    else
                        ImGui.SetCursorPosX(ImGui.GetCursorPos().X);

                    // Add newline if needed
                    if (split[i].Contains('\n'))
                        ImGui.NewLine();
                }
                // Reset Text Color
                ImGui.PopStyleColor();
            }

            // Itemspacing
            ImGui.PopStyleVar();
        }

        /// <summary>
        /// Displays a blue underlined clickable text wrapped over multiple lines.
        /// </summary>
        public static void ClickLinkWrapped(string text, Action onClick)
        {
            var color = ImGui.ColorConvertFloat4ToU32(new Vector4(.1f, .1f, 1f, 1f));

            var wrapWidth = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
            var space = ImGui.CalcTextSize(" ").X - 2f;

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            // Underline
            var textOffset = new Vector2(0f, ImGui.GetTextLineHeight());
            var start = ImGui.GetCursorScreenPos() + textOffset;

            var split = text.Split(' ');
            for (var i = 0; i < split.Length; i++)
            {
                var wordWidth = ImGui.CalcTextSize(split[i]).X;
                var cursorPos = ImGui.GetCursorPos();

                // If the word is too long to fit on the line, just print it on the next line
                if (cursorPos.X + wordWidth > wrapWidth)
                {
                    ImGui.GetWindowDrawList().AddLine(start, ImGui.GetCursorScreenPos() + textOffset, color);
                    ImGui.NewLine();
                    start = ImGui.GetCursorScreenPos() + textOffset;
                }

                ImGui.Text(split[i]);
                ImGui.SameLine();

                // Remove space at the end
                if (i < split.Length - 1)
                    ImGui.SetCursorPosX(ImGui.GetCursorPos().X + space);
                else
                    ImGui.SetCursorPosX(ImGui.GetCursorPos().X);

                // Add newline if needed
                if (split[i].Contains('\n'))
                {
                    ImGui.GetWindowDrawList().AddLine(start, ImGui.GetCursorScreenPos() + textOffset, color);
                    ImGui.NewLine();
                    start = ImGui.GetCursorScreenPos() + textOffset;
                }

                // Clickable
                if (ImGui.IsItemClicked())
                    onClick();
            }

            ImGui.GetWindowDrawList().AddLine(start, ImGui.GetCursorScreenPos() + textOffset, color);

            ImGui.PopStyleColor(); // POP ImGuiCol.Text = blue
        }

        /// <summary>
        /// Displays a blue underlined clickable text
        /// </summary>
        public static void ClickLink(string text, Action onClick)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(.1f, .1f, 1f, 1f));
            // Underline
            var size = ImGui.CalcTextSize(text);
            var start = ImGui.GetCursorScreenPos() + new Vector2(0f, size.Y);
            var end = start + size - new Vector2(0f, size.Y);
            ImGui.GetWindowDrawList().AddLine(start, end, ImGui.ColorConvertFloat4ToU32(new Vector4(.1f, .1f, 1f, 1f)));

            // Text
            ImGui.TextUnformatted(text);
            if (ImGui.IsItemClicked())
                onClick();

            ImGui.PopStyleColor(); // POP ImGuiCol.Text = blue
        }

      /*/// <summary>
        /// Aligns an image in a rectange. imageSize doesnt have to fit in rectange
        /// </summary>
        /// <param name="imgSize">Size of the object to align</param>
        /// <param name="rectangle">Space to align in</param>
        /// <param name="alignment">How it should be aligned</param>
        /// <returns>Starting position and End position of the aligned image.</returns>
        public static (Vector2, Vector2) AlignImage(Vector2 imgSize, Vector2 rectangle, Configuration.ImageAlignment alignment)
        {
            var imageRatio = imgSize.Y / imgSize.X;
            var rectangeRatio = rectangle.Y / rectangle.X;
            var scaled = new Vector2(rectangle.Y / imageRatio, rectangle.X * imageRatio);
            var widthReduced = rectangeRatio > imageRatio; // True when width of image is bigger than rectangle

            var start = alignment switch
            {
                Configuration.ImageAlignment.TopLeft => Vector2.Zero,
                Configuration.ImageAlignment.Top when widthReduced => Vector2.Zero,
                Configuration.ImageAlignment.Top => new Vector2((rectangle.X - scaled.X) / 2f, 0f),
                Configuration.ImageAlignment.TopRight when widthReduced => Vector2.Zero,
                Configuration.ImageAlignment.TopRight => new Vector2(rectangle.X - scaled.X, 0f),
                Configuration.ImageAlignment.Left when widthReduced => new Vector2(0f, (rectangle.Y - scaled.Y) / 2f),
                Configuration.ImageAlignment.Left => Vector2.Zero,
                Configuration.ImageAlignment.Center when widthReduced => new Vector2(0f, (rectangle.Y - scaled.Y) / 2f),
                Configuration.ImageAlignment.Center => new Vector2((rectangle.X - scaled.X) / 2f, 0f),
                Configuration.ImageAlignment.Right when widthReduced => new Vector2(0f, (rectangle.Y - scaled.Y) / 2f),
                Configuration.ImageAlignment.Right => new Vector2(rectangle.X - scaled.X, 0f),
                Configuration.ImageAlignment.BottomLeft when widthReduced => new Vector2(0f, rectangle.Y - scaled.Y),
                Configuration.ImageAlignment.BottomLeft => Vector2.Zero,
                Configuration.ImageAlignment.Bottom when widthReduced => new Vector2(0f, rectangle.Y - scaled.Y),
                Configuration.ImageAlignment.Bottom => new Vector2((rectangle.X - scaled.X) / 2f, 0f),
                Configuration.ImageAlignment.BottomRight when widthReduced => new Vector2(0f, rectangle.Y - scaled.Y),
                Configuration.ImageAlignment.BottomRight => new Vector2(rectangle.X - scaled.X, 0f),
                _ => Vector2.Zero
            };

            var end = alignment switch
            {
                Configuration.ImageAlignment.TopLeft when widthReduced => new Vector2(rectangle.X, scaled.Y),
                Configuration.ImageAlignment.TopLeft => new Vector2(scaled.X, rectangle.Y),
                Configuration.ImageAlignment.Top when widthReduced => new Vector2(rectangle.X, scaled.Y),
                Configuration.ImageAlignment.Top => new Vector2(rectangle.X - start.X, rectangle.Y),
                Configuration.ImageAlignment.TopRight when widthReduced => new Vector2(rectangle.X, scaled.Y),
                Configuration.ImageAlignment.TopRight => rectangle,
                Configuration.ImageAlignment.Left when widthReduced => new Vector2(rectangle.X, rectangle.Y - start.Y),
                Configuration.ImageAlignment.Left => new Vector2(scaled.X, rectangle.Y),
                Configuration.ImageAlignment.Center when widthReduced => new Vector2(rectangle.X, rectangle.Y - start.Y),
                Configuration.ImageAlignment.Center => new Vector2(rectangle.X - start.X, rectangle.Y),
                Configuration.ImageAlignment.Right when widthReduced => new Vector2(rectangle.X, rectangle.Y - start.Y),
                Configuration.ImageAlignment.Right => rectangle,
                Configuration.ImageAlignment.BottomLeft when widthReduced => rectangle,
                Configuration.ImageAlignment.BottomLeft => new Vector2(scaled.X, rectangle.Y),
                Configuration.ImageAlignment.Bottom when widthReduced => rectangle,
                Configuration.ImageAlignment.Bottom => new Vector2(rectangle.X - start.X, rectangle.Y),
                Configuration.ImageAlignment.BottomRight when widthReduced => rectangle,
                Configuration.ImageAlignment.BottomRight => rectangle,
                _ => rectangle
            };

            Debug.Assert(start.X >= 0 && start.Y >= 0 && end.X >= 0 && end.Y >= 0, "Start and end should be positive");
            Debug.Assert(start.X <= rectangle.X && start.Y <= rectangle.Y && end.X <= rectangle.X && end.Y <= rectangle.Y, "Start and end should be smaller than rectangle");
            return (start, end);
        } */
    }
}