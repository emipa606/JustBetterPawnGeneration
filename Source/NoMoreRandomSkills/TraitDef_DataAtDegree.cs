using HarmonyLib;
using RimWorld;

namespace NoMoreRandomSkills;

[HarmonyPatch(typeof(TraitDef), "DataAtDegree")]
public static class TraitDef_DataAtDegree
{
    public static bool Prefix(TraitDef __instance, int degree, ref TraitDegreeData __result)
    {
        foreach (var traitDegreeData in __instance.degreeDatas)
        {
            if (traitDegreeData.degree != degree)
            {
                continue;
            }

            __result = traitDegreeData;
            return false;
        }

        __result = __instance.degreeDatas[0];
        return false;
    }
}