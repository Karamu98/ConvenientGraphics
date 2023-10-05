using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.FFXIV.Common.Configuration;

using System;
using System.Collections.Generic;
using XivCommon;

using ConvenientGraphics.Windows;
using MemoryManager.Structures;

namespace ConvenientGraphics
{
    public unsafe class Plugin : IDalamudPlugin
    {
        [PluginService] public static DalamudPluginInterface? PluginInterface { get; private set; } = null;
        [PluginService] public static IFramework? iFramework { get; private set; } = null;
        [PluginService] public static ICommandManager? CommandManager { get; private set; } = null;
        [PluginService] public static IClientState? ClientState { get; private set; } = null;
        [PluginService] public static IChatGui? ChatGui { get; private set; } = null;
        [PluginService] public static ICondition? Condition { get; private set; } = null;
        [PluginService] public static IGameGui? GameGui { get; private set; } = null;
        [PluginService] public static IPluginLog? Log { get; private set; } = null;

        public static SharedMemoryManager smm = new SharedMemoryManager();
        public string Name => "ConvenientGraphics";
        private const string CommandName = "/congraph";

        public Configuration cfg { get; init; }
        public WindowSystem WindowSystem = new("ConvenientGraphics");
        private MainWindow MainWindow { get; init; }
        private XivCommonBase chatHandler { get; set; }
        private Framework* frameworkInstance = Framework.Instance();

        ConfigBase*[] cfgBase = new ConfigBase*[4];
        private Dictionary<string, List<Tuple<uint, uint>>> MappedSettings = new Dictionary<string, List<Tuple<uint, uint>>>();

        private List<ushort> cityZones = new List<ushort>() { 
            128, 129, // limsa
            130, 131, // uldah
            132, 133, // gridania
            418, 419, // ishgard
            478, // idleshire
            628, // kugane
            635, // ralgarsreach
            819, // crystalium
            820, // eulmore
            963, // radzahan
            962, // sharlayan
        };



        
        private bool isEnabled = false;
        private bool isXIVRActive = false;
        private bool isXIVRCapital = false;
        private int cfgVersionValue = 5;
        private bool isVertMovement = false;
        private int timeOutCount = 0;
        private GroupType prevGroup = GroupType.Standard;
        private GroupType currentGroup = GroupType.Standard;
        bool isInDuty = false;

        public Plugin()
        {
            cfg = PluginInterface!.GetPluginConfig() as Configuration ?? new Configuration();
            cfg.Initialize(PluginInterface!);
            cfg.CheckVersion(cfgVersionValue);

            chatHandler = new XivCommonBase(PluginInterface);
            MainWindow = new MainWindow(this);
            WindowSystem.AddWindow(MainWindow);

            CommandManager!.RemoveHandler(CommandName);
            CommandManager!.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Help stuff here"
            });

            ClientState!.Login += OnLogin;
            ClientState!.Logout += OnLogout;
            iFramework!.Update += Update;
            PluginInterface!.UiBuilder.Draw += DrawUI;
            PluginInterface!.UiBuilder.OpenConfigUi += ToggleUI;

            Initialize();
            Start();
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            MainWindow.Dispose();

            Stop();
            smm.SetClose(SharedMemoryPlugins.ConvenientGraphics);
            smm.Dispose();

            ClientState!.Login -= OnLogin;
            ClientState!.Logout -= OnLogout;
            iFramework!.Update -= Update;
            PluginInterface!.UiBuilder.Draw -= DrawUI;
            PluginInterface!.UiBuilder.OpenConfigUi -= ToggleUI;

            CommandManager!.RemoveHandler(CommandName);
        }

        public void ToggleUI() => MainWindow.IsOpen ^= true;
        private void OnCommand(string command, string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                ToggleUI();
                return;
            }
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        private void OnLogin()
        {
            Start();
        }
        private void OnLogout()
        {

        }

        private void Update(IFramework framework)
        {
            //----
            // If vr is active, load vr settings
            // otherwise check and see if were in a duty and set that
            //----
            if (smm.CheckOpen(SharedMemoryPlugins.XIVR))
            {
                bool xivrActive = smm.CheckActive(SharedMemoryPlugins.XIVR);
                if (isXIVRActive != xivrActive)
                {
                    if (xivrActive)
                    {
                        prevGroup = currentGroup;
                        bool xivrCapital = cityZones.Contains(ClientState!.TerritoryType);
                        if (xivrCapital)
                            currentGroup = GroupType.VRInCapital;
                        else
                            currentGroup = GroupType.VR;
                        isXIVRCapital = xivrCapital;
                    }
                    else
                    {
                        currentGroup = prevGroup;
                        prevGroup = GroupType.Standard;
                        isXIVRCapital = false;
                    }
                    isXIVRActive = xivrActive;
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                }
            }
            if (!isXIVRActive)
            {
                isXIVRCapital = false;
                bool inDuty = Condition![ConditionFlag.BoundByDuty] || Condition![ConditionFlag.BoundByDuty95] || Condition![ConditionFlag.InDeepDungeon];
                if (isInDuty != inDuty)
                {
                    if (inDuty)
                        currentGroup = GroupType.InDuty;
                    else
                        currentGroup = GroupType.Standard;
                    isInDuty = inDuty;
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                }
            }
            else
            {
                bool xivrCapital = cityZones.Contains(ClientState!.TerritoryType);
                if (isXIVRCapital != xivrCapital)
                {
                    if(xivrCapital)
                        currentGroup = GroupType.VRInCapital;
                    else
                        currentGroup = GroupType.VR;
                    isXIVRCapital = xivrCapital;
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                }

                bool vertMovment = Condition![ConditionFlag.InFlight] || Condition![ConditionFlag.Diving];
                if (isVertMovement != vertMovment)
                {
                    if (vertMovment)
                        SetSettingsValue(MappedSettings["MoveMode"], 0);
                    else
                        SetSettingsValue(MappedSettings["MoveMode"], cfg.GraphicsSettings[GroupType.VR]["MoveMode"]);
                    isVertMovement = vertMovment;
                }
            }
        }

        public static void PrintEcho(string message) => ChatGui!.Print($"[ConvenientGraphics] {message}");
        public static void PrintError(string message) => ChatGui!.PrintError($"[ConvenientGraphics] {message}");

        private void Initialize()
        {
            smm.SetOpen(SharedMemoryPlugins.ConvenientGraphics);
            currentGroup = GroupType.Standard;

            cfgBase[0] = &(frameworkInstance->SystemConfig.CommonSystemConfig.ConfigBase);
            cfgBase[1] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiConfig);
            cfgBase[2] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiControlConfig);
            cfgBase[3] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiControlGamepadConfig);

            List<string> cfgSearchStrings = new List<string>() {
                "CharaLight",
                "Gamma",
                "ReflectionType_DX11",
                "ParallaxOcclusion_DX11",
                "TextureFilterQuality_DX11",
                "TextureAnisotropicQuality_DX11",
                "Vignetting_DX11",
                "SSAO_DX11",
                "DisplayObjectLimitType",
                "MouseOpeLimit",
                "MoveMode",
                "ObjectBorderingType",
                "NamePlateDispTypeOther"
                };

            MappedSettings.Clear();
            for (uint cfgId = 0; cfgId < cfgBase.Length; cfgId++)
            {
                for (uint i = 0; i < cfgBase[cfgId]->ConfigCount; i++)
                {
                    if (cfgBase[cfgId]->ConfigEntry[i].Type == 0)
                        continue;

                    string name = MemoryHelper.ReadStringNullTerminated(new IntPtr(cfgBase[cfgId]->ConfigEntry[i].Name));
                    if(cfgSearchStrings.Contains(name))
                    {
                        if (!MappedSettings.ContainsKey(name))
                            MappedSettings[name] = new List<Tuple<uint, uint>>();
                        MappedSettings[name].Add(new Tuple<uint, uint>(cfgId, cfgBase[cfgId]->ConfigEntry[i].Index));
                    }
                }
            }
        }

        public void Start()
        {
            isEnabled = true;
            smm.SetActive(SharedMemoryPlugins.ConvenientGraphics);
            //PrintEcho("Graphics Changing Enabled");
            timeOutCount = 0;
            SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
        }

        public void Stop()
        {
            //PrintEcho("Graphics Changing Disabled");
            smm.SetInactive(SharedMemoryPlugins.ConvenientGraphics);
            isEnabled = false;
        }

        public void Reset()
        {
            timeOutCount = 0;
            SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
        }


        private void SetSettingsValue(List<Tuple<uint, uint>> list, uint value)
        {
            foreach (Tuple<uint, uint> item in list)
                cfgBase[item.Item1]->ConfigEntry[item.Item2].SetValueUInt(value);
        }

        private uint GetSettingsValue(List<Tuple<uint, uint>> list, int index)
        {
            if (index >= list.Count)
                return 0;
            return cfgBase[list[index].Item1]->ConfigEntry[list[index].Item2].Value.UInt;
        }

        private void SetSettings(GroupType currentGroup, Dictionary<string, uint> settingOptions)
        {
            PrintEcho($"Setting {currentGroup}");
            foreach (KeyValuePair<string, uint> option in settingOptions)
            {
                if(!option.Key.StartsWith("_"))
                    if(MappedSettings.ContainsKey(option.Key))
                        SetSettingsValue(MappedSettings[option.Key], option.Value);
            }

            //----
            // Set FPS
            //----
            if(settingOptions.ContainsKey("_UseChillframes"))
                if(settingOptions["_UseChillframes"] == 0)
                    CommandManager!.ProcessCommand($"/chillframes disable");
                else
                    CommandManager!.ProcessCommand($"/chillframes enable");

            //----
            // Set HUD
            //----
            if (settingOptions.ContainsKey("_SetHudLayout"))
            {
                //----
                // Create a timer to check for the existance of the player
                // and only update the hud if one exists
                //----
                uint hudLayout = settingOptions["_SetHudLayout"];
                if (hudLayout > 0)
                {
                    System.Timers.Timer hudChangeTimer = new System.Timers.Timer();
                    hudChangeTimer.Interval = 500;
                    hudChangeTimer.Elapsed += (sender, e) => { RefreshHUDLayoutTick(hudChangeTimer, hudLayout); };
                    hudChangeTimer.Enabled = true;
                    timeOutCount = 0;
                }
            }
        }

        public void RefreshHUDLayoutTick(System.Timers.Timer timer, uint hudLayout)
        {
            AtkUnitBase* hpBar = (AtkUnitBase*)GameGui!.GetAddonByName("_ParameterWidget", 1);
            AtkUnitBase* minimap = (AtkUnitBase*)GameGui!.GetAddonByName("_NaviMap", 1);
           
            timeOutCount++;
            if(hpBar->IsVisible || minimap->IsVisible)
            {
                hudLayout = Math.Max(Math.Min(hudLayout, 4), 1);
                string cleanCmd = chatHandler.Functions.Chat.SanitiseText($"/hudlayout {hudLayout}");
                chatHandler.Functions.Chat.SendMessage(cleanCmd);
                timer.Enabled = false;
            }
            else if (timeOutCount > 20)
                timer.Enabled = false;
        }
    }
}
