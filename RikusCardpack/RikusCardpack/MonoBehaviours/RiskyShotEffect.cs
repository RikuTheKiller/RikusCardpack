using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using System.Windows.Input;
using ModdingUtils.Utils;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class RiskyShotEffect : MonoBehaviour
    {
        private const float _cooldown = 20;
        private float _cooldownLeft = 0;
        private const float _duration = 3;
        private float _durationLeft = 0;
        private const float _noRegenDuration = 10;
        private float _noRegenDurationLeft = 0;
        public float _stackingMultiplier = 0.9f;
        private HitEffects.InstaKillHitEffect _thisIKHE = null;
        private bool _ranOnce = false;
        private bool _isRunning = false;
        private bool _hasRevived = false;
        private bool _happen = true;
        private static bool _forceDestroy = false;
        private Block _b;
        private Gun _g;
        private GunAmmo _ga;
        private CharacterData _cd;
        private Player _p;
        public void RunAdder(Player p, Block b, CharacterData cd, Gun g, GunAmmo ga)
        {
            if (_ranOnce)
            {
                if (_stackingMultiplier > 0.55f)
                {
                    _stackingMultiplier -= 0.15f;
                }

                return;
            }
            _cd = cd;
            _b = b;
            _g = g;
            _p = p;
            _ga = ga;
            _ranOnce = true;

            _b.BlockAction += OnBlock;
            _p.data.healthHandler.reviveAction += OnRevive;
        }
        void Start()
        {
            _happen = false;
            _thisIKHE = _p.gameObject.GetOrAddComponent<HitEffects.InstaKillHitEffect>();
        }
        void Update()
        {
            if (!_happen)
            {
                GameModeManager.AddHook(GameModeHooks.HookGameEnd, OnGameEnd);
                _happen = true;
            }
            if (_cooldownLeft > 0)
            {
                _cooldownLeft -= TimeHandler.deltaTime;
            }
            if (_noRegenDurationLeft > 0)
            {
                _noRegenDurationLeft -= TimeHandler.deltaTime;
                if (_cd.health > 1.1f)
                {
                    Vector2 _damage = new Vector2(Mathf.Sqrt(_cd.health * _cd.health) / 2 - Mathf.Sqrt(2) / 2, Mathf.Sqrt(_cd.health * _cd.health) / 2 - Mathf.Sqrt(2) / 2);
                    Vector2 _position = new Vector2(_p.transform.position.x, _p.transform.position.y);
                    _cd.healthHandler.DoDamage(_damage, _position, Color.white, null, null, false, false, true);
                }
            }
            if (_durationLeft > 0)
            {
                _durationLeft -= TimeHandler.deltaTime;
                if (_thisIKHE._hasHit && _isRunning)
                {
                    _thisIKHE._damagedPlayer.data.maxHealth *= _stackingMultiplier;
                    _isRunning = false;
                }
                if (_durationLeft <= 0 && !_hasRevived)
                {
                    if (!_thisIKHE._hasHit)
                    {
                        _cd.health = 1;
                        _noRegenDurationLeft = _noRegenDuration;
                    }
                    _isRunning = false;
                }
                else if (_durationLeft <= 0)
                {
                    _isRunning = false;
                    _hasRevived = false;
                }
            }
            if (_forceDestroy)
            {
                if (_stackingMultiplier < 1)
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
        private void OnRevive()
        {
            if (_durationLeft > 0)
            {
                _hasRevived = true;
                _durationLeft = 0.05f;
            }
            _cooldownLeft = 0;
            _noRegenDurationLeft = 0;
        }
        private void OnBlock(BlockTrigger.BlockTriggerType obj)
        {
            if (_cooldownLeft <= 0 && _g.isReloading)
            {
                _cooldownLeft = _cooldown;
                _ga.ReloadAmmo();
                _thisIKHE._hasHit = false;
                _durationLeft = _duration;
                _isRunning = true;
            }
        }
        public void RunRemover()
        {
            _stackingMultiplier += 0.15f;
            if (_stackingMultiplier >= 1)
            {
                _b.BlockAction -= OnBlock;
                if (_thisIKHE != null)
                {
                    Destroy(_thisIKHE);
                }
                Destroy(this);
            }
        }
    }
}