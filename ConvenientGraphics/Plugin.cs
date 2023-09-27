using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;

using System;
using System.Collections.Generic;
using XivCommon;

using ConvenientGraphics.Windows;
using MemoryManager.Structures;


namespace ConvenientGraphics
{
    public unsafe class ConvenientGraphics : IDalamudPlugin
    {
        [PluginService] public static DalamudPluginInterface? PluginInterface { get; private set; }
        [PluginService] public static Framework? Framework { get; private set; }
        [PluginService] public static CommandManager? CommandManager { get; private set; }
        [PluginService] public static ClientState? ClientState { get; private set; }
        [PluginService] public static ChatGui? ChatGui { get; private set; }
        [PluginService] public static Condition? Condition { get; private set; }
        [PluginService] public static GameGui? GameGui { get; private set; }

        public ConvenientGraphics Plugin { get; init; }
        public static SharedMemoryManager smm = new SharedMemoryManager();
        public string Name => "ConvenientGraphics";
        private const string CommandName = "/congraph";

        public Configuration cfg { get; init; }
        public WindowSystem WindowSystem = new("ConvenientGraphics");
        private MainWindow MainWindow { get; init; }
        private XivCommonBase chatHandler = new();

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
        private int UpdateValue = 3;
        private bool isVertMovement = false;
        private int timeOutCount = 0;
        private GroupType prevGroup = GroupType.Standard;
        private GroupType currentGroup = GroupType.Standard;
        bool isInDuty = false;

        public ConvenientGraphics()
        {
            Plugin = this;
            cfg = PluginInterface!.GetPluginConfig() as Configuration ?? new Configuration();
            cfg.Initialize(PluginInterface!);
            cfg.CheckVersion(UpdateValue);

            MainWindow = new MainWindow(this);
            WindowSystem.AddWindow(MainWindow);

            CommandManager!.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Help stuff here"
            });

            ClientState!.Login += OnLogin;
            ClientState!.Logout += OnLogout;
            Framework!.Update += Update;
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
            Framework!.Update -= Update;
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

        private void OnLogin(object? sender, EventArgs e)
        {
            Start();
        }
        private void OnLogout(object? sender, EventArgs e)
        {

        }

        private void Update(Framework framework)
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
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                    isXIVRActive = xivrActive;
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
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                    isInDuty = inDuty;
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
                    SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
                    isXIVRCapital = xivrCapital;
                }

                bool vertMovment = Condition![ConditionFlag.InFlight] || Condition![ConditionFlag.Diving];
                if (isVertMovement != vertMovment)
                {
                    if (vertMovment)
                        ConfigModule.Instance()->SetOption(ConfigOption.MoveMode, 0);
                    else
                        ConfigModule.Instance()->SetOption(ConfigOption.MoveMode, cfg.GraphicsSettings[GroupType.VR][ConfigOption.MoveMode]);
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
        }

        public void Start()
        {
            isEnabled = true;
            smm.SetActive(SharedMemoryPlugins.ConvenientGraphics);
            PrintEcho("Graphics Changing Enabled");
            timeOutCount = 0;
            SetSettings(currentGroup, cfg.GraphicsSettings[currentGroup]);
        }

        public void Stop()
        {
            PrintEcho("Graphics Changing Disabled");
            smm.SetInactive(SharedMemoryPlugins.ConvenientGraphics);
            isEnabled = false;
        }

        private void SetSettings(GroupType currentGroup, Dictionary<ConfigOption, int> settingOptions)
        {
            PrintEcho($"Setting {currentGroup}");
            foreach (KeyValuePair<ConfigOption, int> option in settingOptions)
            {
                if((int)option.Key < (int)ConfigOptionLocal.UseChillframes)
                    ConfigModule.Instance()->SetOption(option.Key, option.Value);
            }

            //----
            // Set FPS
            //----
            if(settingOptions.ContainsKey((ConfigOption)ConfigOptionLocal.UseChillframes))
                if(settingOptions[(ConfigOption)ConfigOptionLocal.UseChillframes] == 0)
                    CommandManager!.ProcessCommand($"/chillframes disable");
                else
                    CommandManager!.ProcessCommand($"/chillframes enable");

            //----
            // Set HUD
            //----
            if (settingOptions.ContainsKey((ConfigOption)ConfigOptionLocal.SetHudLayout))
            {
                //----
                // Create a timer to check for the existance of the player
                // and only update the hud if one exists
                //----
                int hudLayout = settingOptions[(ConfigOption)ConfigOptionLocal.SetHudLayout];
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

        public void RefreshHUDLayoutTick(System.Timers.Timer timer, int hudLayout)
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
