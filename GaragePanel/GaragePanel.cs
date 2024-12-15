/*
 * https://www.nexusmods.com/mysummercar/mods/5294
 *
 * "Adds a panel into the garage, allows mod users to put any image into the mods folder to replace the texture."
 */

using MSCLoader;
using UnityEngine;

namespace GaragePanel
{
    public class GaragePanel : Mod
    {
        public override string ID => "GaragePanel"; // Your (unique) mod ID 
        public override string Name => "GaragePanel"; // Your mod name
        public override string Author => "Bitpro17"; // Name of the Author (your name)
        public override string Version => "1.0"; // Version
        public override string Description => ""; // Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        //Default unity materials have these properties, this mod allows you to change them
        SettingsSlider metallic;
        SettingsSlider smoothness;

        private void Mod_Settings()
        {
            metallic = Settings.AddSlider(this, "metallic", "Metallic", 0f, 1f, 0f, ChangeMetallic);
            smoothness = Settings.AddSlider(this, "smoothness", "Smoothness", 0f, 1f, 0f, ChangeSmoothness);
            Settings.AddButton(this, "Apply new texture", ApplyTexture);
        }

        void ChangeMetallic()
        {
            mat.SetFloat("_Metallic", metallic.GetValue());
        }

        void ChangeSmoothness()
        {
            mat.SetFloat("_Glossiness", smoothness.GetValue());
        }

        void ApplyTexture()
        {
            Texture2D tex = LoadAssets.LoadTexture(this, "panel.png");
            
            mat.mainTexture = tex;
            float width = (float)tex.width / tex.height;
            plane.localScale = new Vector3(width * 0.1f, 0.1f, 0.1f);
        }

        Material mat;
        Transform plane;
        private void Mod_OnLoad()
        {
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
            plane.gameObject.name = "garagePanel";

            plane.localPosition = new Vector3(-14.2f, 1.1f, 0f);
            plane.localEulerAngles = new Vector3(90f, 270f, 0f);

            mat = plane.GetComponent<MeshRenderer>().material;

            ApplyTexture();
            ChangeMetallic();
            ChangeSmoothness();
        }
    }
}
