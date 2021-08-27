using System.Collections.Generic;
using System.Reflection;
using AlienRace;
using HarmonyLib;
using RimWorld;
using Verse;

namespace HARPatch
{
    [StaticConstructorOnStartup]
    public class HARPatch : Mod
    {
        public HARPatch(ModContentPack content)
            : base(content)
        {
            var harmonyInstance = new Harmony("Mlie.JBPGHARPatch");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(BackstoryDef), "ResolveReferences")]
        public static class ResolveReferencesPatch
        {
            // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
            public static void Prefix(ref BackstoryDef __instance)
            {
                if (!__instance.forcedTraits.Any())
                {
                    return;
                }

                var resultingTraits = new List<AlienTraitEntry>();
                foreach (var traitEntry in __instance.forcedTraits)
                {
                    if (traitEntry == null)
                    {
                        continue;
                    }

                    var traitDef = DefDatabase<TraitDef>.GetNamedSilentFail(traitEntry.defName.defName);
                    if (traitDef == null)
                    {
                        Log.Message(
                            $"JBPG: Removing {traitEntry.defName.defName} for backstory {__instance.defName} since it does not exist");
                        continue;
                    }

                    if (traitDef.degreeDatas.Any(d => d.degree == traitEntry.degree))
                    {
                        resultingTraits.Add(traitEntry);
                    }
                    else
                    {
                        if (traitEntry.degree > 0)
                        {
                            for (var i = traitEntry.degree; i >= 0; i--)
                            {
                                var testingDegree = i;
                                if (!traitDef.degreeDatas.Any(d => d.degree == testingDegree))
                                {
                                    continue;
                                }

                                traitEntry.degree = i;
                                resultingTraits.Add(traitEntry);
                                Log.Message(
                                    $"JBPG: Lowering {traitDef.defName} degree to {traitEntry.degree} for backstory {__instance.defName}");
                                break;
                            }
                        }

                        // ReSharper disable once InvertIf
                        if (traitEntry.degree < 0)
                        {
                            for (var i = traitEntry.degree; i <= 0; i++)
                            {
                                var testingDegree = i;
                                if (!traitDef.degreeDatas.Any(d => d.degree == testingDegree))
                                {
                                    continue;
                                }

                                traitEntry.degree = i;
                                resultingTraits.Add(traitEntry);
                                Log.Message(
                                    $"JBPG: Raising {traitDef.defName} degree to {traitEntry.degree} for backstory {__instance.defName}");
                                break;
                            }
                        }
                    }

                    if (!traitDef.degreeDatas.Any(d => d.degree == traitEntry.degree))
                    {
                        Log.Message(
                            $"JBPG: Removing {traitDef.defName} degree {traitEntry.degree} for backstory {__instance.defName} since it does not exist");
                    }

                    __instance.forcedTraits = resultingTraits;
                }
            }
        }
    }
}