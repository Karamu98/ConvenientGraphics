
using ConvenientGraphics;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Configuration;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

namespace SettingsManager
{
    public unsafe class ConfigManager
    {
        private Framework* frameworkInstance = Framework.Instance();

        private ConfigBase*[] cfgBase = new ConfigBase*[4];
        private Dictionary<string, List<Tuple<uint, uint>>> MappedSettings = new Dictionary<string, List<Tuple<uint, uint>>>();
        private List<string> cfgSearchStrings = new List<string>();

        private Dictionary<string, uint> savedSettings = new Dictionary<string, uint>();

        public ConfigManager()
        {
            cfgBase[0] = &(frameworkInstance->SystemConfig.CommonSystemConfig.ConfigBase);
            cfgBase[1] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiConfig);
            cfgBase[2] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiControlConfig);
            cfgBase[3] = &(frameworkInstance->SystemConfig.CommonSystemConfig.UiControlGamepadConfig);

            ClearList();
        }

        public void Dispose()
        {
            ClearList();
        }

        public void ClearList()
        {
            cfgSearchStrings.Clear();
            MappedSettings.Clear();
            savedSettings.Clear();
        }

        public void AddToList(string name)
        {
            if(!cfgSearchStrings.Contains(name))
                cfgSearchStrings.Add(name);
        }

        public void AddToList(List<string> list)
        {
            foreach(string name in list)
                AddToList(name);
        }

        public void MapSettings()
        {
            MappedSettings.Clear();
            for (uint cfgId = 0; cfgId < cfgBase.Length; cfgId++)
            {
                for (uint i = 0; i < cfgBase[cfgId]->ConfigCount; i++)
                {
                    if (cfgBase[cfgId]->ConfigEntry[i].Type == 0)
                        continue;

                    string name = MemoryHelper.ReadStringNullTerminated(new IntPtr(cfgBase[cfgId]->ConfigEntry[i].Name));
                    if (cfgSearchStrings.Contains(name))
                    {
                        if (!MappedSettings.ContainsKey(name))
                            MappedSettings[name] = new List<Tuple<uint, uint>>();
                        MappedSettings[name].Add(new Tuple<uint, uint>(cfgId, cfgBase[cfgId]->ConfigEntry[i].Index));
                    }
                }
            }
        }

        public void SetSettingsValue(string setting, uint value)
        {
            List<Tuple<uint, uint>> list = MappedSettings.GetValueOrDefault<string, List<Tuple<uint, uint>>>(setting, new List<Tuple<uint, uint>>());
            foreach (Tuple<uint, uint> item in list)
                cfgBase[item.Item1]->ConfigEntry[item.Item2].SetValueUInt(value);
        }

        public uint GetSettingsValue(string setting, int index)
        {
            List<Tuple<uint, uint>> list = MappedSettings.GetValueOrDefault<string, List<Tuple<uint, uint>>>(setting, new List<Tuple<uint, uint>>());
            if (index >= list.Count)
                return 0;
            return cfgBase[list[index].Item1]->ConfigEntry[list[index].Item2].Value.UInt;
        }


        public void DebugSettings(bool printAll = false)
        {
            for (uint cfgId = 0; cfgId < cfgBase.Length; cfgId++)
            {
                for (uint i = 0; i < cfgBase[cfgId]->ConfigCount; i++)
                {
                    if (cfgBase[cfgId]->ConfigEntry[i].Type == 0)
                        continue;

                    string name = MemoryHelper.ReadStringNullTerminated(new IntPtr(cfgBase[cfgId]->ConfigEntry[i].Name));
                    if (cfgSearchStrings.Contains(name))
                        Plugin.Log!.Info($"Location: * {i} cfgGroup: {cfgId} cfgOffset: {cfgBase[cfgId]->ConfigEntry[i].Index} name: {name} value: {cfgBase[cfgId]->ConfigEntry[i].Value.UInt}");
                    else if(printAll)
                        Plugin.Log!.Info($"Location:   {i} cfgGroup: {cfgId} cfgOffset: {cfgBase[cfgId]->ConfigEntry[i].Index} name: {name} value: {cfgBase[cfgId]->ConfigEntry[i].Value.UInt}");
                }
            }
        }

        public void Save(bool compare = false)
        {
            if (!compare)
            {
                for (uint cfgId = 0; cfgId < cfgBase.Length; cfgId++)
                {
                    for (uint i = 0; i < cfgBase[cfgId]->ConfigCount; i++)
                    {
                        if (cfgBase[cfgId]->ConfigEntry[i].Type == 0)
                            continue;

                        string name = MemoryHelper.ReadStringNullTerminated(new IntPtr(cfgBase[cfgId]->ConfigEntry[i].Name));
                        if (!savedSettings.ContainsKey(name))
                            savedSettings[name] = cfgBase[cfgId]->ConfigEntry[i].Value.UInt;
                    }
                }
                Plugin.Log!.Info($"Settings values saved");
            }
            else
            {
                for (uint cfgId = 0; cfgId < cfgBase.Length; cfgId++)
                {
                    for (uint i = 0; i < cfgBase[cfgId]->ConfigCount; i++)
                    {
                        if (cfgBase[cfgId]->ConfigEntry[i].Type == 0)
                            continue;

                        string name = MemoryHelper.ReadStringNullTerminated(new IntPtr(cfgBase[cfgId]->ConfigEntry[i].Name));
                        if(savedSettings.ContainsKey(name))
                            if (savedSettings[name] != cfgBase[cfgId]->ConfigEntry[i].Value.UInt)
                                Plugin.Log!.Info($"{name} | {savedSettings[name]} => {cfgBase[cfgId]->ConfigEntry[i].Value.UInt}");
                    }
                }
            }
        }
    }
}
