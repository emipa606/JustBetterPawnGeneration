using Verse;

namespace NoMoreRandomSkills;

[StaticConstructorOnStartup]
internal class NoMoreRandomSkills
{
    static NoMoreRandomSkills()
    {
        UpdateMinimumAge();
    }

    public static void UpdateMinimumAge()
    {
        var spawnAge = LoadedModManager.GetMod<NoMoreRandomSkillsMod>().GetSettings<NoMoreRandomSkillsSettings>()
            .MinimumSpawnAge;
        foreach (var pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
        {
            if (!pawnKindDef.HasModExtension<AgeLimit>())
            {
                continue;
            }

            pawnKindDef.minGenerationAge = spawnAge;
        }
    }
}