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
    class SugarRushEffect : MonoBehaviour
    {
        private Player _p;
        private bool _ranOnce = false;
        private bool _happen = true;
        private bool _isRunning = true;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private const float _duration = 1;
        private float _durationLeft = 0;
        public void RunAdder(Player p)
        {
            if (_ranOnce)
            {
                if (_durationLeft > 0)
                {
                    _p.data.stats.movementSpeed /= 1.5f + _stackCount / 2;
                    _p.data.stats.jump /= 1.25f + _stackCount / 4;
                    _durationLeft = 0;
                }
                _stackCount += 1;

                return;
            }
            _p = p;
            _ranOnce = true;

            _p.data.block.BlockAction += OnBlock;
            _p.data.healthHandler.reviveAction += OnRevive;
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
            if (_durationLeft > 0)
            {
                _durationLeft -= TimeHandler.deltaTime;
                if (_isRunning)
                {
                    _p.data.stats.movementSpeed *= 1.5f + _stackCount / 2;
                    _p.data.stats.jump *= 1.25f + _stackCount / 4;
                    _isRunning = false;
                }
                if (_durationLeft <= 0)
                {
                    _p.data.stats.movementSpeed /= 1.5f + _stackCount / 2;
                    _p.data.stats.jump /= 1.25f + _stackCount / 4;
                    _isRunning = true;
                }
            }
            if (_forceDestroy)
            {
                if (_stackCount > 0)
                {
                    RunRemover();
                }
            }
        }
        private void OnBlock(BlockTrigger.BlockTriggerType obj)
        {
            _durationLeft = _duration;
        }
        private void OnRevive()
        {
            if (_durationLeft > 0)
            {
                _durationLeft = 0.05f;
            }
        }
        static IEnumerator OnGameEnd(IGameModeHandler gm)
        {
            _forceDestroy = true;
            yield break;
        }
        public void RunRemover()
        {
            if (_durationLeft > 0)
            {
                _p.data.stats.movementSpeed /= 1.5f + _stackCount / 2;
                _p.data.stats.jump /= 1.25f + _stackCount / 4;
                _durationLeft = 0;
            }
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                _p.data.block.BlockAction -= OnBlock;

                Destroy(this);
            }
        }
    }
}