using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class AutoloaderEffect : MonoBehaviour
    {
        private GunAmmo _ga;
        private Player _p;
        private bool _ranOnce = false;
        private bool _initialized = false;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        public void RunAdder(GunAmmo ga, Player p)
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _ga = ga;
            _p = p;
            _ranOnce = true;

            _p.data.stats.OutOfAmmpAction += OnReload;
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
        private void OnReload(int obj)
        {
            _ga.ReloadAmmo();
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                _p.data.stats.OutOfAmmpAction -= OnReload;

                Destroy(this);
            }
        }
    }
}
