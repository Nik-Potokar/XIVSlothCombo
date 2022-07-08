using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace XIVSlothCombo.Services
{
    internal unsafe static class BlueMageService
    {
        public static void PopulateBLUSpells()
        {
            IntPtr notebookPtr = Service.GameGui.GetAddonByName("AOZNotebook", 1);
            if (notebookPtr == IntPtr.Zero)
                return;

            List<uint> prevList = Service.Configuration.ActiveBLUSpells.ToList();

            Service.Configuration.ActiveBLUSpells.Clear();
            AddonAOZNotebook notebook = Marshal.PtrToStructure<AddonAOZNotebook>(notebookPtr);

            if (notebook.ActiveActions01.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions01.ActionID);
            if (notebook.ActiveActions02.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions02.ActionID);
            if (notebook.ActiveActions03.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions03.ActionID);
            if (notebook.ActiveActions04.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions04.ActionID);
            if (notebook.ActiveActions05.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions05.ActionID);
            if (notebook.ActiveActions06.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions06.ActionID);

            if (notebook.ActiveActions07.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions07.ActionID);
            if (notebook.ActiveActions08.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions08.ActionID);
            if (notebook.ActiveActions09.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions09.ActionID);
            if (notebook.ActiveActions10.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions10.ActionID);
            if (notebook.ActiveActions11.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions11.ActionID);
            if (notebook.ActiveActions12.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions12.ActionID);

            if (notebook.ActiveActions13.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions13.ActionID);
            if (notebook.ActiveActions14.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions14.ActionID);
            if (notebook.ActiveActions15.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions15.ActionID);
            if (notebook.ActiveActions16.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions16.ActionID);
            if (notebook.ActiveActions17.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions17.ActionID);
            if (notebook.ActiveActions18.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions18.ActionID);

            if (notebook.ActiveActions19.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions19.ActionID);
            if (notebook.ActiveActions20.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions20.ActionID);
            if (notebook.ActiveActions21.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions21.ActionID);
            if (notebook.ActiveActions22.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions22.ActionID);
            if (notebook.ActiveActions23.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions23.ActionID);
            if (notebook.ActiveActions24.ActionID != 0) Service.Configuration.ActiveBLUSpells.Add((uint)notebook.ActiveActions24.ActionID);

            if (Service.Configuration.ActiveBLUSpells.Except(prevList).Any())
                Service.Configuration.Save();
        }
    }
}
