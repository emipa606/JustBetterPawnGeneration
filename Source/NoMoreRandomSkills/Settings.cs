using Verse;

namespace NoMoreRandomSkills
{
    /// <summary>
    ///     Definition of the settings for the mod
    /// </summary>
    internal class NoMoreRandomSkillsSettings : ModSettings
    {
        public int MinimumSpawnAge = 20;
        public bool OnlyCustomBackstories = true;

        /// <summary>
        ///     Saving and loading the values
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref OnlyCustomBackstories, "OnlyCustomBackstories", true);
            Scribe_Values.Look(ref MinimumSpawnAge, "MinimumSpawnAge", 20);
        }
    }
}