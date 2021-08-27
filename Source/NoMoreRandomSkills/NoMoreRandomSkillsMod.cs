using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace NoMoreRandomSkills
{
    [StaticConstructorOnStartup]
    internal class NoMoreRandomSkillsMod : Mod
    {
        /// <summary>
        ///     The instance of the settings to be read by the mod
        /// </summary>
        public static NoMoreRandomSkillsMod instance;

        /// <summary>
        ///     The private settings
        /// </summary>
        private NoMoreRandomSkillsSettings settings;

        /// <summary>
        ///     Cunstructor
        /// </summary>
        /// <param name="content"></param>
        public NoMoreRandomSkillsMod(ModContentPack content)
            : base(content)
        {
            instance = this;

            var harmonyInstance = new Harmony("com.nomorerandomskills.patch");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     The instance-settings for the mod
        /// </summary>
        internal NoMoreRandomSkillsSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<NoMoreRandomSkillsSettings>();
                }

                return settings;
            }

            set => settings = value;
        }

        /// <summary>
        ///     The settings-window
        ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
        /// </summary>
        /// <param name="rect"></param>
        public override void DoSettingsWindowContents(Rect rect)
        {
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            listing_Standard.Gap();
            listing_Standard.CheckboxLabeled("Ignore vanilla backstories", ref Settings.OnlyCustomBackstories,
                "Will only select backstories from the custom added by this mod instead of vanilla.");
            listing_Standard.Label($"Minimum age: {Settings.MinimumSpawnAge}", -1,
                "The minimum age of any spawned pawn");
            listing_Standard.IntAdjuster(ref Settings.MinimumSpawnAge, 1, 1);
            listing_Standard.End();
        }

        /// <summary>
        ///     The title for the mod-settings
        /// </summary>
        /// <returns></returns>
        public override string SettingsCategory()
        {
            return "Just Better Pawn Generation";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            NoMoreRandomSkills.UpdateMinimumAge();
        }
    }
}