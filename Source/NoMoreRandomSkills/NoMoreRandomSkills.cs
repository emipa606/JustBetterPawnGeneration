using Verse;

namespace NoMoreRandomSkills
{
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
                //Log.Message($"Checking for modextension for {pawnKindDef.defName}");
                if (!pawnKindDef.HasModExtension<AgeLimit>())
                {
                    continue;
                }

                //Log.Message($"Setting age to {spawnAge} for {pawnKindDef.defName}");
                pawnKindDef.minGenerationAge = spawnAge;
            }
        }
    }
}