using System;
using RikusCardpack.Extensions;
using RikusCardpack.MonoBehaviours;
using UnityEngine;
using HarmonyLib;

namespace RikusCardpack.Patches
{
    [HarmonyPatch(typeof(HealthHandler), "RPCA_Die")]
    class DiePatch
    {
        private static bool Prefix(HealthHandler __instance, Vector2 deathDirection)
        {
            CharacterData _cd = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();
            Player _p = _cd.player;
            DeterminationEffect _thisDeterminationEffect = _p.gameObject.GetComponent<DeterminationEffect>();
            if (_thisDeterminationEffect != null && !_cd.stats.GetAdditionalData().skipDiePatch)
            {
                _cd.stats.GetAdditionalData().skipDiePatch = true;
                _thisDeterminationEffect.OnDeath();
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
