using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoMoreRandomSkills
{
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
                    // Log.Message($"Found: {backstoryCategoryFilter.categories[j]}");
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

                    // Log.Message($"Final: {backstoryCategoryFilter.categories[j]}");
                }
            }
        }
    }
}