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
            if (_thisDeterminationEffect != null && !_cd.stats.GetAdditionalData().inDetermination)
            {
                _cd.stats.GetAdditionalData().inDetermination = true;
                _thisDeterminationEffect.OnDeath();
                return false;
            }
            else if (_thisDeterminationEffect != null && _cd.health > 0 && !_cd.stats.GetAdditionalData().skipDiePatch)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
