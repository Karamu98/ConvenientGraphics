using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;

namespace ConvenientGraphics.Windows;

public class MainWindow : Window, IDisposable
{
    private ConvenientGraphics Plugin;
    private GroupType drawGroup = GroupType.Standard;
    public MainWindow(ConvenientGraphics plugin) : base("Graphics Settings")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
    }

    public void Dispose()
    {
    }


    private void setCombo(string[] optionList, ref int optionValue)
    {
        for (int n = 0; n < optionList.Length; n++)
        {
            bool is_selected = (optionValue == n);
            if (ImGui.Selectable(optionList[n], is_selected))
                optionValue = n;
            if (is_selected)
                ImGui.SetItemDefaultFocus();
        }
    }

    public override void Draw()
    {
        ShowKofi();

        ImGui.BeginChild($"##buttoms", new Vector2(500, 50) * ImGuiHelpers.GlobalScale, true);
        foreach (GroupType i in Enum.GetValues(typeof(GroupType)))
        {
            ImGui.SameLine();
            if (ImGui.Button($"{i}", new Vector2(100, 25)))
                drawGroup = i;
        }
        ImGui.EndChild();

        DrawGroup(drawGroup);
    }

    public void DrawGroup(GroupType curGroup)
    {
        if (Plugin.cfg.GraphicsSettings.ContainsKey(curGroup))
        {
            int CharaLight = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.CharaLight];
            int Gamma = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.Gamma];
            int ReflectionType_DX11 = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ReflectionType_DX11];
            int ParallaxOcclusion_DX11 = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ParallaxOcclusion_DX11];
            int TextureFilterQuality_DX11 = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.TextureFilterQuality_DX11];
            int TextureAnisotropicQuality_DX11 = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.TextureAnisotropicQuality_DX11];
            bool Vignetting_DX11 = Convert.ToBoolean(Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.Vignetting_DX11]);
            int SSAO_DX11 = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.SSAO_DX11];
            int DisplayObjectLimitType = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.DisplayObjectLimitType];
            bool MouseOpeLimit = Convert.ToBoolean(Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.MouseOpeLimit]);
            int MoveMode = Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.MoveMode];
            bool ObjectBorderingType = Convert.ToBoolean(Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ObjectBorderingType]);

            bool UseChillframes = Convert.ToBoolean(Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.UseChillframes]);
            int HUDLayout = Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.SetHudLayout];
            int ShowNameplates = Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.ShowNameplates];

            string[] optionsReflectionType_DX11 = { "Maximum", "High", "Standard", "Off" };
            string[] optionsTranslucentQuality_DX11 = { "High", "Normal" };
            string[] optionsParallaxOcclusion_DX11 = { "High", "Normal" };
            string[] optionsTextureFilterQuality_DX11 = { "Anisotropic", "Trilinear", "Bilinear" };
            string[] optionsTextureAnisotropicQuality_DX11 = { "x16", "x8", "x4" };
            string[] optionsSSAO_DX11 = { "HBAO+: Quality", "HBAO+: Standard", "Strong", "Light", "Off" };
            string[] optionsDisplayObjectLimitType = { "Maximum", "High", "Normal", "Low", "Minimum" };
            string[] optionsMoveMode = { "Standard", "Legacy" };
            string[] optionsShowNameplates = { "Always", "During Battle", "Out of Battle", "When Targeted", "Never" };
            string[] optionsHUDLayout = { "Keep Current Layout", "Layout 1", "Layout 2", "Layout 3", "Layout 4" };

            int optionWidth = 200;

            ImGui.Text($"Current Saved Settings: {curGroup}");
            ImGui.BeginChild($"##{curGroup}", new Vector2(500, 450) * ImGuiHelpers.GlobalScale, true);

            ImGui.Text("HUD Layout"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SetHudLayout" + curGroup, optionsHUDLayout[HUDLayout]))
            {
                setCombo(optionsHUDLayout, ref HUDLayout);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.SetHudLayout] = HUDLayout;
            }

            ImGui.Text("Movement Settings"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##MoveMode" + curGroup, optionsMoveMode[MoveMode]))
            {
                setCombo(optionsMoveMode, ref MoveMode);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.MoveMode] = MoveMode;
            }

            ImGui.Text("Other PC Nameplates"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ShowNameplates" + curGroup, optionsShowNameplates[ShowNameplates]))
            {
                setCombo(optionsShowNameplates, ref ShowNameplates);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.ShowNameplates] = ShowNameplates;
            }

            ImGui.Text("Highlight Targets"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##ObjectBorderingType", ref ObjectBorderingType))
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ObjectBorderingType] = Convert.ToInt32(ObjectBorderingType);

            ImGui.Text("Character Lighting"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.SliderInt("##CharaLight" + curGroup, ref CharaLight, 0, 100))
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.CharaLight] = CharaLight;

            ImGui.Text("Gamma Correction"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.SliderInt("##Gamma" + curGroup, ref Gamma, 0, 100))
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.Gamma] = (Gamma == 50) ? 49 : Gamma;

            ImGui.Text("Lock Mouse to Game"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##MouseOpeLimit", ref MouseOpeLimit))
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.MouseOpeLimit] = Convert.ToInt32(MouseOpeLimit);

            ImGui.Text("Real-time Reflections"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ReflectionType_DX11" + curGroup, optionsReflectionType_DX11[ReflectionType_DX11]))
            {
                setCombo(optionsReflectionType_DX11, ref ReflectionType_DX11);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ReflectionType_DX11] = ReflectionType_DX11;
            }

            ImGui.Text("Parallax Occlusion"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ParallaxOcclusion_DX11" + curGroup, optionsParallaxOcclusion_DX11[ParallaxOcclusion_DX11]))
            {
                setCombo(optionsParallaxOcclusion_DX11, ref ParallaxOcclusion_DX11);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.ParallaxOcclusion_DX11] = ParallaxOcclusion_DX11;
            }

            ImGui.Text("Texture Filtering"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##TextureFilterQuality_DX11" + curGroup, optionsTextureFilterQuality_DX11[TextureFilterQuality_DX11]))
            {
                setCombo(optionsTextureFilterQuality_DX11, ref TextureFilterQuality_DX11);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.TextureFilterQuality_DX11] = TextureFilterQuality_DX11;
            }

            ImGui.Text("Anisotropic Filtering"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##TextureAnisotropicQuality_DX11" + curGroup, optionsTextureAnisotropicQuality_DX11[TextureAnisotropicQuality_DX11]))
            {
                setCombo(optionsTextureAnisotropicQuality_DX11, ref TextureAnisotropicQuality_DX11);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.TextureAnisotropicQuality_DX11] = TextureAnisotropicQuality_DX11;
            }

            ImGui.Text("Naturally darken edge of screen"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##Vignetting_DX11", ref Vignetting_DX11))
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.Vignetting_DX11] = Convert.ToInt32(Vignetting_DX11);

            ImGui.Text("SSAO"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SSAO_DX11" + curGroup, optionsSSAO_DX11[SSAO_DX11]))
            {
                setCombo(optionsSSAO_DX11, ref SSAO_DX11);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.SSAO_DX11] = SSAO_DX11;
            }

            ImGui.Text("Character and Object Quantity"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##DisplayObjectLimitType" + curGroup, optionsDisplayObjectLimitType[DisplayObjectLimitType]))
            {
                setCombo(optionsDisplayObjectLimitType, ref DisplayObjectLimitType);
                ImGui.EndCombo();
                Plugin.cfg.GraphicsSettings[curGroup][ConfigOption.DisplayObjectLimitType] = DisplayObjectLimitType;
            }

            ImGui.Text("Use Chillframes for fps limit"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##UseChillframes", ref UseChillframes))
                Plugin.cfg.GraphicsSettings[curGroup][(ConfigOption)ConfigOptionLocal.UseChillframes] = Convert.ToInt32(UseChillframes);

            ImGui.EndChild();

            if (ImGui.Button("Update"))
            {
                this.Plugin.cfg.Save();
                this.Plugin.Stop();
                this.Plugin.Start();
            }
        }
    }

    public static void ShowKofi()
    {
        ImGui.BeginChild("Support", new Vector2(500, 50) * ImGuiHelpers.GlobalScale, true);

        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        if (ImGui.Button("Support via Ko-fi"))
        {
            Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/projectmimer", UseShellExecute = true });
        }
        ImGui.PopStyleColor(3);
        ImGui.EndChild();
    }
}
