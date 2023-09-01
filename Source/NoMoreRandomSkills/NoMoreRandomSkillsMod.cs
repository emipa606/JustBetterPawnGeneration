using System.Reflection;
using HarmonyLib;
using Mlie;
using UnityEngine;
using Verse;

namespace NoMoreRandomSkills;

[StaticConstructorOnStartup]
internal class NoMoreRandomSkillsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static NoMoreRandomSkillsMod instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    public readonly NoMoreRandomSkillsSettings Settings;

    /// <summary>
    ///     Cunstructor
    /// </summary>
    /// <param name="content"></param>
    public NoMoreRandomSkillsMod(ModContentPack content)
        : base(content)
    {
        instance = this;
        Settings = GetSettings<NoMoreRandomSkillsSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        var harmonyInstance = new Harmony("com.nomorerandomskills.patch");
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
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
        listing_Standard.CheckboxLabeled("REB.OnlyCustomBackstories".Translate(), ref Settings.OnlyCustomBackstories,
            "REB.OnlyCustomBackstoriesTT".Translate());
        listing_Standard.Label("REB.MinimumSpawnAge".Translate(Settings.MinimumSpawnAge), -1,
            "REB.MinimumSpawnAgeTT".Translate());
        listing_Standard.IntAdjuster(ref Settings.MinimumSpawnAge, 1, 1);
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("REB.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

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