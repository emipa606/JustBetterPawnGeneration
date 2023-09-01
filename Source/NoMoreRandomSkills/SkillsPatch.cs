using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace NoMoreRandomSkills;

[HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill", null)]
public static class SkillsPatch
{
    public static int SkillRangePatch(int min, int max)
    {
        return 0;
    }

    public static float SkillRangePatch2(SimpleCurve curve)
    {
        return 0f;
    }

    public static float SkillRangePatch3(float min, float max)
    {
        return 1f;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
    {
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("RangeInclusive"),
            typeof(SkillsPatch).GetMethod("SkillRangePatch"));
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("ByCurve"),
            typeof(SkillsPatch).GetMethod("SkillRangePatch2"));
        instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("Range", new[] { typeof(float), typeof(float) }),
            typeof(SkillsPatch).GetMethod("SkillRangePatch3"));
        return instrs;
    }
}