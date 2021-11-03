using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.Utils;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class BadMathEffect : MonoBehaviour
    {
        private Player _p;
        private Gun _g;
        private GunAmmo _ga;
        private bool _ranOnce;
        private bool _happen = true;
        private static bool _disable = true;
        private int _timesGet = 1;
        private int _givenAmmo = 0;
        public void RunAdder(Player p, Gun g, GunAmmo ga)
        {
            if (_ranOnce)
            {
                _timesGet += 1;

                return;
            }
            _p = p;
            _g = g;
            _ga = ga;

            _p.data.stats.OnReloadDoneAction += OnReload;
            _ranOnce = true;
        }
        void Start()
        {
            _happen = false;
            if (GM_Test.instance != null && GM_Test.instance.gameObject.activeInHierarchy)
            {
                _disable = false;
            }
        }
        void Update()
        {
            if (!_happen)
            {
                GameModeManager.AddHook(GameModeHooks.HookPointStart, OnPointStart);
                GameModeManager.AddHook(GameModeHooks.HookPointEnd, OnPointEnd);
                _happen = true;
            }
        }
        public void OnReload(int obj)
        {
            if (!_disable)
            {
                _ga.maxAmmo += _timesGet;
                _g.ammo += _timesGet;
                _givenAmmo += _timesGet;
            }
        }
        static IEnumerator OnPointStart(IGameModeHandler gm)
        {
            _disable = false;
            yield break;
        }
        static IEnumerator OnPointEnd(IGameModeHandler gm)
        {
            _disable = true;
            yield break;
        }
        public void RunRemover()
        {
            _ga.maxAmmo -= _givenAmmo / _timesGet;
            _timesGet -= 1;

            if (_timesGet < 1)
            {
                _p.data.stats.OnReloadDoneAction -= OnReload;

                Destroy(this);
            }
        }
    }
}
