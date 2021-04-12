using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace NoMoreRandomSkills
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(BackstoryDatabase), "ReloadAllBackstories")]
    public static class BackstoryPatch
    {
        public static void RemoveMissingSkills(this Backstory backstory)
        {
            backstory.ResolveReferences();
            if (backstory.forcedTraits != null)
            {
                backstory.forcedTraits = PawnBioPatch.CleanUpTraitList(backstory.forcedTraits, $"Backstory: {backstory.title}");
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            instrs = instrs.MethodReplacer(typeof(Backstory).GetMethod("ResolveReferences"), typeof(BackstoryPatch).GetMethod("RemoveMissingSkills"));
            return instrs;
        }
    }
}