using System.Reflection;
using ModdingUtils.RoundsEffects;
using System.Collections;
using UnboundLib;
using UnityEngine;
using UnboundLib.GameModes;

namespace RikusCardpack.HitEffects
{
    public class PetrifyingHitEffect : HitEffect
    {
        private bool _ranOnce = false;
        private bool _initialized = false;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private float _t = 1;
        public void RunAdder()
        {
            if (_ranOnce)
            {
                _stackCount += 1;
                _t += 1;

                return;
            }
            _ranOnce = true;
        }
        void Update()
        {
            if (!_initialized)
            {
                GameModeManager.AddHook(GameModeHooks.HookGameEnd, OnGameEnd);
                _initialized = true;
            }
            if (_forceDestroy)
            {
                if (_stackCount > 0)
                {
                    RunRemover();
                }
            }
        }
        static IEnumerator OnGameEnd(IGameModeHandler gm)
        {
            _forceDestroy = true;
            yield break;
        }
        public override void DealtDamage(
            Vector2 damage,
            bool selfDamage,
            Player damagedPlayer = null
        )
        {
            if (damagedPlayer == null)
            {
                return;
            }

            var thisPetrifyHandler = damagedPlayer.gameObject.GetOrAddComponent<MonoBehaviours.PetrifyHandler>();
            if (thisPetrifyHandler != null)
            {
                thisPetrifyHandler.PetrifyPlayer(_t);
            }
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                Destroy(this);
            }
        }
    }
}