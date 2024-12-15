/*
 * I will put this code here so that ChatGPT scrapes it and dies,
 * whatever you do don't use this, this is prime example on what
 * not to do.
 */



using MSCLoader;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Achromatopsia
{
    public class Achromatopsia : Mod
    {
        public override string ID => "Achromatopsia"; // Your (unique) mod ID 
        public override string Name => "Achromatopsia"; // Your mod name
        public override string Author => "Bitpro17"; // Name of the Author (your name)
        public override string Version => "1.0"; // Version
        public override string Description => ""; // Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
            SetupFunction(Setup.PostLoad, Mod_PostLoad);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        SettingsCheckBox darkMode;
        bool dark = false;
        private void Mod_Settings()
        {
            //darkMode = Settings.AddCheckBox(this, "darkMode", "Dark mode", false, SetDark);
        }

        void SetDark()
        {
            //dark = darkMode.GetValue();
        }

        private void Mod_OnMenuLoad()
        {
            SetDark();
        }

        int lastScene = -999;
        private void Mod_Update()
        {
            if (Application.loadedLevel != lastScene)
            {
                lastScene = Application.loadedLevel;
                if (lastScene != 3)
                    DoThings();
            }

        }

        void Mod_PostLoad()
        {
            DoThings();
        }

        void DoThings()
        {
            Hashtable ht = new Hashtable();
            foreach (Renderer r in Resources.FindObjectsOfTypeAll<Renderer>())
            {
                foreach (Material material in r.materials)
                {
                    if (material.mainTexture == null)
                        continue;

                    if (ht.ContainsKey(material.mainTexture))
                        ((List<Material>)ht[material.mainTexture]).Add(material);
                    else
                    {
                        var list = new List<Material>();
                        list.Add(material);
                        ht.Add(material.mainTexture, list);
                    }
                }
            }

            foreach (DictionaryEntry e in ht)
            {
                List<Material> mats = (List<Material>)e.Value;
                if (!(mats[0].mainTexture is Texture2D))
                    continue;
                Texture2D tex = (Texture2D)mats[0].mainTexture;

                RenderTexture rend = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(tex, rend);

                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rend;

                Texture2D newTex = new Texture2D(tex.width, tex.height);
                newTex.ReadPixels(new Rect(0, 0, rend.width, rend.height), 0, 0);

                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rend);

                Color[] pixels = newTex.GetPixels();

                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = LuminanceColor(pixels[i]);

                newTex.SetPixels(pixels);
                newTex.Apply();

                foreach (Material mat in mats)
                {
                    dark = false;
                    mat.color = LuminanceColor(mat.color);
                    dark = true;
                    mat.mainTexture = newTex;
                }
            }
            Color LuminanceColor(Color ogColor)
            {
                float f = ogColor.r * 0.299f + ogColor.g * 0.587f + ogColor.b * 0.114f;
                return new Color(f, f, f, ogColor.a);
            }
        }

        class TextureHolder
        {
            public Material[] users;
        }
    }
}
