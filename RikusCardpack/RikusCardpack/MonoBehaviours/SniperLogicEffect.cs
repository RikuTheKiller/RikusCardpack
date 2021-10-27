using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RikusCardpack.MonoBehaviours
{
    class SniperLogicEffect : MonoBehaviour
    {
        private bool _ranOnce = false;
        private Gun _g;
        private float _checkCooldownLeft = 0;
        private float _lastDamageAdd = 0;
        private float _damageToAdd = 0;
        private float _damageToAddHolder = 0;
        private float _givenDamage = 0;
        private float _lastPSpeedAdd = 0;
        private float _pSpeedToAdd = 0;
        private float _pSpeedToAddHolder = 0;
        private float _givenPSpeed = 0;
        private float _stackDivider = 8;
        private const float _checkCooldown = 0.5f;
        public void RunAdder(Gun g)
        {
            if (_ranOnce)
            {
                _stackDivider *= 0.85f;

                return;
            }

            _g = g;
            _ranOnce = true;
        }
        void Update()
        {
            if (_checkCooldownLeft > 0)
            {
                _checkCooldownLeft -= Time.deltaTime;
            }
            else
            {
                _checkCooldownLeft = _checkCooldown;
                _damageToAdd = _g.damage / _stackDivider * _g.reloadTime;
                _damageToAddHolder = _damageToAdd;
                _damageToAdd -= _lastDamageAdd;
                _lastDamageAdd = _damageToAddHolder;
                _g.damage += _damageToAdd;
                _givenDamage += _damageToAdd;
                if (_givenDamage < 0)
                {
                    _g.damage -= _givenDamage;
                    _lastDamageAdd -= _givenDamage;
                    _givenDamage = 0;
                }
                _pSpeedToAdd = _g.projectileSpeed / _stackDivider * _g.reloadTime;
                _pSpeedToAddHolder = _pSpeedToAdd;
                _pSpeedToAdd -= _lastPSpeedAdd;
                _lastPSpeedAdd = _pSpeedToAddHolder;
                _g.projectileSpeed += _pSpeedToAdd;
                _givenPSpeed += _pSpeedToAdd;
                if (_givenPSpeed < 0)
                {
                    _g.projectileSpeed -= _givenPSpeed;
                    _lastPSpeedAdd -= _givenPSpeed;
                    _givenPSpeed = 0;
                }
            }
        }
        public void RunRemover()
        {
            _g.damage -= _givenDamage;
            _g.projectileSpeed -= _givenPSpeed;

            Destroy(this);

            return;
        }
    }
}