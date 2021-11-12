using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class SniperLogicEffect : MonoBehaviour
    {
        private Gun _g;
        private GunAmmo _ga;
        private bool _ranOnce = false;
        private bool _happen = true;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private float _lastReloadTime = 0;
        private float _givenDamage = 0;
        private float _givenProjectileSpeed = 0;
        private float _reloadTime = 0;
        public void RunAdder(Gun g, GunAmmo ga)
        {
            if (_ranOnce)
            {
                _g.damage -= _givenDamage;
                _givenDamage = 0;
                _g.projectileSpeed -= _givenProjectileSpeed;
                _givenProjectileSpeed = 0;
                _lastReloadTime = 0;
                _stackCount += 1;

                return;
            }
            _g = g;
            _ga = ga;
            _ranOnce = true;
        }
        void Start()
        {
            _happen = false;
        }
        void Update()
        {
            if (!_happen)
            {
                GameModeManager.AddHook(GameModeHooks.HookGameEnd, OnGameEnd);
                _happen = true;
            }
            if (_lastReloadTime != (_ga.reloadTime + _ga.reloadTimeAdd) * _ga.reloadTimeMultiplier)
            {
                _reloadTime = (_ga.reloadTime + _ga.reloadTimeAdd) * _ga.reloadTimeMultiplier;
                _g.bulletDamageMultiplier += (_reloadTime - _lastReloadTime) * 15f * _stackCount / 55 / _g.damage;
                _givenDamage += _g.bulletDamageMultiplier - (_g.bulletDamageMultiplier - _givenDamage);
                if (_givenDamage < 0)
                {
                    _g.damage -= _givenDamage;
                    _givenDamage = 0;
                }
                _g.projectileSpeed += (_reloadTime - _lastReloadTime) * _stackCount * 0.333f;
                _givenProjectileSpeed += _g.projectileSpeed - (_g.projectileSpeed - _givenProjectileSpeed);
                if (_givenProjectileSpeed < 0)
                {
                    _g.projectileSpeed -= _givenProjectileSpeed;
                    _givenProjectileSpeed = 0;
                }
                _lastReloadTime = _reloadTime;
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
        public void RunRemover()
        {
            _g.damage -= _givenDamage;
            _givenDamage = 0;
            _g.projectileSpeed -= _givenProjectileSpeed;
            _givenProjectileSpeed = 0;
            _lastReloadTime = 0;
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                Destroy(this);
            }
        }
    }
}