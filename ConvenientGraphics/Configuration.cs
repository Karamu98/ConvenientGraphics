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
        public Dictionary<GroupType, Dictionary<string, uint>> GraphicsSettings = new Dictionary<GroupType, Dictionary<string, uint>>();

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

            GraphicsSettings[GroupType.Standard] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.Standard]["CharaLight"] = 30;
            GraphicsSettings[GroupType.Standard]["Gamma"] = 30;
            GraphicsSettings[GroupType.Standard]["ReflectionType_DX11"] = 3;
            GraphicsSettings[GroupType.Standard]["ParallaxOcclusion_DX11"] = 1;
            GraphicsSettings[GroupType.Standard]["TextureFilterQuality_DX11"] = 2;
            GraphicsSettings[GroupType.Standard]["TextureAnisotropicQuality_DX11"] = 2;
            GraphicsSettings[GroupType.Standard]["Vignetting_DX11"] = 0;
            GraphicsSettings[GroupType.Standard]["SSAO_DX11"] = 4;
            GraphicsSettings[GroupType.Standard]["DisplayObjectLimitType"] = 0;
            GraphicsSettings[GroupType.Standard]["MouseOpeLimit"] = 0;
            GraphicsSettings[GroupType.Standard]["MoveMode"] = 0;
            GraphicsSettings[GroupType.Standard]["ObjectBorderingType"] = 0;
            GraphicsSettings[GroupType.Standard]["NamePlateDispTypeOther"] = 2;
            GraphicsSettings[GroupType.Standard]["_UseChillframes"] = 1;
            GraphicsSettings[GroupType.Standard]["_SetHudLayout"] = 0;

            GraphicsSettings[GroupType.InDuty] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.InDuty]["CharaLight"] = 20;
            GraphicsSettings[GroupType.InDuty]["Gamma"] = 0;
            GraphicsSettings[GroupType.InDuty]["ReflectionType_DX11"] = 0;
            GraphicsSettings[GroupType.InDuty]["ParallaxOcclusion_DX11"] = 0;
            GraphicsSettings[GroupType.InDuty]["TextureFilterQuality_DX11"] = 2;
            GraphicsSettings[GroupType.InDuty]["TextureAnisotropicQuality_DX11"] = 0;
            GraphicsSettings[GroupType.InDuty]["Vignetting_DX11"] = 0;
            GraphicsSettings[GroupType.InDuty]["SSAO_DX11"] = 0;
            GraphicsSettings[GroupType.InDuty]["DisplayObjectLimitType"] = 4;
            GraphicsSettings[GroupType.InDuty]["MouseOpeLimit"] = 0;
            GraphicsSettings[GroupType.InDuty]["MoveMode"] = 0;
            GraphicsSettings[GroupType.InDuty]["ObjectBorderingType"] = 0;
            GraphicsSettings[GroupType.InDuty]["NamePlateDispTypeOther"] = 2;
            GraphicsSettings[GroupType.InDuty]["_UseChillframes"] = 1;
            GraphicsSettings[GroupType.InDuty]["_SetHudLayout"] = 0;

            GraphicsSettings[GroupType.VR] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.VR]["CharaLight"] = 20;
            GraphicsSettings[GroupType.VR]["Gamma"] = 0;
            GraphicsSettings[GroupType.VR]["ReflectionType_DX11"] = 0;
            GraphicsSettings[GroupType.VR]["ParallaxOcclusion_DX11"] = 0;
            GraphicsSettings[GroupType.VR]["TextureFilterQuality_DX11"] = 2;
            GraphicsSettings[GroupType.VR]["TextureAnisotropicQuality_DX11"] = 0;
            GraphicsSettings[GroupType.VR]["Vignetting_DX11"] = 0;
            GraphicsSettings[GroupType.VR]["SSAO_DX11"] = 0;
            GraphicsSettings[GroupType.VR]["DisplayObjectLimitType"] = 3;
            GraphicsSettings[GroupType.VR]["MouseOpeLimit"] = 1;
            GraphicsSettings[GroupType.VR]["MoveMode"] = 1;
            GraphicsSettings[GroupType.VR]["ObjectBorderingType"] = 1;
            GraphicsSettings[GroupType.VR]["NamePlateDispTypeOther"] = 2;
            GraphicsSettings[GroupType.VR]["_UseChillframes"] = 0;
            GraphicsSettings[GroupType.VR]["_SetHudLayout"] = 0;

            GraphicsSettings[GroupType.VRInCapital] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.VRInCapital]["CharaLight"] = 20;
            GraphicsSettings[GroupType.VRInCapital]["Gamma"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["ReflectionType_DX11"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["ParallaxOcclusion_DX11"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["TextureFilterQuality_DX11"] = 2;
            GraphicsSettings[GroupType.VRInCapital]["TextureAnisotropicQuality_DX11"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["Vignetting_DX11"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["SSAO_DX11"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["DisplayObjectLimitType"] = 2;
            GraphicsSettings[GroupType.VRInCapital]["MouseOpeLimit"] = 1;
            GraphicsSettings[GroupType.VRInCapital]["MoveMode"] = 1;
            GraphicsSettings[GroupType.VRInCapital]["ObjectBorderingType"] = 1;
            GraphicsSettings[GroupType.VRInCapital]["NamePlateDispTypeOther"] = 2;
            GraphicsSettings[GroupType.VRInCapital]["_UseChillframes"] = 0;
            GraphicsSettings[GroupType.VRInCapital]["_SetHudLayout"] = 0;
        }
    }
}
