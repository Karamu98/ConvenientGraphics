using Dalamud.Configuration;
using Dalamud.Plugin;
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
        private IDalamudPluginInterface? iPluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.iPluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.iPluginInterface!.SavePluginConfig(this);
        }

        public void CheckVersion(int UpdateValue)
        {
            if (Version != UpdateValue)
            {
                SaveCurrent();
                Version = UpdateValue;
            }
        }

        public void SaveCurrent()
        {
            Dictionary<GroupType, Dictionary<string, uint>> tmpGraphicsSettings = new Dictionary<GroupType, Dictionary<string, uint>>();
            if (GraphicsSettings.Count == 0)
            {
                ResetDefaults();
            }
            else
            {
                //----
                // Saves the current list of settings
                //----
                foreach(KeyValuePair<GroupType, Dictionary<string, uint>> groups in GraphicsSettings)
                {
                    tmpGraphicsSettings[groups.Key] = new Dictionary<string, uint>();
                    foreach (KeyValuePair<string, uint> item in groups.Value)
                        tmpGraphicsSettings[groups.Key][item.Key] = item.Value;
                }

                //----
                // Resets all defaults so we have a clean list to deal with, removing any
                // unused values while setting new defaults
                //----
                SetDefaults();

                //----
                // Goes though all the defaults and if the key exists
                // sets it from the saved value
                //----
                foreach (KeyValuePair<GroupType, Dictionary<string, uint>> groups in GraphicsSettings)
                    foreach (KeyValuePair<string, uint> item in groups.Value)
                        if (tmpGraphicsSettings[groups.Key].ContainsKey(item.Key))
                            GraphicsSettings[groups.Key][item.Key] = tmpGraphicsSettings[groups.Key][item.Key];
                Save();
            }
        }

        public void ResetDefaults()
        {
            SetDefaults();
            Save();
        }

        private void SetDefaults()
        {
            Dictionary<string, Tuple<uint, uint, uint, uint>> DefaultSettings = new Dictionary<string, Tuple<uint, uint, uint, uint>>();
            DefaultSettings["Fps"]                              = new Tuple<uint, uint, uint, uint>(1, 1, 0, 0);
            DefaultSettings["MouseOpeLimit"]                    = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["Gamma"]                            = new Tuple<uint, uint, uint, uint>(30, 0, 0, 0);
            DefaultSettings["CharaLight"]                       = new Tuple<uint, uint, uint, uint>(30, 20, 20, 20);
            DefaultSettings["DisplayObjectLimitType"]           = new Tuple<uint, uint, uint, uint>(0, 4, 3, 2);
            DefaultSettings["TextureAnisotropicQuality_DX11"]   = new Tuple<uint, uint, uint, uint>(2, 0, 0, 0);
            DefaultSettings["SSAO_DX11"]                        = new Tuple<uint, uint, uint, uint>(5, 0, 0, 0);
            DefaultSettings["Vignetting_DX11"]                  = new Tuple<uint, uint, uint, uint>(0, 0, 0, 0);
            DefaultSettings["GrassQuality_DX11"]                = new Tuple<uint, uint, uint, uint>(3, 2, 0, 0);
            DefaultSettings["ShadowLOD_DX11"]                   = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["ShadowVisibilityTypeOther_DX11"]   = new Tuple<uint, uint, uint, uint>(1, 0, 0, 0);
            DefaultSettings["ShadowVisibilityTypeEnemy_DX11"]   = new Tuple<uint, uint, uint, uint>(1, 1, 1, 0);
            DefaultSettings["PhysicsTypeOther_DX11"]            = new Tuple<uint, uint, uint, uint>(2, 0, 0, 0);
            DefaultSettings["PhysicsTypeEnemy_DX11"]            = new Tuple<uint, uint, uint, uint>(2, 1, 1, 0);
            DefaultSettings["ReflectionType_DX11"]              = new Tuple<uint, uint, uint, uint>(3, 0, 0, 0);
            DefaultSettings["ParallaxOcclusion_DX11"]           = new Tuple<uint, uint, uint, uint>(1, 0, 0, 0);
            DefaultSettings["DynamicRezoThreshold"]             = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["GraphicsRezoScale"]                = new Tuple<uint, uint, uint, uint>(100, 100, 100, 100);
            DefaultSettings["GraphicsRezoUpscaleType"]          = new Tuple<uint, uint, uint, uint>(1, 1, 0, 0);
            DefaultSettings["ShadowBgLOD"]                      = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["DynamicRezoType"]                  = new Tuple<uint, uint, uint, uint>(0, 0, 0, 0);
            DefaultSettings["BattleEffectParty"]                = new Tuple<uint, uint, uint, uint>(1, 1, 1, 1);
            DefaultSettings["BattleEffectOther"]                = new Tuple<uint, uint, uint, uint>(2, 2, 2, 2);
            DefaultSettings["FPSCameraInterpolationType"]       = new Tuple<uint, uint, uint, uint>(2, 2, 2, 2);
            DefaultSettings["EventCameraAutoControl"]           = new Tuple<uint, uint, uint, uint>(0, 0, 0, 0);
            DefaultSettings["NamePlateDispTypeOther"]           = new Tuple<uint, uint, uint, uint>(2, 2, 2, 2);
            DefaultSettings["ObjectBorderingType"]              = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["MoveMode"]                         = new Tuple<uint, uint, uint, uint>(0, 0, 1, 1);
            DefaultSettings["_UseChillframes"]                  = new Tuple<uint, uint, uint, uint>(1, 1, 0, 0);
            DefaultSettings["_SetHudLayout"]                    = new Tuple<uint, uint, uint, uint>(0, 0, 0, 0);

            GraphicsSettings.Clear();
            GraphicsSettings[GroupType.Standard] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.InDuty] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.VR] = new Dictionary<string, uint>();
            GraphicsSettings[GroupType.VRInCapital] = new Dictionary<string, uint>();

            foreach (KeyValuePair<string, Tuple<uint, uint, uint, uint>> configOption in DefaultSettings)
            {
                GraphicsSettings[GroupType.Standard][configOption.Key] = configOption.Value.Item1;
                GraphicsSettings[GroupType.InDuty][configOption.Key] = configOption.Value.Item2;
                GraphicsSettings[GroupType.VR][configOption.Key] = configOption.Value.Item3;
                GraphicsSettings[GroupType.VRInCapital][configOption.Key] = configOption.Value.Item4;
            }
        }
    }
}
