using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace NoMoreRandomSkills;

[HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill", null)]
public static class PawnGenerator_FinalLevelOfSkill
{
    public static int SkillRangeInclusive(int min, int max)
    {
        return 0;
    }

    public static float SkillByCurve(SimpleCurve curve)
    {
        return 0f;
    }

    public static float SkillRange(float min, float max)
    {
        return 1f;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
    {
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod(nameof(Rand.RangeInclusive)),
            typeof(PawnGenerator_FinalLevelOfSkill).GetMethod(nameof(SkillRangeInclusive)));
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod(nameof(Rand.ByCurve)),
            typeof(PawnGenerator_FinalLevelOfSkill).GetMethod(nameof(SkillByCurve)));
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod(nameof(Rand.Range), [typeof(float), typeof(float)]),
            typeof(PawnGenerator_FinalLevelOfSkill).GetMethod(nameof(SkillRange)));
        return instrs;
    }
}