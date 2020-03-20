using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace REB_Code {
	
	[StaticConstructorOnStartup]
	internal static class REB_Initializer {
		public static Dictionary<string, Backstory> REB_Backstories = new Dictionary<string, Backstory>();
		public static List<string> NamesFirstMale = new List<string>();
		public static List<string> NamesFirstFemale = new List<string>();
		public static List<string> NamesNicksMale = new List<string>();
		public static List<string> NamesNicksFemale = new List<string>();
		public static List<string> NamesNicksUnisex = new List<string>();
		public static List<string> NamesLast = new List<string>();
		public static int adultCount;
		public static int adultNSCount;
		public static int childCount;
		public static int childNSCount;
		public static int adultCountHAR;
		public static int adultNSCountHAR;
		public static int childCountHAR;
		public static int childNSCountHAR;
		public static int firstCount;
		public static int nickCount;
		public static int lastCount;
		public static int fullCount;
		public static int fullBioCount;
		public static bool detectedREPN = false;
		static REB_Initializer() {
			HarmonyInstance harmony = HarmonyInstance.Create("net.rainbeau.rimworld.mod.backstories");
			harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "GiveAppropriateBioAndNameTo"), new HarmonyMethod(typeof(REB_PawnBioAndNameGenerator), "GiveAppropriateBioAndNameToPrefix"), null);
			harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "FillBackstorySlotShuffled"), new HarmonyMethod(typeof(REB_PawnBioAndNameGenerator), "FillBackstorySlotShuffledPrefix"), null);
			harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "TryGetRandomUnusedSolidBioFor"), new HarmonyMethod(typeof(REB_PawnBioAndNameGenerator), "TryGetRandomUnusedSolidBioForPrefix"), null);
			harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "TryGetRandomUnusedSolidName"), new HarmonyMethod(typeof(REB_PawnBioAndNameGenerator), "TryGetRandomUnusedSolidNamePrefix"), null);
			harmony.Patch(AccessTools.Method(typeof(SolidBioDatabase), "LoadAllBios"), new HarmonyMethod(typeof(REB_SolidBioDatabase), "LoadAllBiosPrefix"), null);
			harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateBodyType"), new HarmonyMethod(typeof(REB_PawnGenerator), "GenerateBodyTypePrefix"), null);
			LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);
		}
		public static void Setup() {
			if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("[RF] Editable Pawn Names"))) {
				detectedREPN = true;
			}
			if (Controller.Settings.useLiteMode.Equals(false)) {
				foreach (KeyValuePair<string,Backstory> story in BackstoryDatabase.allBackstories) {
					if (story.Key.Equals("ColonySettler43")) {
						break;
				    }
					REB_Initializer.REB_Backstories.Add(story.Key, story.Value);
					if (story.Value.slot == BackstorySlot.Childhood) {
						if (story.Value.shuffleable.Equals(true)) { childCountHAR++; }
						else { childNSCountHAR++; }
					}
					else {
						if (story.Value.shuffleable.Equals(true)) { adultCountHAR++; }
						else { adultNSCountHAR++; }
					}
				}
				BackstoryDatabase.allBackstories.Clear();
				BackstoryDatabase.allBackstories = REB_Backstories;
			}
			Log.Message("||========================================");
			Log.Message("|| Setting up Editable Backstories (REB)");
			Log.Message("||========================================");
			if (Controller.Settings.useLiteMode.Equals(true)) {
				Log.Message("|| 'Lite Mode' in Use.");
				Log.Message("|| Vanilla Backstory Database *NOT* Cleared.");
				Log.Message("|| REB Backstories Added to Vanilla Backstory Database:");
				Log.Message("||    Childhood Backstories (shuffleable): "+childCount);
				Log.Message("||    Childhood Backstories (non-shuffleable): "+childNSCount);
				Log.Message("||    Adulthood Backstories (shuffleable): "+adultCount);
				Log.Message("||    Adulthood Backstories (non-shuffleable): "+adultNSCount);
				Log.Message("||========================================");
			}
			else {
				Log.Message("|| Vanilla Backstory Database Cleared.");
				Log.Message("|| Backstories in REB Backstory Database:");
				Log.Message("||    Childhood Backstories (shuffleable): "+childCount);
				Log.Message("||    Childhood Backstories (non-shuffleable): "+childNSCount);
				Log.Message("||    Adulthood Backstories (shuffleable): "+adultCount);
				Log.Message("||    Adulthood Backstories (non-shuffleable): "+adultNSCount);
				Log.Message("|| Humanoid Alien Races (HAR) Backstories Added to REB Database:");
				Log.Message("||    HAR Childhood Backstories (shuffleable): "+childCountHAR);
				Log.Message("||    HAR Childhood Backstories (non-shuffleable): "+childNSCountHAR);
				Log.Message("||    HAR Adulthood Backstories (shuffleable): "+adultCountHAR);
				Log.Message("||    HAR Adulthood Backstories (non-shuffleable): "+adultNSCountHAR);
				Log.Message("||========================================");
			}
			if (detectedREPN.Equals(true)) {
				Log.Message("|| Rainbeau's \"Editable Pawn Names\" Detected.");
				Log.Message("|| Names Added to REPN Name Database:");
				Log.Message("||    Full Names (with bios): "+fullBioCount);
				Log.Message("||========================================");
			}
			else {
				Log.Message("|| Vanilla Name Database *NOT* Cleared.");
				Log.Message("|| Names Added to Vanilla Name Database:");
				Log.Message("||    First Names: "+firstCount);
				Log.Message("||    Nicknames: "+nickCount);
				Log.Message("||    Last Names: "+lastCount);
				Log.Message("||    Full Names (without bios): "+fullCount);
				Log.Message("||    Full Names (with bios): "+fullBioCount);
				Log.Message("||========================================");
			}
			NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
			if (NamesFirstMale != null) {
				nameBank.AddNames(PawnNameSlot.First, Gender.Male, NamesFirstMale);
			}
			if (NamesFirstFemale != null) {
				nameBank.AddNames(PawnNameSlot.First, Gender.Female, NamesFirstFemale);
			}
			if (NamesNicksMale != null) {
				nameBank.AddNames(PawnNameSlot.Nick, Gender.Male, NamesNicksMale);
			}
			if (NamesNicksFemale != null) {
				nameBank.AddNames(PawnNameSlot.Nick, Gender.Female, NamesNicksFemale);
			}
			if (NamesNicksUnisex != null) {
				nameBank.AddNames(PawnNameSlot.Nick, Gender.None, NamesNicksUnisex);
			}
			if (NamesLast != null) {
				nameBank.AddNames(PawnNameSlot.Last, Gender.None, NamesLast);
			}
		}
	}

	public class Controller : Mod {
		public static Settings Settings;
		public override string SettingsCategory() { return "REB.EditableBackstories".Translate(); }
		public override void DoSettingsWindowContents(Rect canvas) { Settings.DoWindowContents(canvas); }
		public Controller(ModContentPack content) : base(content) {
			Settings = GetSettings<Settings>();
		}
	}

	public class Settings : ModSettings {
		public bool useLiteMode = false;
		public void DoWindowContents(Rect canvas) {
			Listing_Standard list = new Listing_Standard();
			list.ColumnWidth = canvas.width;
			list.Begin(canvas);
			list.Gap();
			list.CheckboxLabeled( "REB.UseLiteMode".Translate(), ref useLiteMode, "REB.UseLiteModeTip".Translate() );
			list.End();
		}
		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref useLiteMode, "useLiteMode", false);
		}
	}
	
	public static class REB_PawnGenerator {
		public static bool GenerateBodyTypePrefix(Pawn pawn) {
			if (REB_Initializer.REB_Backstories.Count < 1) { return true; }
			BodyType bodyGlobal;
			BodyType bodyMale;
			BodyType bodyFemale;
			float randBodyType;
			if (pawn.story.adulthood != null) {
				bodyGlobal = pawn.story.adulthood.bodyTypeGlobal;
				bodyMale = pawn.story.adulthood.bodyTypeMale;
				bodyFemale=pawn.story.adulthood.bodyTypeFemale;
			}
			else {
				bodyGlobal = pawn.story.childhood.bodyTypeGlobal;
				bodyMale = pawn.story.childhood.bodyTypeMale;
				bodyFemale=pawn.story.childhood.bodyTypeFemale;
			}
			if (bodyGlobal == BodyType.Male) {
				randBodyType = Rand.Value;
				if (randBodyType < 0.05) {
					pawn.story.bodyType = BodyType.Hulk;
				}
				else if (randBodyType < 0.15) {
					pawn.story.bodyType = BodyType.Fat;
				}
				else if (randBodyType < 0.5) {
					pawn.story.bodyType = BodyType.Thin;
				}
				else if (pawn.gender == Gender.Female) {
					pawn.story.bodyType = BodyType.Female;
				}
				else {
					pawn.story.bodyType = BodyType.Male;
				}		
			}
			else if (bodyGlobal != BodyType.Undefined || pawn.gender == Gender.None) {
				pawn.story.bodyType = bodyGlobal;
			}
			else if (pawn.gender == Gender.Female) {
				pawn.story.bodyType = bodyFemale;
			}
			else {
				pawn.story.bodyType = bodyMale;
			}
			return false;
		}
	}
	
	public static class REB_SolidBioDatabase {
		public static bool LoadAllBiosPrefix() {
			if (REB_Initializer.REB_Backstories.Count < 1) { return true; }
			foreach (PawnBio pawnBio in DirectXmlLoader.LoadXmlDataInResourcesFolder<PawnBio>("Backstories/Solid")) {
				pawnBio.name.ResolveMissingPieces(null);
				PawnNameDatabaseSolid.AddPlayerContentName(pawnBio.name, pawnBio.gender);
			}
			return false;
		}
	}	

	public static class REB_PawnBioAndNameGenerator {
		private static List<PawnBio> tempBios = new List<PawnBio>();
		private static List<NameTriple> tempNames = new List<NameTriple>();
		public static bool GiveAppropriateBioAndNameToPrefix(Pawn pawn, string requiredLastName) {
			if (REB_Initializer.REB_Backstories.Count < 1) { return true; }
			float baseBioLikelihood = ((float)SolidBioDatabase.allBios.Count/500);
			if (((Rand.Value < (baseBioLikelihood/2)) || ((Rand.Value < (baseBioLikelihood*2)) && pawn.kindDef.factionLeader))) {
				MethodInfo solidBio = typeof(PawnBioAndNameGenerator).GetMethod("TryGiveSolidBioTo", BindingFlags.NonPublic | BindingFlags.Static);
				bool b = (bool) solidBio.Invoke(null, new object[] {pawn, requiredLastName});
				if (b.Equals(true)) {
					return false;
				}
			}
			typeof(PawnBioAndNameGenerator).GetMethod("GiveShuffledBioTo", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] {pawn, pawn.Faction.def, requiredLastName});
			return false;
		}
		[HarmonyPriority(Priority.Low)]
		public static bool FillBackstorySlotShuffledPrefix(Pawn pawn, BackstorySlot slot, ref Backstory backstory, FactionDef factionType) {
			if (!(from kvp in BackstoryDatabase.allBackstories
			where kvp.Value.shuffleable
			  && kvp.Value.spawnCategories.Contains(factionType.backstoryCategory)
			  && (!kvp.Value.spawnCategories.Contains("Uncommon")
			      || (kvp.Value.spawnCategories.Contains("Uncommon") && (Rand.Value < 0.333333)))
			  && (!kvp.Value.spawnCategories.Contains("Rare")
			      || (kvp.Value.spawnCategories.Contains("Rare") && (Rand.Value < 0.111111)))
			  && (!kvp.Value.spawnCategories.Contains("Legendary")
			      || (kvp.Value.spawnCategories.Contains("Legendary") && (Rand.Value < 0.037037)))
			  && (!kvp.Value.spawnCategories.Contains("AgeOver40")
			      || (kvp.Value.spawnCategories.Contains("AgeOver40") && pawn.ageTracker.AgeBiologicalYearsFloat > 40f))
			  && (!kvp.Value.spawnCategories.Contains("AgeOver60")
			      || (kvp.Value.spawnCategories.Contains("AgeOver60") && pawn.ageTracker.AgeBiologicalYearsFloat > 60f))
			  && (!kvp.Value.spawnCategories.Contains("AgeOver80")
			      || (kvp.Value.spawnCategories.Contains("AgeOver80") && pawn.ageTracker.AgeBiologicalYearsFloat > 80f))
			  && (!kvp.Value.spawnCategories.Contains("Male")
			      || (kvp.Value.spawnCategories.Contains("Male") && pawn.gender == Gender.Male))
			  && (!kvp.Value.spawnCategories.Contains("Female")
			      || (kvp.Value.spawnCategories.Contains("Female") && pawn.gender == Gender.Female))
			  && kvp.Value.slot == slot 
			  && (slot != BackstorySlot.Adulthood
			    || (!SkillGainsConflict(kvp.Value.skillGainsResolved, pawn.story.childhood.skillGainsResolved, kvp.Value.workDisables, pawn.story.childhood.workDisables, kvp.Value.forcedTraits, pawn.story.childhood.forcedTraits, kvp.Value.requiredWorkTags, pawn.story.childhood.requiredWorkTags)
			    && SameFilterSet(kvp.Value.spawnCategories, pawn.story.childhood.spawnCategories)))
			select kvp.Value).TryRandomElement(out backstory)) {
				Log.Error(string.Concat(new object[] {
					"No shuffled ",slot," found for ",pawn," of ",factionType,". Defaulting."
				}));
				backstory = (from kvp in BackstoryDatabase.allBackstories
				where kvp.Value.slot == slot
				select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
			}
			return false;
		}
		public static bool TryGetRandomUnusedSolidBioForPrefix(string backstoryCategory, PawnKindDef kind, Gender gender, string requiredLastName, ref PawnBio __result) {
			if (REB_Initializer.REB_Backstories.Count < 1) { return true; }
			NameTriple nameTriple = null;
			if (Rand.Value < 0.5f) {
				nameTriple = Prefs.RandomPreferredName();
				if (nameTriple != null && (nameTriple.UsedThisGame || (requiredLastName != null && nameTriple.Last != requiredLastName))) {
					nameTriple = null;
				}
			}
			while (true) {
				int i = 0;
				while (i < SolidBioDatabase.allBios.Count) {
					PawnBio pawnBio = SolidBioDatabase.allBios[i];
					if (pawnBio.gender == GenderPossibility.Either) {
						goto IL_8F;
					}
					if (gender != Gender.Male || pawnBio.gender == GenderPossibility.Male) {
						if (gender != Gender.Female || pawnBio.gender == GenderPossibility.Female) {
							goto IL_8F;
						}
					}
					IL_14E:
					i++;
					continue;
					IL_8F:
					if (!requiredLastName.NullOrEmpty() && pawnBio.name.Last != requiredLastName) {
						goto IL_14E;
					}
					if (pawnBio.name.UsedThisGame) {
						goto IL_14E;
					}
					if (nameTriple != null && !pawnBio.name.Equals(nameTriple)) {
						goto IL_14E;
					}
					for (int j = 0; j < pawnBio.adulthood.spawnCategories.Count; j++) {
						if (pawnBio.adulthood.spawnCategories[j] == backstoryCategory) {
							REB_PawnBioAndNameGenerator.tempBios.Add(pawnBio);
							break;
						}
					}
					goto IL_14E;
				}
				if (REB_PawnBioAndNameGenerator.tempBios.Count != 0 || nameTriple == null) {
					break;
				}
				nameTriple = null;
			}
			PawnBio result;
			REB_PawnBioAndNameGenerator.tempBios.TryRandomElement(out result);
			REB_PawnBioAndNameGenerator.tempBios.Clear();
			__result = result;
			return false;
		}
		public static bool TryGetRandomUnusedSolidNamePrefix(Gender gender, ref NameTriple __result, string requiredLastName = null) {
			if (REB_Initializer.REB_Backstories.Count < 1) { return true; }
			NameTriple nameTriple = null;
			if (Rand.Value < 0.5f) {
				nameTriple = Prefs.RandomPreferredName();
				if (nameTriple != null && (nameTriple.UsedThisGame || (requiredLastName != null && nameTriple.Last != requiredLastName))) {
					nameTriple = null;
				}
			}
			List<NameTriple> listForGender;
			List<NameTriple> list;
			listForGender = PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either);
			list = (gender != Gender.Male) ? PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female) : PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male);
			float num = ((float)listForGender.Count + 0.1f) / ((float)(listForGender.Count + list.Count) + 0.1f);
			List<NameTriple> list2;
			if (listForGender.Count == 0) {
				list2 = list; 
			}
			else {
				if (Rand.Value < num) {
					list2 = listForGender;
				}
				else {
					list2 = list;
				}
			}
			if (list2.Count == 0) {
				Log.Error("Empty solid pawn name list for gender: " + gender + ".");
				__result = null;
				return false;
			}
			if (nameTriple != null && list2.Contains(nameTriple)) {
				__result = nameTriple;
				return false;
			}
			for (int i = 0; i < list2.Count; i++) {
				NameTriple nameTriple2 = list2[i];
				if (requiredLastName == null || !(nameTriple2.Last != requiredLastName)) {
					if (!nameTriple2.UsedThisGame) {
						REB_PawnBioAndNameGenerator.tempNames.Add(nameTriple2);
					}
				}
			}
			NameTriple result;
			REB_PawnBioAndNameGenerator.tempNames.TryRandomElement(out result);
			REB_PawnBioAndNameGenerator.tempNames.Clear();
			__result = result;
			return false;
		}
		public static bool SkillGainsConflict(Dictionary<SkillDef,int> a, Dictionary<SkillDef,int> b, WorkTags aDisabled, WorkTags bDisabled, List<TraitEntry> aTraits, List<TraitEntry> bTraits, WorkTags aRequired, WorkTags bRequired) {
			List<WorkTypeDef> allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
			for (int i = 0; i < allWorkTypes.Count; i++) {
				WorkTypeDef item = allWorkTypes[i];
				if (((item.workTags & aRequired) != WorkTags.None && (item.workTags & bDisabled) != WorkTags.None)
				  || ((item.workTags & bRequired) != WorkTags.None && (item.workTags & aDisabled) != WorkTags.None)) {
					return true;
				}
			}
			if (((WorkTags.ManualDumb & aRequired) != WorkTags.None && ((WorkTags.Cleaning & bDisabled) != WorkTags.None || (WorkTags.Hauling & bDisabled) != WorkTags.None || (WorkTags.PlantWork & bDisabled) != WorkTags.None))
			    || ((WorkTags.ManualDumb & bRequired) != WorkTags.None && ((WorkTags.Cleaning & aDisabled) != WorkTags.None || (WorkTags.Hauling & aDisabled) != WorkTags.None || (WorkTags.PlantWork & aDisabled) != WorkTags.None))) {
				return true;
			}
			if (((WorkTags.ManualDumb & aDisabled) != WorkTags.None && ((WorkTags.Cleaning & bRequired) != WorkTags.None || (WorkTags.Hauling & bRequired) != WorkTags.None || (WorkTags.PlantWork & bRequired) != WorkTags.None))
			    || ((WorkTags.ManualDumb & bDisabled) != WorkTags.None && ((WorkTags.Cleaning & aRequired) != WorkTags.None || (WorkTags.Hauling & aRequired) != WorkTags.None || (WorkTags.PlantWork & aRequired) != WorkTags.None))) {
				return true;
			}
			if (((WorkTags.ManualSkilled & aRequired) != WorkTags.None && ((WorkTags.Cooking & bDisabled) != WorkTags.None || (WorkTags.Crafting & bDisabled) != WorkTags.None || (WorkTags.Mining & bDisabled) != WorkTags.None || (WorkTags.PlantWork & bDisabled) != WorkTags.None))
			    || ((WorkTags.ManualSkilled & bRequired) != WorkTags.None && ((WorkTags.Cooking & aDisabled) != WorkTags.None || (WorkTags.Crafting & aDisabled) != WorkTags.None || (WorkTags.Mining & aDisabled) != WorkTags.None || (WorkTags.PlantWork & aDisabled) != WorkTags.None))) {
				return true;
			}
			if (((WorkTags.ManualSkilled & aDisabled) != WorkTags.None && ((WorkTags.Cooking & bRequired) != WorkTags.None || (WorkTags.Crafting & bRequired) != WorkTags.None || (WorkTags.Mining & bRequired) != WorkTags.None || (WorkTags.PlantWork & bRequired) != WorkTags.None))
			    || ((WorkTags.ManualSkilled & bDisabled) != WorkTags.None && ((WorkTags.Cooking & aRequired) != WorkTags.None || (WorkTags.Crafting & aRequired) != WorkTags.None || (WorkTags.Mining & aRequired) != WorkTags.None || (WorkTags.PlantWork & aRequired) != WorkTags.None))) {
				return true;
			}
			List<SkillDef> allSkills = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allSkills.Count; i++) {
				SkillDef skill = allSkills[i];
				if (a.ContainsKey(skill) && b.ContainsKey(skill)) {
					if ((a[skill] > 3 && b[skill] < -3) || (b[skill] > 3 && a[skill] < -3)) {
						return true;
					}
				}
			}
			if (a.ContainsKey(SkillDefOf.Animals)) {
			    if ((bRequired & WorkTags.Animals) != WorkTags.None) {
			    	if (a[SkillDefOf.Animals] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Animals) != WorkTags.None) {
					if (a[SkillDefOf.Animals] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Animals)) {
			    if ((aRequired & WorkTags.Animals) != WorkTags.None) {
			    	if (b[SkillDefOf.Animals] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Animals) != WorkTags.None) {
					if (b[SkillDefOf.Animals] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Artistic)) {
			    if ((bRequired & WorkTags.Artistic) != WorkTags.None) {
			    	if (a[SkillDefOf.Artistic] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Artistic) != WorkTags.None) {
					if (a[SkillDefOf.Artistic] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Artistic)) {
			    if ((aRequired & WorkTags.Artistic) != WorkTags.None) {
			    	if (b[SkillDefOf.Artistic] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Artistic) != WorkTags.None) {
					if (b[SkillDefOf.Artistic] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Construction)) {
			    if ((bRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (a[SkillDefOf.Construction] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (a[SkillDefOf.Construction] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Construction)) {
			    if ((aRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (b[SkillDefOf.Construction] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (b[SkillDefOf.Construction] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Cooking)) {
			    if ((bRequired & WorkTags.Cooking) != WorkTags.None || (bRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (a[SkillDefOf.Cooking] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Cooking) != WorkTags.None || (bDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (a[SkillDefOf.Cooking] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Cooking)) {
				if ((aRequired & WorkTags.Cooking) != WorkTags.None || (aRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (b[SkillDefOf.Cooking] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Cooking) != WorkTags.None || (aDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (b[SkillDefOf.Cooking] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Crafting)) {
			    if ((bRequired & WorkTags.Crafting) != WorkTags.None || (bRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (a[SkillDefOf.Crafting] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Crafting) != WorkTags.None || (bDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (a[SkillDefOf.Crafting] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Crafting)) {
				if ((aRequired & WorkTags.Crafting) != WorkTags.None || (aRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (b[SkillDefOf.Crafting] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Crafting) != WorkTags.None || (aDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (b[SkillDefOf.Crafting] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Growing)) {
			    if ((bRequired & WorkTags.PlantWork) != WorkTags.None || (bRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (a[SkillDefOf.Growing] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.PlantWork) != WorkTags.None || (bDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (a[SkillDefOf.Growing] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Growing)) {
				if ((aRequired & WorkTags.PlantWork) != WorkTags.None || (aRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (b[SkillDefOf.Growing] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.PlantWork) != WorkTags.None || (aDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (b[SkillDefOf.Growing] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Medicine)) {
			    if ((bRequired & WorkTags.Caring) != WorkTags.None) {
			    	if (a[SkillDefOf.Medicine] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Caring) != WorkTags.None) {
					if (a[SkillDefOf.Medicine] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Medicine)) {
			    if ((aRequired & WorkTags.Caring) != WorkTags.None) {
			    	if (b[SkillDefOf.Medicine] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Caring) != WorkTags.None) {
					if (b[SkillDefOf.Medicine] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Mining)) {
			    if ((bRequired & WorkTags.Mining) != WorkTags.None || (bRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (a[SkillDefOf.Mining] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Mining) != WorkTags.None || (bDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (a[SkillDefOf.Mining] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Mining)) {
				if ((aRequired & WorkTags.Mining) != WorkTags.None || (aRequired & WorkTags.ManualSkilled) != WorkTags.None) {
			    	if (b[SkillDefOf.Mining] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Mining) != WorkTags.None || (aDisabled & WorkTags.ManualSkilled) != WorkTags.None) {
					if (b[SkillDefOf.Mining] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Intellectual)) {
			    if ((bRequired & WorkTags.Intellectual) != WorkTags.None) {
			    	if (a[SkillDefOf.Intellectual] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Intellectual) != WorkTags.None) {
					if (a[SkillDefOf.Intellectual] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Intellectual)) {
			    if ((aRequired & WorkTags.Intellectual) != WorkTags.None) {
			    	if (b[SkillDefOf.Intellectual] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Intellectual) != WorkTags.None) {
					if (b[SkillDefOf.Intellectual] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Social)) {
			    if ((bRequired & WorkTags.Social) != WorkTags.None) {
			    	if (a[SkillDefOf.Social] < -3) { return true; }
			    }
				if ((bDisabled & WorkTags.Social) != WorkTags.None) {
					if (a[SkillDefOf.Social] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Social)) {
			    if ((aRequired & WorkTags.Social) != WorkTags.None) {
			    	if (b[SkillDefOf.Social] < -3) { return true; }
			    }
				if ((aDisabled & WorkTags.Social) != WorkTags.None) {
					if (b[SkillDefOf.Social] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Melee)) {
				if ((bDisabled & WorkTags.Violent) != WorkTags.None) {
					if (a[SkillDefOf.Melee] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Melee)) {
				if ((aDisabled & WorkTags.Violent) != WorkTags.None) {
					if (b[SkillDefOf.Melee] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Shooting)) {
				if ((bDisabled & WorkTags.Violent) != WorkTags.None) {
					if (a[SkillDefOf.Shooting] > 3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Shooting)) {
				if ((aDisabled & WorkTags.Violent) != WorkTags.None) {
					if (b[SkillDefOf.Shooting] > 3) { return true; }
			    }
			}
			if (a.ContainsKey(SkillDefOf.Melee) && a.ContainsKey(SkillDefOf.Shooting)) {
			    if ((bRequired & WorkTags.Violent) != WorkTags.None) {
					if (a[SkillDefOf.Melee] < -3 && a[SkillDefOf.Shooting] < -3) { return true; }
			    }
			}
			if (b.ContainsKey(SkillDefOf.Melee) && b.ContainsKey(SkillDefOf.Shooting)) {
			    if ((aRequired & WorkTags.Violent) != WorkTags.None) {
					if (b[SkillDefOf.Melee] < -3 && b[SkillDefOf.Shooting] < -3) { return true; }
			    }
			}
			return false;
		}
		public static bool SameFilterSet(List<string> a, List<string> b) {
			bool sameSet = true;
			if (a.Contains("FilterSet1")) {
				if (b.Contains("FilterSet1")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet2")) {
				if (b.Contains("FilterSet2")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet3")) {
				if (b.Contains("FilterSet3")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet4")) {
				if (b.Contains("FilterSet4")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet5")) {
				if (b.Contains("FilterSet5")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet6")) {
				if (b.Contains("FilterSet6")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet7")) {
				if (b.Contains("FilterSet7")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet8")) {
				if (b.Contains("FilterSet8")) { return true; }
				else { sameSet = false; }
			}
			if (a.Contains("FilterSet9")) {
				if (b.Contains("FilterSet9")) { return true; }
				else { sameSet = false; }
			}
			if (b.Contains("FilterSet1") || b.Contains("FilterSet2") || b.Contains("FilterSet3") || b.Contains("FilterSet4") || b.Contains("FilterSet5") || b.Contains("FilterSet6") || b.Contains("FilterSet7") || b.Contains("FilterSet8") || b.Contains("FilterSet9")) {
				sameSet = false;
			}
			if (sameSet == true) { return true; }
			return false;
		}
	}
	
//
//	Read Backstories
//
	
	public class BackstoryDef : Def {
		#region XML Data
		public string baseDesc;
		public BodyType bodyTypeGlobal = BodyType.Undefined;
		public BodyType bodyTypeMale = BodyType.Undefined;
		public BodyType bodyTypeFemale = BodyType.Undefined;
		public string title;
		public string titleShort;
		public BackstorySlot slot = BackstorySlot.Adulthood;
		public bool shuffleable = true;
		public bool addToDatabase = true;
		public List<WorkTags> workAllows = new List<WorkTags>();
		public List<WorkTags> workDisables = new List<WorkTags>();
		public List<WorkTags> requiredWorkTags = new List<WorkTags>();
		public List<BackstoryDefSkillListItem> skillGains = new List<BackstoryDefSkillListItem>();
		public List<string> spawnCategories = new List<string>();
		public List<BackstoryDefTraitListItem> forcedTraits = new List<BackstoryDefTraitListItem>();
		public List<BackstoryDefTraitListItem> disallowedTraits = new List<BackstoryDefTraitListItem>();
		#endregion
		public static BackstoryDef Named(string defName) {
			return DefDatabase<BackstoryDef>.GetNamed(defName);
		}
		public override void ResolveReferences() {
			base.ResolveReferences();
			if (!this.addToDatabase) return;
			if (Controller.Settings.useLiteMode.Equals(true)) {
				if (BackstoryDatabase.allBackstories.ContainsKey(this.UniqueSaveKey())) {
					Log.Error("Backstory Error ("+this.defName+"): Duplicate defName.");
					return;
				}
			}
			else {
				if (REB_Initializer.REB_Backstories.ContainsKey(this.UniqueSaveKey())) {
					Log.Error("Backstory Error ("+this.defName+"): Duplicate defName.");
					return;
				}
			}
			Backstory b = new Backstory();
			if (!this.title.NullOrEmpty())
				b.SetTitle(this.title);
			else {
				return;
			}
			if (!titleShort.NullOrEmpty())
				b.SetTitleShort(titleShort);
			else
				b.SetTitleShort(b.Title);
			if (!baseDesc.NullOrEmpty())
				b.baseDesc = baseDesc;
			else {
				b.baseDesc = "Empty.";
			}
			if (bodyTypeMale == BodyType.Undefined) {
				bodyTypeMale = BodyType.Male;
				if (bodyTypeGlobal == BodyType.Undefined) {
					bodyTypeGlobal = BodyType.Male;
				}
			}
			if (bodyTypeFemale == BodyType.Undefined) {
				bodyTypeFemale = BodyType.Female;
				if (bodyTypeGlobal == BodyType.Undefined) {
					bodyTypeGlobal = BodyType.Male;
				}
			}
			b.bodyTypeGlobal = bodyTypeGlobal;
			b.bodyTypeMale = bodyTypeMale;
			b.bodyTypeFemale = bodyTypeFemale;
			b.slot = slot;
			b.shuffleable = shuffleable;
			if (spawnCategories.NullOrEmpty()) {
				return;
			}
			else
				b.spawnCategories = spawnCategories;
			if (workAllows.Count > 0) {
				foreach (WorkTags current in Enum.GetValues(typeof(WorkTags))) {
					if (!workAllows.Contains(current)) {
						b.workDisables |= current;
					}
				}
			}
			else if (workDisables.Count > 0) {
				foreach (var tag in workDisables) {
					b.workDisables |= tag;
				}
			}
			else {
				b.workDisables = WorkTags.None;
			}
			if (requiredWorkTags.Count > 0) {
				foreach (var tag in requiredWorkTags) {
					b.requiredWorkTags |= tag;
				}
			}
			else {
				b.requiredWorkTags = WorkTags.None;
			}
			b.skillGains = skillGains.ToDictionary(i => i.key, i => i.value);
			Dictionary<string, int> fTraitList = forcedTraits.ToDictionary(i => i.key, i=> i.value);
			if (fTraitList.Count > 0) {
				b.forcedTraits = new List<TraitEntry>();
				foreach (KeyValuePair<string,int> trait in fTraitList) {
					b.forcedTraits.Add(new TraitEntry(TraitDef.Named(trait.Key), trait.Value));
				}
			}
			Dictionary<string, int> dTraitList = disallowedTraits.ToDictionary(i => i.key, i=> i.value);
			if (dTraitList.Count > 0) {
				b.disallowedTraits = new List<TraitEntry>();
				foreach (KeyValuePair<string,int> trait in dTraitList) {
					b.disallowedTraits.Add(new TraitEntry(TraitDef.Named(trait.Key), trait.Value));
				}
			}
			b.ResolveReferences();
			b.PostLoad();
			b.identifier = this.UniqueSaveKey();
			bool flag = false;
			foreach (var s in b.ConfigErrors(false)) {
				Log.Error("Backstory Error ("+b.identifier+"): "+s);
				if (!flag) {
					flag = true;
				}
			}
			if (!flag)	{
				if (b.slot.Equals(BackstorySlot.Adulthood)) {
					if (b.shuffleable.Equals(true)) {
						REB_Initializer.adultCount++;
					}
					else {
						REB_Initializer.adultNSCount++;
					}
				}
				if (b.slot.Equals(BackstorySlot.Childhood)) {
					if (b.shuffleable.Equals(true)) {
						REB_Initializer.childCount++;
					}
					else {
						REB_Initializer.childNSCount++;
					}
				}
				if (Controller.Settings.useLiteMode.Equals(true)) {
					BackstoryDatabase.allBackstories.Add(b.identifier, b);
				}
				else {
					REB_Initializer.REB_Backstories.Add(b.identifier, b);
				}
			}
		}
	}

	public static class BackstoryDefExt {
		public static string UniqueSaveKey(this BackstoryDef def) {
			return "REB_" + def.defName;
		}
	}

	public struct BackstoryDefSkillListItem {
		public string key;
		public int value;
	}
	
	public struct BackstoryDefTraitListItem {
		public string key;
		public int value;
	}

//
//	Read Names
//

	public class NameDef : Def {
		#region XML Data
		public GenderPossibility gender;
		public NameTriple name;
		public string childhoodStory;
		public string adulthoodStory;
		#endregion
		public static NameDef Named(string defName) {
			return DefDatabase<NameDef>.GetNamed(defName);
		}
		public PawnBioType BioType {
			get { return PawnBioType.Undefined; }
		}
		public override void ResolveReferences() {
			base.ResolveReferences();
			PawnBio bio = new PawnBio();
			bio.name = name;
			bio.gender = gender;
			if (bio.gender != GenderPossibility.Male && bio.gender != GenderPossibility.Female) {
				bio.gender = GenderPossibility.Either;
			}
			bio.PostLoad();
			if (bio.name.First.NullOrEmpty() || bio.name.Last.NullOrEmpty()) {
				if (!childhoodStory.NullOrEmpty() || !adulthoodStory.NullOrEmpty()) {
					Log.Error("Backstory Error ("+bio.name+"): A locked backstory can only be attached to a full name.");
				}
				if (bio.name.First.NullOrEmpty() && bio.name.Last.NullOrEmpty()) {
					if (!bio.name.Nick.NullOrEmpty()) {
						if (bio.gender == GenderPossibility.Male) {
							REB_Initializer.NamesNicksMale.Add(bio.name.Nick);
							REB_Initializer.nickCount++;
						}
						else if (bio.gender == GenderPossibility.Female) {
							REB_Initializer.NamesNicksFemale.Add(bio.name.Nick);
							REB_Initializer.nickCount++;
						}
						else {
							REB_Initializer.NamesNicksUnisex.Add(bio.name.Nick);
							REB_Initializer.nickCount++;
						}
					}
				}
				else if (bio.name.First.NullOrEmpty()) {
					if (!bio.name.Last.NullOrEmpty()) {
						REB_Initializer.NamesLast.Add(bio.name.Last);
							REB_Initializer.lastCount++;
					}
				}
				else if (bio.name.Last.NullOrEmpty()) {
					if (!bio.name.First.NullOrEmpty()) {
						if (bio.gender == GenderPossibility.Male) {
							REB_Initializer.NamesFirstMale.Add(bio.name.First);
							REB_Initializer.firstCount++;
						}
						else if (bio.gender == GenderPossibility.Female) {
							REB_Initializer.NamesFirstFemale.Add(bio.name.First);
							REB_Initializer.firstCount++;
						}
						else {
							REB_Initializer.NamesFirstMale.Add(bio.name.First);
							REB_Initializer.NamesFirstFemale.Add(bio.name.First);
							REB_Initializer.firstCount++;
						}
					}
				}
			}
			else {
				bio.name.ResolveMissingPieces(null);
				if ((!childhoodStory.NullOrEmpty() && adulthoodStory.NullOrEmpty())
				  || (childhoodStory.NullOrEmpty() && !adulthoodStory.NullOrEmpty())) {
					Log.Error("Backstory Error ("+bio.name+"): A locked backstory must include both a childhood story and an adulthood story.");
				}
				if (Controller.Settings.useLiteMode.Equals(true)) {
					if (!childhoodStory.NullOrEmpty() && !BackstoryDatabase.allBackstories.ContainsKey("REB_"+childhoodStory)) {
						Log.Error("Backstory Error ("+bio.name+"): Childhood backstory '"+childhoodStory+"' does not exist.");
						childhoodStory = "";
					}
					if (!adulthoodStory.NullOrEmpty() && !BackstoryDatabase.allBackstories.ContainsKey("REB_"+adulthoodStory)) {
						Log.Error("Backstory Error ("+bio.name+"): Adulthood backstory '"+adulthoodStory+"' does not exist.");
						adulthoodStory = "";
					}
					if (!childhoodStory.NullOrEmpty() && !adulthoodStory.NullOrEmpty()) {
						bio.childhood = BackstoryDatabase.allBackstories["REB_"+childhoodStory];
						bio.adulthood = BackstoryDatabase.allBackstories["REB_"+adulthoodStory];
						SolidBioDatabase.allBios.Add(bio);
						REB_Initializer.fullBioCount++;
					}
					else {
						PawnNameDatabaseSolid.AddPlayerContentName(bio.name, bio.gender);
						REB_Initializer.fullCount++;
					}
				}
				else {
					if (!childhoodStory.NullOrEmpty() && !REB_Initializer.REB_Backstories.ContainsKey("REB_"+childhoodStory)) {
						Log.Error("Backstory Error ("+bio.name+"): Childhood backstory '"+childhoodStory+"' does not exist.");
						childhoodStory = "";
					}
					if (!adulthoodStory.NullOrEmpty() && !REB_Initializer.REB_Backstories.ContainsKey("REB_"+adulthoodStory)) {
						Log.Error("Backstory Error ("+bio.name+"): Adulthood backstory '"+adulthoodStory+"' does not exist.");
						adulthoodStory = "";
					}
					if (!childhoodStory.NullOrEmpty() && !adulthoodStory.NullOrEmpty()) {
						bio.childhood = REB_Initializer.REB_Backstories["REB_"+childhoodStory];
						bio.adulthood = REB_Initializer.REB_Backstories["REB_"+adulthoodStory];
						SolidBioDatabase.allBios.Add(bio);
						REB_Initializer.fullBioCount++;
					}
					else {
						PawnNameDatabaseSolid.AddPlayerContentName(bio.name, bio.gender);
						REB_Initializer.fullCount++;
					}
				}
			}
		}
	}
		
}
