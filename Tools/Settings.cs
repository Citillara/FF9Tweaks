using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace FF9Tweaks.Tools
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Lose gil on hit (includes overkill)")]
        public bool EnableGilLoss = false;

        [Draw("Disable XP gain")]
        public bool EnableNoXP = false;

        [Draw("Enable battle log")]
        public bool EnableBattleLog = false;


        private void DrawTab1()
        {
            GUILayout.Label("General tweaks");

            EnableGilLoss = GUILayout.Toggle(EnableGilLoss, "Lose gil on hit (includes overkill)");
            EnableNoXP = GUILayout.Toggle(EnableNoXP, "Disable XP gain");
            EnableBattleLog = GUILayout.Toggle(EnableBattleLog, "[Debug] Enable battle log");

        }



        public StatsPenaltySettings StatsPenaltySettings = new StatsPenaltySettings();
        public HPLossSettings HPLossSettings = new HPLossSettings();
        public SoloModeSettings SoloModeSettings = new SoloModeSettings();
        public PartyEditorSettings PartyEditorSettings = new PartyEditorSettings();


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {

        }

        private int selectedTab = 0;
        private string[] tabTitles = { "General tweaks", "Party Editor", "Solo Mode", "Stats Penalty", "FUCK HP™" };


        public void OnGui(UnityModManager.ModEntry modEntry)
        {
            // Tab selection
            selectedTab = GUILayout.Toolbar(selectedTab, tabTitles);

            switch (selectedTab)
            {
                case 0:
                    DrawTab1();
                    break;
                case 1:
                    PartyEditorSettings.DrawTab();
                    break;
                case 2:
                    SoloModeSettings.DrawTab();
                    break;
                case 3:
                    StatsPenaltySettings.DrawTab();
                    break;
                case 4:
                    HPLossSettings.DrawTab();
                    break;
            }

            if (GUI.changed)
            {
                Save(modEntry);
            }
        }

    }

    public class PartyEditorSettings
    {

        public int[] SelectedOption = new int[4] { 0, 0, 0, 0 };
        // Array to manage the state of checkboxes
        public bool[] Locked = new bool[4] { false, false, false, false };

        public void DrawTab()
        {
            GUILayout.Label("Party editor");

            // Draw the GUI in a new tab
            if (GUILayout.Button("Refresh"))
            {
                int i = 0;
                foreach (PLAYER p in FF9StateSystem.Common.FF9.party.member)
                {
                    // Check if the party member is not null
                    if (p != null)
                    {
                        SelectedOption[i] = p.info.slot_no;
                    }
                    else
                    {
                        // Handle null (empty slot)
                        SelectedOption[i] = 9; // 9 can indicate an empty slot
                    }
                    i++;
                }
            }

            GUILayout.Space(10); // Add some spacing for better UI

            // Iterate through each line
            for (int i = 0; i < 4; i++)
            {
                GUILayout.BeginHorizontal();

                // Create a group of radio buttons
                SelectedOption[i] = GUILayout.SelectionGrid(SelectedOption[i], new string[]
                {
                "Zidane",
                "Vivi",
                "Garnet",
                "Steiner",
                "Freija",
                "Kuina",
                "Eiko",
                "Salamander",
                "Beatrix",
                "None"
                }, 10);

                // Checkbox for locking the character selection
                Locked[i] = GUILayout.Toggle(Locked[i], "Lock");

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10); // Add some spacing

            if (GUILayout.Button("Apply"))
            {
                for (int i = 0; i < SelectedOption.Length; i++)
                {
                    bool result = FF9Helpers.ForceAddCharacterToParty(SelectedOption[i], i);
                    if (!result)
                    {
                        // Handle error if the character could not be added
                        Debug.Log($"Failed to add character {SelectedOption[i]} to slot {i}");
                    }
                }
            }
        }

    }

    public class SoloModeSettings
    {
        public bool Enabled = false;

        public bool Zidane = false;
        public bool Vivi = false;
        public bool Dagger = false;
        public bool Steiner = false;
        public bool Freya = false;
        public bool Quina = false;
        public bool Eiko = false;
        public bool Amarant = false;
        public bool Beatrix = false;

        public void DrawTab()
        {
            GUILayout.Label("Settings for Solo Mode");

            Enabled = GUILayout.Toggle(Enabled, "Enable Solo Mode");
            Zidane = GUILayout.Toggle(Zidane, "Zidane");
            Vivi = GUILayout.Toggle(Vivi, "Vivi");
            Dagger = GUILayout.Toggle(Dagger, "Dagger");
            Steiner = GUILayout.Toggle(Steiner, "Steiner");
            Freya = GUILayout.Toggle(Freya, "Freya");
            Quina = GUILayout.Toggle(Quina, "Quina");
            Eiko = GUILayout.Toggle(Eiko, "Eiko");
            Amarant = GUILayout.Toggle(Amarant, "Amarant");
            Beatrix = GUILayout.Toggle(Beatrix, "Beatrix");
        }
    }

    public class HPLossSettings
    {
        public bool Enabled = false;

        public bool OutputToFile = false;

        public HPLossFormula LossFormula = HPLossFormula.APDividedBy10;

        public enum HPLossFormula { APDividedBy10, APPercentage, APDividedBy10WithLevel }

        [Draw("File path", DrawType.Ignore)]
        public string FilePath = null;

        [Draw("Percentage Max HP loss per AP ( 10 = 1% )", Min = 0, Max = 50, Type = DrawType.Slider)]
        public int HPAPLossPercent = 0;


        public void DrawTab()
        {
            GUILayout.Label("Settings for FUCK HP™");

            Enabled = GUILayout.Toggle(Enabled, "Enable FUCK HP™");

            GUILayout.Label("HP loss formula");
            LossFormula = (HPLossFormula)GUILayout.SelectionGrid((int)LossFormula, Enum.GetNames(typeof(HPLossFormula)), 3);

            GUILayout.Label("Percentage Max HP loss per AP");
            HPAPLossPercent = (int)GUILayout.HorizontalSlider(HPAPLossPercent, 0, 50);
            GUILayout.Label(((float)HPAPLossPercent / 10).ToString() + "%");


            OutputToFile = GUILayout.Toggle(OutputToFile, "Output to a file");

            GUILayout.Label("File path");
            FilePath = GUILayout.TextField(FilePath);
        }
    }

    public class StatsPenaltySettings : UnityModManager.ModSettings, IDrawable
    {

        [Draw("Enable stats penalty system")]
        public bool EnableStatPenalty = false;

        [Draw("Enable stats penalty based on external file")]
        public bool EnableStatPenaltyBaseOnFile = false;

        public enum FileType { INI }

        [Draw("File type")]
        public FileType StatPenaltyBaseFileType = FileType.INI;

        [Draw("File path", DrawType.Ignore)]
        public string StatPenaltyBaseFilePath = @"E:\Lioranboard 2\viewers.ini";

        [Draw("Enable raw stats penalty")]
        public bool EnableRawStatsPenalty = false;

        [Draw("Raw stats penalty", Min = 0, Max = 100, Type = DrawType.Slider)]
        public int RawStatsPenalty = 0;


        public void DrawTab()
        {
            GUILayout.Label("Settings Stats Penalty");

            // Checkbox for EnableStatPenalty
            EnableStatPenalty = GUILayout.Toggle(EnableStatPenalty, "Enable stats penalty system");

            // Checkbox for EnableStatPenaltyBaseOnFile
            EnableStatPenaltyBaseOnFile = GUILayout.Toggle(EnableStatPenaltyBaseOnFile, "Enable stats penalty based on external file");

            // Dropdown for StatPenaltyBaseFileType
            GUILayout.Label("File type");
            StatPenaltyBaseFileType = (FileType)GUILayout.SelectionGrid((int)StatPenaltyBaseFileType, Enum.GetNames(typeof(FileType)), 1);

            // TextField for StatPenaltyBaseFilePath
            GUILayout.Label("File path");
            StatPenaltyBaseFilePath = GUILayout.TextField(StatPenaltyBaseFilePath);

            // Checkbox for EnableRawStatsPenalty
            EnableRawStatsPenalty = GUILayout.Toggle(EnableRawStatsPenalty, "Enable raw stats penalty");

            // Slider for RawStatsPenalty
            GUILayout.Label("Raw stats penalty");
            RawStatsPenalty = (int)GUILayout.HorizontalSlider(RawStatsPenalty, 0, 100);
            GUILayout.Label(RawStatsPenalty.ToString());
        }


        public void OnChange()
        {
        }
    }
}
