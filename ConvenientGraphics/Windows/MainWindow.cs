using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;

namespace ConvenientGraphics.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin self;
    private GroupType drawGroup = GroupType.Standard;
    public MainWindow(Plugin self) : base("Graphics Settings")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.self = self;
    }

    public void Dispose()
    {
    }


    private void setCombo(string[] optionList, bool reverse, ref uint optionValue)
    {
        for (uint n = 0; n < optionList.Length; n++)
        {
            uint r = reverse ? (uint)(optionList.Length - 1) - n : n;
            bool is_selected = (optionValue == r);
            if (ImGui.Selectable(optionList[r], is_selected))
                optionValue = r;
            if (is_selected)
                ImGui.SetItemDefaultFocus();
        }
    }

    public override void Draw()
    {
        ShowKofi();

        ImGui.BeginChild($"##buttoms", new Vector2(500, 50), true);
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
        if (self.cfg.GraphicsSettings.ContainsKey(curGroup))
        {
            int CharaLight = (int)self.cfg.GraphicsSettings[curGroup]["CharaLight"];
            int Gamma = (int)self.cfg.GraphicsSettings[curGroup]["Gamma"];
            uint ReflectionType_DX11 = self.cfg.GraphicsSettings[curGroup]["ReflectionType_DX11"];
            uint ParallaxOcclusion_DX11 = self.cfg.GraphicsSettings[curGroup]["ParallaxOcclusion_DX11"];
            uint TextureFilterQuality_DX11 = self.cfg.GraphicsSettings[curGroup]["TextureFilterQuality_DX11"];
            uint TextureAnisotropicQuality_DX11 = self.cfg.GraphicsSettings[curGroup]["TextureAnisotropicQuality_DX11"];
            bool Vignetting_DX11 = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["Vignetting_DX11"]);
            uint SSAO_DX11 = self.cfg.GraphicsSettings[curGroup]["SSAO_DX11"];
            uint DisplayObjectLimitType = self.cfg.GraphicsSettings[curGroup]["DisplayObjectLimitType"];
            bool MouseOpeLimit = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["MouseOpeLimit"]);
            uint MoveMode = self.cfg.GraphicsSettings[curGroup]["MoveMode"];
            bool ObjectBorderingType = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["ObjectBorderingType"]);
            uint ShowNameplates = self.cfg.GraphicsSettings[curGroup]["NamePlateDispTypeOther"];

            bool UseChillframes = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["_UseChillframes"]);
            uint HUDLayout = self.cfg.GraphicsSettings[curGroup]["_SetHudLayout"];

            string[] optionsReflectionType_DX11 = { "Off", "Standard", "High", "Maximum" };
            string[] optionsTranslucentQuality_DX11 = { "Normal", "High" };
            string[] optionsParallaxOcclusion_DX11 = { "Normal", "High" };
            string[] optionsTextureFilterQuality_DX11 = { "Bilinear", "Trilinear", "Anisotropic" };
            string[] optionsTextureAnisotropicQuality_DX11 = { "x4", "x8", "x16" };
            string[] optionsSSAO_DX11 = { "Off", "Light", "Strong", "HBAO+: Standard", "HBAO+: Quality" };
            string[] optionsDisplayObjectLimitType = { "Maximum", "High", "Normal", "Low", "Minimum" };
            string[] optionsMoveMode = { "Standard", "Legacy" };
            string[] optionsShowNameplates = { "Always", "During Battle", "When Targeted", "Never", "Out of Battle" };
            string[] optionsHUDLayout = { "Keep Current Layout", "Layout 1", "Layout 2", "Layout 3", "Layout 4" };

            int optionWidth = 200;

            ImGui.Text($"Current Saved Settings: {curGroup}");
            ImGui.BeginChild($"##{curGroup}", new Vector2(500, 450), true);

            ImGui.Text("HUD Layout"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SetHudLayout" + curGroup, optionsHUDLayout[HUDLayout]))
            {
                setCombo(optionsHUDLayout, false, ref HUDLayout);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["_SetHudLayout"] = HUDLayout;
            }

            ImGui.Text("Movement Settings"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##MoveMode" + curGroup, optionsMoveMode[MoveMode]))
            {
                setCombo(optionsMoveMode, false, ref MoveMode);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["MoveMode"] = MoveMode;
            }

            ImGui.Text("Other PC Nameplates"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ShowNameplates" + curGroup, optionsShowNameplates[ShowNameplates]))
            {
                setCombo(optionsShowNameplates, false, ref ShowNameplates);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["NamePlateDispTypeOther"] = ShowNameplates;
            }

            ImGui.Text("Highlight Targets"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##ObjectBorderingType", ref ObjectBorderingType))
                self.cfg.GraphicsSettings[curGroup]["ObjectBorderingType"] = Convert.ToUInt32(ObjectBorderingType);

            ImGui.Text("Character Lighting"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.SliderInt("##CharaLight" + curGroup, ref CharaLight, 0, 100))
                self.cfg.GraphicsSettings[curGroup]["CharaLight"] = (uint)CharaLight;

            ImGui.Text("Gamma Correction"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.SliderInt("##Gamma" + curGroup, ref Gamma, 0, 100))
                self.cfg.GraphicsSettings[curGroup]["Gamma"] = (Gamma == 50) ? 49 : (uint)Gamma;

            ImGui.Text("Lock Mouse to Game"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##MouseOpeLimit", ref MouseOpeLimit))
                self.cfg.GraphicsSettings[curGroup]["MouseOpeLimit"] = Convert.ToUInt32(MouseOpeLimit);

            ImGui.Text("Real-time Reflections"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ReflectionType_DX11" + curGroup, optionsReflectionType_DX11[ReflectionType_DX11]))
            {
                setCombo(optionsReflectionType_DX11, true, ref ReflectionType_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ReflectionType_DX11"] = ReflectionType_DX11;
            }

            ImGui.Text("Parallax Occlusion"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ParallaxOcclusion_DX11" + curGroup, optionsParallaxOcclusion_DX11[ParallaxOcclusion_DX11]))
            {
                setCombo(optionsParallaxOcclusion_DX11, true, ref ParallaxOcclusion_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ParallaxOcclusion_DX11"] = ParallaxOcclusion_DX11;
            }

            ImGui.Text("Texture Filtering"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##TextureFilterQuality_DX11" + curGroup, optionsTextureFilterQuality_DX11[TextureFilterQuality_DX11]))
            {
                setCombo(optionsTextureFilterQuality_DX11, true, ref TextureFilterQuality_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["TextureFilterQuality_DX11"] = TextureFilterQuality_DX11;
            }

            ImGui.Text("Anisotropic Filtering"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##TextureAnisotropicQuality_DX11" + curGroup, optionsTextureAnisotropicQuality_DX11[TextureAnisotropicQuality_DX11]))
            {
                setCombo(optionsTextureAnisotropicQuality_DX11, true, ref TextureAnisotropicQuality_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["TextureAnisotropicQuality_DX11"] = TextureAnisotropicQuality_DX11;
            }

            ImGui.Text("Naturally darken edge of screen"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##Vignetting_DX11", ref Vignetting_DX11))
                self.cfg.GraphicsSettings[curGroup]["Vignetting_DX11"] = Convert.ToUInt32(Vignetting_DX11);

            ImGui.Text("SSAO"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SSAO_DX11" + curGroup, optionsSSAO_DX11[SSAO_DX11]))
            {
                setCombo(optionsSSAO_DX11, true, ref SSAO_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["SSAO_DX11"] = SSAO_DX11;
            }

            ImGui.Text("Character and Object Quantity"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##DisplayObjectLimitType" + curGroup, optionsDisplayObjectLimitType[DisplayObjectLimitType]))
            {
                setCombo(optionsDisplayObjectLimitType, false, ref DisplayObjectLimitType);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["DisplayObjectLimitType"] = DisplayObjectLimitType;
            }

            ImGui.Text("Use Chillframes for fps limit"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##UseChillframes", ref UseChillframes))
                self.cfg.GraphicsSettings[curGroup]["_UseChillframes"] = Convert.ToUInt32(UseChillframes);

            ImGui.EndChild();

            if (ImGui.Button("Update"))
            {
                this.self.cfg.Save();
                this.self.Reset();
            }
        }
    }

    public static void ShowKofi()
    {
        ImGui.BeginChild("Support", new Vector2(500, 50), true);

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
