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
        private bool _ranOnce = false;
        private bool _happen = true;
        private static bool _forceDestroy = false;
        private Gun _g;
        private float _checkCooldownLeft = 0;
        private float _lastDamageAdd = 0;
        private float _damageToAdd = 0;
        private float _damageToAddHolder = 0;
        private float _givenDamage = 0;
        private float _givenDamageHolder = 0;
        private float _lastPSpeedAdd = 0;
        private float _pSpeedToAdd = 0;
        private float _pSpeedToAddHolder = 0;
        private float _givenPSpeed = 0;
        private float _givenPSpeedHolder = 0;
        private const float _damageDivider = 6;
        private const float _pSpeedDivider = 4;
        private float _stackMultiplier = 1;
        private const float _checkCooldown = 0.2f;
        public void RunAdder(Gun g)
        {
            if (_ranOnce)
            {
                _stackMultiplier += 1;

                return;
            }
            _g = g;
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
            if (_checkCooldownLeft > 0)
            {
                _checkCooldownLeft -= TimeHandler.deltaTime;
            }
            else
            {
                _checkCooldownLeft = _checkCooldown;
                _damageToAdd = _g.damage / _damageDivider * _g.reloadTime;
                _damageToAdd *= _stackMultiplier;
                _damageToAddHolder = _damageToAdd;
                _damageToAdd -= _lastDamageAdd;
                _lastDamageAdd = _damageToAddHolder;
                _g.damage += _damageToAdd;
                _givenDamageHolder += _damageToAddHolder;
                _givenDamage += _damageToAdd;
                if (_givenDamage < 0)
                {
                    _g.damage -= _givenDamage;
                    _lastDamageAdd -= _givenDamageHolder;
                    _givenDamage = 0;
                    _givenDamageHolder = 0;
                }
                _pSpeedToAdd = _g.projectileSpeed / _pSpeedDivider * _g.reloadTime;
                _pSpeedToAdd *= _stackMultiplier;
                _pSpeedToAddHolder = _pSpeedToAdd;
                _pSpeedToAdd -= _lastPSpeedAdd;
                _lastPSpeedAdd = _pSpeedToAddHolder;
                _g.projectileSpeed += _pSpeedToAdd;
                _givenPSpeedHolder += _pSpeedToAddHolder;
                _givenPSpeed += _pSpeedToAdd;
                if (_givenPSpeed < 0)
                {
                    _g.projectileSpeed -= _givenPSpeed;
                    _lastPSpeedAdd -= _givenPSpeedHolder;
                    _givenPSpeed = 0;
                    _givenPSpeedHolder = 0;
                }
            }
            if (_forceDestroy)
            {
                if (_stackMultiplier > 0)
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
            _stackMultiplier -= 1;
            if (_stackMultiplier < 1)
            {
                _g.damage -= _givenDamage;
                _g.projectileSpeed -= _givenPSpeed;

                Destroy(this);
            }
        }
    }
}