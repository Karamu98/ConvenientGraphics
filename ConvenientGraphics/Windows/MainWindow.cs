using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface.Windowing;
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


    private void setCombo(string[] optionList, ref uint optionValue)
    {
        for (uint n = 0; n < optionList.Length; n++)
        {
            bool is_selected = (optionValue == n);
            if (ImGui.Selectable($"{optionList[n]} ({n})", is_selected))
                optionValue = n;
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
            uint Fps = self.cfg.GraphicsSettings[curGroup]["Fps"];
            bool MouseOpeLimit = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["MouseOpeLimit"]);
            int Gamma = (int)self.cfg.GraphicsSettings[curGroup]["Gamma"];
            int CharaLight = (int)self.cfg.GraphicsSettings[curGroup]["CharaLight"];
            uint DisplayObjectLimitType = self.cfg.GraphicsSettings[curGroup]["DisplayObjectLimitType"];
            uint TextureAnisotropicQuality_DX11 = self.cfg.GraphicsSettings[curGroup]["TextureAnisotropicQuality_DX11"];
            uint SSAO_DX11 = self.cfg.GraphicsSettings[curGroup]["SSAO_DX11"];
            bool Vignetting_DX11 = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["Vignetting_DX11"]);
            uint GrassQuality_DX11 = self.cfg.GraphicsSettings[curGroup]["GrassQuality_DX11"];
            bool ShadowLOD_DX11 = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["ShadowLOD_DX11"]);
            uint ShadowVisibilityTypeOther_DX11 = self.cfg.GraphicsSettings[curGroup]["ShadowVisibilityTypeOther_DX11"];
            uint ShadowVisibilityTypeEnemy_DX11 = self.cfg.GraphicsSettings[curGroup]["ShadowVisibilityTypeEnemy_DX11"];
            uint PhysicsTypeOther_DX11 = self.cfg.GraphicsSettings[curGroup]["PhysicsTypeOther_DX11"];
            uint PhysicsTypeEnemy_DX11 = self.cfg.GraphicsSettings[curGroup]["PhysicsTypeEnemy_DX11"];
            uint ReflectionType_DX11 = self.cfg.GraphicsSettings[curGroup]["ReflectionType_DX11"];
            uint ParallaxOcclusion_DX11 = self.cfg.GraphicsSettings[curGroup]["ParallaxOcclusion_DX11"];
            uint DynamicRezoThreshold = self.cfg.GraphicsSettings[curGroup]["DynamicRezoThreshold"];
            int GraphicsRezoScale = (int)self.cfg.GraphicsSettings[curGroup]["GraphicsRezoScale"];
            uint GraphicsRezoUpscaleType = self.cfg.GraphicsSettings[curGroup]["GraphicsRezoUpscaleType"];
            bool ShadowBgLOD = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["ShadowBgLOD"]);
            bool DynamicRezoType = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["DynamicRezoType"]);
            uint BattleEffectParty = self.cfg.GraphicsSettings[curGroup]["BattleEffectParty"];
            uint BattleEffectOther = self.cfg.GraphicsSettings[curGroup]["BattleEffectOther"];
            uint FPSCameraInterpolationType = self.cfg.GraphicsSettings[curGroup]["FPSCameraInterpolationType"];
            bool EventCameraAutoControl = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["EventCameraAutoControl"]);
            uint ShowNameplates = self.cfg.GraphicsSettings[curGroup]["NamePlateDispTypeOther"];
            bool ObjectBorderingType = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["ObjectBorderingType"]);
            uint MoveMode = self.cfg.GraphicsSettings[curGroup]["MoveMode"];
            bool UseChillframes = Convert.ToBoolean(self.cfg.GraphicsSettings[curGroup]["_UseChillframes"]);
            uint HUDLayout = self.cfg.GraphicsSettings[curGroup]["_SetHudLayout"];


            string[] optionsFps = { "None", "Main Display", "60fps", "30fps" };
            string[] optionsDisplayObjectLimitType = { "Maximum", "High", "Normal", "Low", "Minimum" };
            string[] optionsTextureAnisotropicQuality_DX11 = { "x4", "x8", "x16" };
            string[] optionsSSAO_DX11 = { "Off", "Light", "Strong", "HBAO+: Standard", "HBAO+: Quality", "GTAO: Standard", "GTAO: Quality" };
            string[] optionsGrassQuality_DX11 = { "Off", "Low", "Normal", "High" };
            string[] optionsShadowVisibilityType = { "Hide", "Display" };
            string[] optionsPhysicsType = { "Off", "Simple", "Full" };
            string[] optionsReflectionType_DX11 = { "Off", "Standard", "High", "Maximum" };
            string[] optionsParallaxOcclusion_DX11 = { "Normal", "High" };
            string[] optionsDynamicRezoThreshold = { "Always Enabled", "Below 30 fps", "Below 60 fps" };
            string[] optionsGraphicsRezoUpscaleType = { "AMD FSR", "NVIDIA DLSS" };
            string[] optionsBattleEffect = { "Show All", "Show Limited", "Show None" };
            string[] optionsFPSCameraInterpolationType = { "Only When Moving", "Always", "Never" };
            string[] optionsShowNameplates = { "Always", "During Battle", "When Targeted", "Never", "Out of Battle" };
            string[] optionsMoveMode = { "Standard", "Legacy" };
            string[] optionsHUDLayout = { "Keep Current Layout", "Layout 1", "Layout 2", "Layout 3", "Layout 4" };
            

            int optionWidth = 200;

            ImGui.Text($"Current Saved Settings: {curGroup}");
            ImGui.BeginChild($"##{curGroup}", new Vector2(500, 450), true);

            ImGui.Text("HUD Layout"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SetHudLayout" + curGroup, optionsHUDLayout[HUDLayout]))
            {
                setCombo(optionsHUDLayout, ref HUDLayout);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["_SetHudLayout"] = HUDLayout;
            }

            ImGui.Text("Fps"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##Fps" + curGroup, optionsFps[Fps]))
            {
                setCombo(optionsFps, ref Fps);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["Fps"] = Fps;
            }

            //-----

            ImGui.Text("Movement Settings"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##MoveMode" + curGroup, optionsMoveMode[MoveMode]))
            {
                setCombo(optionsMoveMode, ref MoveMode);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["MoveMode"] = MoveMode;
            }

            ImGui.Text("Other PC Nameplates"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ShowNameplates" + curGroup, optionsShowNameplates[ShowNameplates]))
            {
                setCombo(optionsShowNameplates, ref ShowNameplates);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["NamePlateDispTypeOther"] = ShowNameplates;
            }

            //----

            ImGui.Text("Shadow LOD"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##ShadowLOD_DX11", ref ShadowLOD_DX11))
                self.cfg.GraphicsSettings[curGroup]["ShadowLOD_DX11"] = Convert.ToUInt32(ShadowLOD_DX11);

            ImGui.Text("Shadow LOD Distant"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##ShadowBgLOD", ref ShadowBgLOD))
                self.cfg.GraphicsSettings[curGroup]["ShadowBgLOD"] = Convert.ToUInt32(ShadowBgLOD);

            ImGui.Text("Shadows Other NPCs"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ShadowVisibilityTypeOther_DX11" + curGroup, optionsShadowVisibilityType[ShadowVisibilityTypeOther_DX11]))
            {
                setCombo(optionsShadowVisibilityType, ref ShadowVisibilityTypeOther_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ShadowVisibilityTypeOther_DX11"] = ShadowVisibilityTypeOther_DX11;
            }

            ImGui.Text("Shadows Enemy"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ShadowVisibilityTypeEnemy_DX11" + curGroup, optionsShadowVisibilityType[ShadowVisibilityTypeEnemy_DX11]))
            {
                setCombo(optionsShadowVisibilityType, ref ShadowVisibilityTypeEnemy_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ShadowVisibilityTypeEnemy_DX11"] = ShadowVisibilityTypeEnemy_DX11;
            }

            //----

            ImGui.Text("Movement Physics Other"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##PhysicsTypeOther_DX11" + curGroup, optionsPhysicsType[PhysicsTypeOther_DX11]))
            {
                setCombo(optionsPhysicsType, ref PhysicsTypeOther_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["PhysicsTypeOther_DX11"] = PhysicsTypeOther_DX11;
            }

            ImGui.Text("Movement Physics Enemy"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##PhysicsTypeEnemy_DX11" + curGroup, optionsPhysicsType[PhysicsTypeEnemy_DX11]))
            {
                setCombo(optionsPhysicsType, ref PhysicsTypeEnemy_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["PhysicsTypeEnemy_DX11"] = PhysicsTypeEnemy_DX11;
            }

            ImGui.Text("Battle Effects Party"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##BattleEffectParty" + curGroup, optionsBattleEffect[BattleEffectParty]))
            {
                setCombo(optionsBattleEffect, ref BattleEffectParty);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["BattleEffectParty"] = BattleEffectParty;
            }

            ImGui.Text("Battle Effects Other (excl. PvP)"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##BattleEffectOther" + curGroup, optionsBattleEffect[BattleEffectOther]))
            {
                setCombo(optionsBattleEffect, ref BattleEffectOther);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["BattleEffectOther"] = BattleEffectOther;
            }

            ImGui.Text("1st Person Camera Auto Adjust"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##FPSCameraInterpolationType" + curGroup, optionsFPSCameraInterpolationType[FPSCameraInterpolationType]))
            {
                setCombo(optionsFPSCameraInterpolationType, ref FPSCameraInterpolationType);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["FPSCameraInterpolationType"] = FPSCameraInterpolationType;
            }

            ImGui.Text("Look at target when speaking"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##EventCameraAutoControl", ref EventCameraAutoControl))
                self.cfg.GraphicsSettings[curGroup]["EventCameraAutoControl"] = Convert.ToUInt32(EventCameraAutoControl);

            //----

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

            //----

            ImGui.Text("Upscaling"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##GraphicsRezoUpscaleType" + curGroup, optionsGraphicsRezoUpscaleType[GraphicsRezoUpscaleType]))
            {
                setCombo(optionsGraphicsRezoUpscaleType, ref GraphicsRezoUpscaleType);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["GraphicsRezoUpscaleType"] = GraphicsRezoUpscaleType;
            }

            ImGui.Text("Resolution Scale"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.SliderInt("##GraphicsRezoScale" + curGroup, ref GraphicsRezoScale, 50, 100))
                self.cfg.GraphicsSettings[curGroup]["GraphicsRezoScale"] = (uint)GraphicsRezoScale;

            ImGui.Text("Enable Dynamic Resolution"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##DynamicRezoType", ref DynamicRezoType))
                self.cfg.GraphicsSettings[curGroup]["DynamicRezoType"] = Convert.ToUInt32(DynamicRezoType);

            ImGui.Text("Framerate Threshold"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##DynamicRezoThreshold" + curGroup, optionsDynamicRezoThreshold[DynamicRezoThreshold]))
            {
                setCombo(optionsDynamicRezoThreshold, ref DynamicRezoThreshold);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["DynamicRezoThreshold"] = DynamicRezoThreshold;
            }

            ImGui.Text("Real-time Reflections"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ReflectionType_DX11" + curGroup, optionsReflectionType_DX11[ReflectionType_DX11]))
            {
                setCombo(optionsReflectionType_DX11, ref ReflectionType_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ReflectionType_DX11"] = ReflectionType_DX11;
            }

            ImGui.Text("Parallax Occlusion"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##ParallaxOcclusion_DX11" + curGroup, optionsParallaxOcclusion_DX11[ParallaxOcclusion_DX11]))
            {
                setCombo(optionsParallaxOcclusion_DX11, ref ParallaxOcclusion_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["ParallaxOcclusion_DX11"] = ParallaxOcclusion_DX11;
            }

            ImGui.Text("Anisotropic Filtering"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##TextureAnisotropicQuality_DX11" + curGroup, optionsTextureAnisotropicQuality_DX11[TextureAnisotropicQuality_DX11]))
            {
                setCombo(optionsTextureAnisotropicQuality_DX11, ref TextureAnisotropicQuality_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["TextureAnisotropicQuality_DX11"] = TextureAnisotropicQuality_DX11;
            }

            ImGui.Text("Grass Quality"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##GrassQuality_DX11" + curGroup, optionsGrassQuality_DX11[GrassQuality_DX11]))
            {
                setCombo(optionsGrassQuality_DX11, ref GrassQuality_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["GrassQuality_DX11"] = GrassQuality_DX11;
            }

            ImGui.Text("Naturally darken edge of screen"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.Checkbox("##Vignetting_DX11", ref Vignetting_DX11))
                self.cfg.GraphicsSettings[curGroup]["Vignetting_DX11"] = Convert.ToUInt32(Vignetting_DX11);

            ImGui.Text("SSAO"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##SSAO_DX11" + curGroup, optionsSSAO_DX11[SSAO_DX11]))
            {
                setCombo(optionsSSAO_DX11, ref SSAO_DX11);
                ImGui.EndCombo();
                self.cfg.GraphicsSettings[curGroup]["SSAO_DX11"] = SSAO_DX11;
            }

            ImGui.Text("Character and Object Quantity"); ImGui.SameLine(); ImGui.SetNextItemWidth(optionWidth);
            if (ImGui.BeginCombo("##DisplayObjectLimitType" + curGroup, optionsDisplayObjectLimitType[DisplayObjectLimitType]))
            {
                setCombo(optionsDisplayObjectLimitType, ref DisplayObjectLimitType);
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
