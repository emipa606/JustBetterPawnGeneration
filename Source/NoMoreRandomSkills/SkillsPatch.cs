using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoMoreRandomSkills
{

    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    internal class NoMoreRandomSkills : Mod
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public NoMoreRandomSkills(ModContentPack content) : base(content)
        {
            var harmonyInstance = new Harmony("com.nomorerandomskills.patch");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(TraitDef), "DataAtDegree", new Type[] { typeof(int) })]
    public static class DataAtDegreePatch
    {
        static bool Prefix(ref TraitDegreeData __result, ref TraitDef __instance, ref int degree)
        {
            __result = __instance.degreeDatas[0];
            return false;
        }
    }

    [HarmonyPatch(typeof(Backstory), "ConfigErrors", new Type[] { typeof(bool) })]
    public static class ConfigErrorsPatch
    {
        static IEnumerable<string> Postfix(IEnumerable<string> values)
        {
            var returnValues = new List<string>();
            foreach (var value in values)
            {
                switch (value)
                {
                    case string a when a.Contains("has invalid trait Beauty degree="):
                    case string b when b.Contains("has invalid trait NaturalMood degree="):
                    case string c when c.Contains("has invalid trait Industriousness degree="):
                    case string d when d.Contains("has invalid trait Nerves degree="):
                    case string e when e.Contains("has invalid trait SpeedOffset degree="):
                    case string f when f.Contains("has invalid trait PsychicSensitivity degree="):
                        if (Prefs.DevMode) Log.Message($"Skipping error: {value} since we cannot change hardcoded backstories.");
                        break;
                    default:
                        returnValues.Add(value);
                        break;
                }
            }
            return returnValues;
        }
    }
    
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill", new Type[] { typeof(Pawn), typeof(SkillDef) })]
    public static class FinalLevelOfSkillPatch
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            instrs = Transpilers.MethodReplacer(instrs, typeof(Rand).GetMethod("RangeInclusive"), typeof(FinalLevelOfSkillPatch).GetMethod("SkillRangePatch"));
            instrs = Transpilers.MethodReplacer(instrs, typeof(Rand).GetMethod("ByCurve", new Type[] { typeof(SimpleCurve) }), typeof(FinalLevelOfSkillPatch).GetMethod("SkillRangePatch2"));
            instrs = Transpilers.MethodReplacer(instrs, typeof(Rand).GetMethod("Range", new Type[] { typeof(float), typeof(float) }), typeof(FinalLevelOfSkillPatch).GetMethod("SkillRangePatch3"));
            return instrs;
        }

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
    }
}
