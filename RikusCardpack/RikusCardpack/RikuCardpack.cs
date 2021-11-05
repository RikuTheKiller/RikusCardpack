using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RikusCardpack.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace RikusCardpack
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class RikusCardpack : BaseUnityPlugin
    {
        private const string ModId = "com.RikuTheKiller.RikusCardpack";
        private const string ModName = "RikusCardpack";
        public const string Version = "1.3.4"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "RC";
        public static RikusCardpack instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<BadMath>();
            CustomCard.BuildCard<RiskyShot>();
            CustomCard.BuildCard<SniperLogic>();
            CustomCard.BuildCard<Autoloader>();
            CustomCard.BuildCard<PetrifyingShots>();
            CustomCard.BuildCard<Perseverance>();
        }
    }
}
