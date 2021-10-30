using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using System.Windows.Input;
using ModdingUtils.Utils;

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
        private Block _b;
        private Gun _g;
        private GunAmmo _ga;
        private CharacterData _cd;
        private Player _p;
        public void RunAdder(Player p, Block b, CharacterData cd, Gun g, GunAmmo ga)
        {
            if (_ranOnce)
            {
                if (_stackingMultiplier > 0.7f)
                {
                    _stackingMultiplier -= 0.1f;
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
            _thisIKHE = _p.gameObject.GetOrAddComponent<HitEffects.InstaKillHitEffect>();
        }
        void Update()
        {
            if (_cooldownLeft > 0)
            {
                _cooldownLeft -= TimeHandler.deltaTime;
                _durationLeft = 0;
                _cooldownLeft = 0;
                _noRegenDurationLeft = 0;
            }
            if (_noRegenDurationLeft > 0)
            {
                _noRegenDurationLeft -= TimeHandler.deltaTime;
                if (_cd.health > 1)
                {
                    _cd.health = 1;
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
                }
                if (_durationLeft <= 0)
                {
                    _isRunning = false;
                }
            }
        }
        private void OnRevive()
        {
            _hasRevived = true;
            _durationLeft = 0.05f;
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
            _stackingMultiplier += 0.1f;
            if (_stackingMultiplier > 0.9f)
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