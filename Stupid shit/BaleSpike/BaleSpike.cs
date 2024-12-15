/*
 * https://www.nexusmods.com/mysummercar/mods/6408
 *
 * Tries to make the tractor forks penetrate the haybales,
 * this mod is buggy af (since it messes with physics) and
 * i never bothered to fix it.
 */

using MSCLoader;
using UnityEngine;

namespace BaleSpike
{
    public class BaleSpike : Mod
    {
        public override string ID => "BaleSpike"; // Your (unique) mod ID 
        public override string Name => "Bale Spike"; // Your mod name
        public override string Author => "Bitpro17"; // Name of the Author (your name)
        public override string Version => "1.0"; // Version
        public override string Description => ""; // Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
        }

        private void Mod_OnLoad()
        {
            new GameObject("TipTrigger").AddComponent<HayBaleLogic>();
        }
    }
}
