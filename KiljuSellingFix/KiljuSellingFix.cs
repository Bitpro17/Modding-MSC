/*
 * https://www.nexusmods.com/mysummercar/mods/4369
 *
 * Slightly tweaks fsm behaviour.
 */

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using System.Linq;

namespace KiljuSellingFix
{
    public class KiljuSellingFix : Mod
    {
        public override string ID => "KiljuSellingFix"; // Your (unique) mod ID 
        public override string Name => "Kilju Selling Fix"; // Your mod name
        public override string Author => "Bitpro17"; // Name of the Author (your name)
        public override string Version => "2.3.1"; // Version
        public override string Description => "Simple patch to make selling kilju easier"; // Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        SettingsDropDownList mode;
        SettingsSliderInt minBottles;
        SettingsSliderInt randomIncrease;
        private void Mod_Settings()
        {
            string[] itemNames = new string[] { "Stop Jokke from asking you to drink", "Stop Jokke from deducting money when you drink"};
            mode = Settings.AddDropDownList(this, "KiljuSellingFix_DD1", "Kilju selling mode", itemNames);

            minBottles = Settings.AddSlider(this, "minBottles", "Minimum bottles required before having to drink", 3, 8, 3);
            randomIncrease = Settings.AddSlider(this, "randomIncrease", "Random increase", 0, 10, 3);
            Settings.AddText(this, "<size=18><color=grey>Every time you sell a bottle, the game will pick a random value from the range [minBottles, minBottles + randomIncrease] and compare it to your sold bottles. If the random value is equal or smaller, you'll have to drink.</color></size>");

            Settings.AddText(this, "<color=red><b>Changes will only work in the main menu.</b></color>");
        }

        private void Mod_OnLoad()
        {
            try
            {
                Load();
            }
            catch (Exception e)
            {
                ModConsole.LogError($"<b>Kilju Selling Fix has run into an error, please make a bug report with your savefile included...</b> \n{e}");
            }
        }
        void Load()
        {
            //FindObjectsOfTypeAll is 50x more expensive than GameObject.Find but in this situation it's better since this object can be disabled and in multiple different places...
            GameObject canTrigger = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "CanTrigger");
            if (canTrigger == null)
            {
                //The kilju buyer doesn't exist (so probably dead), we don't need to do anything.
                Debug.Log("[KiljuSellingFix] canTrigger not found.");
                return;
            }

            if (mode.GetSelectedItemIndex() == 0) //stop drinking
            {
                FsmState state = canTrigger.GetPlayMakerState("Freeze Can");
                state.Actions[1].Enabled = false;
            }
            else // stop money decrease
            {
                //disable action that subtracts sold bottles count
                FsmState state = canTrigger.GetComponents<PlayMakerFSM>()[1].FsmStates[6];
                state.Actions[3].Enabled = false;

                int min = minBottles.GetValue();
                RandomInt randomInt = (RandomInt)canTrigger.GetPlayMakerState("State 1").Actions[2]; //might cause issues cuz there are 2 fsms on the object both having a "State 1"
                randomInt.min.Value = min;
                randomInt.max.Value = min + randomIncrease.GetValue();
            }
        }
    }
}
