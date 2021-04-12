using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace NoMoreRandomSkills
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill", null)]
    public static class SkillsPatch
    {
        // Token: 0x06000003 RID: 3 RVA: 0x0000213C File Offset: 0x0000033C
        public static int SkillRangePatch(int min, int max)
        {
            return 0;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002150 File Offset: 0x00000350
        public static float SkillRangePatch2(SimpleCurve curve)
        {
            return 0f;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002168 File Offset: 0x00000368
        public static float SkillRangePatch3(float min, float max)
        {
            return 1f;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("RangeInclusive"), typeof(SkillsPatch).GetMethod("SkillRangePatch"));
            instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("ByCurve"), typeof(SkillsPatch).GetMethod("SkillRangePatch2"));
            instrs = instrs.MethodReplacer(typeof(Rand).GetMethod("Range", new[] { typeof(float), typeof(float) }), typeof(SkillsPatch).GetMethod("SkillRangePatch3"));
            return instrs;
        }
    }
}