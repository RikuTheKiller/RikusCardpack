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
        private bool _alreadyRan;
        private bool _disable = false;
        private int _timesGet = 0;
        private int _givenAmmo = 0;
        public void RunAdder(Player p, Gun g, GunAmmo ga)
        {
            if (_alreadyRan)
            {
                _timesGet += 1;

                return;
            }
            _p = p;
            _g = g;
            _ga = ga;

            _timesGet += 1;

            _p.data.stats.OnReloadDoneAction += OnReload;
            _alreadyRan = true;
        }
        void Start()
        {
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, OnPickStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, OnPickEnd);
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
        private IEnumerator OnPickStart(IGameModeHandler gm)
        {
            _disable = true;
            yield break;
        }
        private IEnumerator OnPickEnd(IGameModeHandler gm)
        {
            _disable = false;
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
