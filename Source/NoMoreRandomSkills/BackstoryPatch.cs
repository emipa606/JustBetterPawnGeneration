using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoMoreRandomSkills
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(BackstoryDatabase), "ReloadAllBackstories")]
    public static class BackstoryPatch
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            instrs = instrs.MethodReplacer(typeof(Backstory).GetMethod("ResolveReferences"),
                typeof(BackstoryPatch).GetMethod("RemoveMissingSkills"));
            return instrs;
        }

        public static void RemoveMissingSkills(this Backstory backstory)
        {
            backstory.ResolveReferences();
            if (backstory.forcedTraits != null)
            {
                backstory.forcedTraits =
                    PawnBioPatch.CleanUpTraitList(backstory.forcedTraits, $"Backstory: {backstory.title}");
            }
        }
    }

    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GetBackstoryCategoryFiltersFor")]
    public static class PawnBioAndNameGenerator_GetBackstoryCategoryFiltersFor
    {
        public static void Postfix(Pawn pawn, FactionDef faction, ref List<BackstoryCategoryFilter> __result)
        {
            if (!LoadedModManager.GetMod<NoMoreRandomSkillsMod>().GetSettings<NoMoreRandomSkillsSettings>()
                .OnlyCustomBackstories)
            {
                return;
            }

            foreach (var backstoryCategoryFilter in __result)
            {
                for (var j = 0; j < backstoryCategoryFilter.categories.Count; j++)
                {
                    //Log.Message($"Found: {backstoryCategoryFilter.categories[j]}");
                    if (backstoryCategoryFilter.categories[j] == "OutlanderCivil" ||
                        backstoryCategoryFilter.categories[j] == "OutlanderRough" ||
                        backstoryCategoryFilter.categories[j] == "Outlander" ||
                        backstoryCategoryFilter.categories[j] == "Slave" ||
                        backstoryCategoryFilter.categories[j] == "Tribal" ||
                        backstoryCategoryFilter.categories[j] == "TribeCivil" ||
                        backstoryCategoryFilter.categories[j] == "TribeRough" ||
                        backstoryCategoryFilter.categories[j] == "Pirate" ||
                        backstoryCategoryFilter.categories[j] == "Trader" ||
                        backstoryCategoryFilter.categories[j] == "PlayerColony" ||
                        backstoryCategoryFilter.categories[j] == "Offworld")
                    {
                        backstoryCategoryFilter.categories[j] = "REB_" + backstoryCategoryFilter.categories[j];
                    }

                    //Log.Message($"Final: {backstoryCategoryFilter.categories[j]}");
                }
            }
        }
    }
}