using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;

namespace XIVSlothCombo.Window
{
    public static class IconButtons
    {
        private static Vector2 GetIconSize(FontAwesomeIcon icon)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            var iconSize = ImGui.CalcTextSize(icon.ToIconString());
            ImGui.PopFont();
            return iconSize;
        }

        public static bool IconImageButton(IDalamudTextureWrap texture, string text, Vector2 size = new(), bool imageOnRight = false, float imageScale = 0)
        {
            var buttonClicked = false;

            var buttonSize = Vector2.Zero;
            var imageSize = new Vector2(texture.Width, texture.Height);
            if (imageScale > 0)
            {
                imageSize.X = imageSize.X * imageScale;
                imageSize.Y = imageSize.Y * imageScale;
            }
            var textSize = ImGui.CalcTextSize(text);
            var padding = ImGui.GetStyle().FramePadding;
            var spacing = ImGui.GetStyle().ItemSpacing;

            var buttonSizeX = imageSize.X + textSize.X + padding.X * 2 + spacing.X;
            var buttonSizeY = (imageSize.Y > textSize.Y ? imageSize.Y : textSize.Y) + padding.Y * 2;

            if (size == Vector2.Zero)
            {
                buttonSize = new Vector2(buttonSizeX, buttonSizeY);
            }
            else
            {
                buttonSize = size;
            }

            if (ImGui.Button("###" + text, buttonSize))
            {
                buttonClicked = true;
            }

            ImGui.SameLine();
            if (size == Vector2.Zero)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - buttonSize.X - padding.X);
            }
            else
            {
                ImGui.SetCursorPosX((ImGui.GetContentRegionMax().X - textSize.X - size.X) * 0.5f);
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
            }
            if (imageOnRight)
            {
                ImGui.Text(text);
                ImGui.SameLine();
                if (size != Vector2.Zero)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
                }

                ImGui.Image(texture.ImGuiHandle, imageSize);

            }
            else
            {

                ImGui.Image(texture.ImGuiHandle, imageSize);

                ImGui.SameLine();
                if (size != Vector2.Zero)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
                }
                ImGui.Text(text);
            }


            return buttonClicked;
        }
        public static bool IconImageButton(string imageUrl, string text, Vector2 size = new(), bool imageOnRight = false, float imageScale = 0)
        {
            var buttonClicked = false;

            if (ThreadLoadImageHandler.TryGetTextureWrap(imageUrl, out var texture))
            {
                buttonClicked = IconImageButton(texture, text, size, imageOnRight, imageScale);
            }

            return buttonClicked;
        }
        public static bool IconTextButton(FontAwesomeIcon icon, string text, Vector2 size = new(), bool iconOnRight = false)
        {
            var buttonClicked = false;

            var buttonSize = Vector2.Zero;
            var iconSize = GetIconSize(icon);
            var textSize = ImGui.CalcTextSize(text);
            var padding = ImGui.GetStyle().FramePadding;
            var spacing = ImGui.GetStyle().ItemSpacing;

            var buttonSizeX = iconSize.X + textSize.X + padding.X * 2 + spacing.X;
            var buttonSizeY = (iconSize.Y > textSize.Y ? iconSize.Y : textSize.Y) + padding.Y * 2;
            if (size == Vector2.Zero)
            {
                buttonSize = new Vector2(buttonSizeX, buttonSizeY);
            }
            else
            {
                buttonSize = size;
            }

            if (ImGui.Button("###" + icon.ToIconString() + text, buttonSize))
            {
                buttonClicked = true;
            }

            ImGui.SameLine();
            if (size == Vector2.Zero)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - buttonSize.X - padding.X);
            }
            else
            {
                ImGui.SetCursorPosX((ImGui.GetContentRegionMax().X - textSize.X - iconSize.X) * 0.5f);
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
            }
            if (iconOnRight)
            {
                ImGui.Text(text);
                ImGui.SameLine();
                if (size != Vector2.Zero)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
                }
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.Text(icon.ToIconString());
                ImGui.PopFont();
            }
            else
            {
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.Text(icon.ToIconString());
                ImGui.PopFont();
                ImGui.SameLine();
                if (size != Vector2.Zero)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (padding.Y));
                }
                ImGui.Text(text);
            }


            return buttonClicked;
        }
    }
}
