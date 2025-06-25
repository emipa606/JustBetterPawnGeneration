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
    public static NoMoreRandomSkillsMod Instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    private readonly NoMoreRandomSkillsSettings settings;

    /// <summary>
    ///     Cunstructor
    /// </summary>
    /// <param name="content"></param>
    public NoMoreRandomSkillsMod(ModContentPack content)
        : base(content)
    {
        Instance = this;
        settings = GetSettings<NoMoreRandomSkillsSettings>();
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
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Gap();
        listingStandard.CheckboxLabeled("REB.OnlyCustomBackstories".Translate(), ref settings.OnlyCustomBackstories,
            "REB.OnlyCustomBackstoriesTT".Translate());
        listingStandard.Label("REB.MinimumSpawnAge".Translate(settings.MinimumSpawnAge), -1,
            "REB.MinimumSpawnAgeTT".Translate());
        listingStandard.IntAdjuster(ref settings.MinimumSpawnAge, 1, 1);
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("REB.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
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