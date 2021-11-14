using System;
using System.Runtime.CompilerServices;
using HarmonyLib;


namespace RikusCardpack.Extensions
{
    // 100% not a copy of PCE's code. Totally.
    [Serializable]
    public class CharacterStatModifiersAdditionalData
    {
        public bool skipDiePatch;
        public bool inDetermination;
        public CharacterStatModifiersAdditionalData()
        {
            skipDiePatch = false;
            inDetermination = false;
        }
    }
    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData> data =
            new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData>();

        public static CharacterStatModifiersAdditionalData GetAdditionalData(this CharacterStatModifiers cs)
        {
            return data.GetOrCreateValue(cs);
        }

        public static void AddData(this CharacterStatModifiers cs, CharacterStatModifiersAdditionalData value)
        {
            try
            {
                data.Add(cs, value);
            }
            catch (Exception) { }
        }

    }
    // Resets additional stat modifiers when "ResetStats" is called.
    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetAdditionalData().skipDiePatch = false;
            __instance.GetAdditionalData().inDetermination = false;
        }
    }
}