using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoMoreRandomSkills
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(SolidBioDatabase), "LoadAllBios")]
    public static class PawnBioPatch
    {
        public static List<TraitEntry> CleanUpTraitList(List<TraitEntry> listOfTraits, string logName)
        {
            var resultingTraits = new List<TraitEntry>();
            foreach (var traitEntry in listOfTraits)
            {
                if (traitEntry != null && traitEntry.def.degreeDatas.Any(d => d.degree == traitEntry.degree))
                {
                    resultingTraits.Add(traitEntry);
                }
                else
                {
                    if (traitEntry == null)
                    {
                        continue;
                    }

                    if (traitEntry.degree > 0)
                    {
                        for (var i = traitEntry.degree; i >= 0; i--)
                        {
                            var testingDegree = i;
                            if (!traitEntry.def.degreeDatas.Any(d => d.degree == testingDegree))
                            {
                                continue;
                            }

                            traitEntry.degree = i;
                            resultingTraits.Add(traitEntry);
                            Log.Message(
                                $"JBPG: Lowering {traitEntry.def.defName} degree to {traitEntry.degree} for {logName}");
                            break;
                        }
                    }

                    // ReSharper disable once InvertIf
                    if (traitEntry.degree < 0)
                    {
                        for (var i = traitEntry.degree; i <= 0; i++)
                        {
                            var testingDegree = i;
                            if (!traitEntry.def.degreeDatas.Any(d => d.degree == testingDegree))
                            {
                                continue;
                            }

                            traitEntry.degree = i;
                            resultingTraits.Add(traitEntry);
                            Log.Message(
                                $"JBPG: Raising {traitEntry.def.defName} degree to {traitEntry.degree} for {logName}");
                            break;
                        }
                    }
                }

                if (!traitEntry.def.degreeDatas.Any(d => d.degree == traitEntry.degree))
                {
                    Log.Message(
                        $"JBPG: Removing {traitEntry.def.defName} degree {traitEntry.degree} for {logName} since it does not exist");
                }
            }

            return resultingTraits;
        }

        public static void RemoveMissingSkills(this PawnBio pawnBio)
        {
            pawnBio.ResolveReferences();
            if (pawnBio.childhood?.forcedTraits != null)
            {
                pawnBio.childhood.forcedTraits = CleanUpTraitList(pawnBio.childhood.forcedTraits,
                    $"PawnBio: {pawnBio.name.ToStringFull}");
            }

            if (pawnBio.adulthood?.forcedTraits != null)
            {
                pawnBio.adulthood.forcedTraits = CleanUpTraitList(pawnBio.adulthood.forcedTraits,
                    $"PawnBio: {pawnBio.name.ToStringFull}");
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            instrs = instrs.MethodReplacer(typeof(PawnBio).GetMethod("ResolveReferences"),
                typeof(PawnBioPatch).GetMethod("RemoveMissingSkills"));
            return instrs;
        }
    }
}