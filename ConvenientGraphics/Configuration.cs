using Dalamud.Configuration;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;

namespace ConvenientGraphics
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public Dictionary<GroupType, Dictionary<ConfigOption, int>> GraphicsSettings = new Dictionary<GroupType, Dictionary<ConfigOption, int>>();

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }

        public void CheckVersion(int UpdateValue)
        {
            if (Version != UpdateValue)
            {
                SetDefaults();
                Version = UpdateValue;
                Save();
            }
        }


        public void SetDefaults()
        {
            GraphicsSettings.Clear();

            GraphicsSettings[GroupType.Standard] = new Dictionary<ConfigOption, int>();
            GraphicsSettings[GroupType.Standard][ConfigOption.CharaLight] = 30;
            GraphicsSettings[GroupType.Standard][ConfigOption.Gamma] = 30;
            GraphicsSettings[GroupType.Standard][ConfigOption.ReflectionType_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.ParallaxOcclusion_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.TextureFilterQuality_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.TextureAnisotropicQuality_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.Vignetting_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.SSAO_DX11] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.DisplayObjectLimitType] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.MouseOpeLimit] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.MoveMode] = 0;
            GraphicsSettings[GroupType.Standard][ConfigOption.ObjectBorderingType] = 0;
            GraphicsSettings[GroupType.Standard][(ConfigOption)ConfigOptionLocal.UseChillframes] = 1;
            GraphicsSettings[GroupType.Standard][(ConfigOption)ConfigOptionLocal.SetHudLayout] = 0;
            GraphicsSettings[GroupType.Standard][(ConfigOption)ConfigOptionLocal.ShowNameplates] = 3;

            GraphicsSettings[GroupType.InDuty] = new Dictionary<ConfigOption, int>();
            GraphicsSettings[GroupType.InDuty][ConfigOption.CharaLight] = 20;
            GraphicsSettings[GroupType.InDuty][ConfigOption.Gamma] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.ReflectionType_DX11] = 3;
            GraphicsSettings[GroupType.InDuty][ConfigOption.ParallaxOcclusion_DX11] = 1;
            GraphicsSettings[GroupType.InDuty][ConfigOption.TextureFilterQuality_DX11] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.TextureAnisotropicQuality_DX11] = 2;
            GraphicsSettings[GroupType.InDuty][ConfigOption.Vignetting_DX11] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.SSAO_DX11] = 4;
            GraphicsSettings[GroupType.InDuty][ConfigOption.DisplayObjectLimitType] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.MouseOpeLimit] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.MoveMode] = 0;
            GraphicsSettings[GroupType.InDuty][ConfigOption.ObjectBorderingType] = 0;
            GraphicsSettings[GroupType.InDuty][(ConfigOption)ConfigOptionLocal.UseChillframes] = 1;
            GraphicsSettings[GroupType.InDuty][(ConfigOption)ConfigOptionLocal.SetHudLayout] = 0;
            GraphicsSettings[GroupType.InDuty][(ConfigOption)ConfigOptionLocal.ShowNameplates] = 3;

            GraphicsSettings[GroupType.VR] = new Dictionary<ConfigOption, int>();
            GraphicsSettings[GroupType.VR][ConfigOption.CharaLight] = 20;
            GraphicsSettings[GroupType.VR][ConfigOption.Gamma] = 0;
            GraphicsSettings[GroupType.VR][ConfigOption.ReflectionType_DX11] = 3;
            GraphicsSettings[GroupType.VR][ConfigOption.ParallaxOcclusion_DX11] = 1;
            GraphicsSettings[GroupType.VR][ConfigOption.TextureFilterQuality_DX11] = 0;
            GraphicsSettings[GroupType.VR][ConfigOption.TextureAnisotropicQuality_DX11] = 2;
            GraphicsSettings[GroupType.VR][ConfigOption.Vignetting_DX11] = 0;
            GraphicsSettings[GroupType.VR][ConfigOption.SSAO_DX11] = 4;
            GraphicsSettings[GroupType.VR][ConfigOption.DisplayObjectLimitType] = 2;
            GraphicsSettings[GroupType.VR][ConfigOption.MouseOpeLimit] = 1;
            GraphicsSettings[GroupType.VR][ConfigOption.MoveMode] = 1;
            GraphicsSettings[GroupType.VR][ConfigOption.ObjectBorderingType] = 1;
            GraphicsSettings[GroupType.VR][(ConfigOption)ConfigOptionLocal.UseChillframes] = 0;
            GraphicsSettings[GroupType.VR][(ConfigOption)ConfigOptionLocal.SetHudLayout] = 0;
            GraphicsSettings[GroupType.VR][(ConfigOption)ConfigOptionLocal.ShowNameplates] = 3;

            GraphicsSettings[GroupType.VRInCapital] = new Dictionary<ConfigOption, int>();
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.CharaLight] = 20;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.Gamma] = 0;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.ReflectionType_DX11] = 3;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.ParallaxOcclusion_DX11] = 1;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.TextureFilterQuality_DX11] = 0;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.TextureAnisotropicQuality_DX11] = 2;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.Vignetting_DX11] = 0;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.SSAO_DX11] = 4;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.DisplayObjectLimitType] = 3;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.MouseOpeLimit] = 1;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.MoveMode] = 1;
            GraphicsSettings[GroupType.VRInCapital][ConfigOption.ObjectBorderingType] = 1;
            GraphicsSettings[GroupType.VRInCapital][(ConfigOption)ConfigOptionLocal.UseChillframes] = 0;
            GraphicsSettings[GroupType.VRInCapital][(ConfigOption)ConfigOptionLocal.SetHudLayout] = 0;
            GraphicsSettings[GroupType.VRInCapital][(ConfigOption)ConfigOptionLocal.ShowNameplates] = 3;
        }
    }
}
