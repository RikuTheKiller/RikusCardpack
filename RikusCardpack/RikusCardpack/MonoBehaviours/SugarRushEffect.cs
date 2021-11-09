﻿using System;
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
        private CharacterStatModifiers _cs;
        private bool _ranOnce = false;
        private bool _happen = true;
        private bool _isRunning = true;
        private bool _skip = false;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private const float _duration = 1.5f;
        private float _durationLeft = 0;
        private float _givenSpeed = 0;
        private float _givenJump = 0;
        public void RunAdder(Player p, CharacterStatModifiers cs)
        {
            if (_ranOnce)
            {
                if (_durationLeft > 0)
                {
                    _cs.movementSpeed -= _givenSpeed;
                    _cs.jump -= _givenJump;
                    _durationLeft = 0.05f;
                    _skip = true;
                }
                _stackCount += 1;

                return;
            }
            _p = p;
            _cs = cs;
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
                    _givenSpeed = _cs.movementSpeed;
                    _givenJump = _cs.jump;
                    _cs.movementSpeed *= 1.7f + (_stackCount - 1) * 0.7f;
                    _cs.jump *= 1.3f + (_stackCount - 1) * 0.3f;
                    _givenSpeed = (_givenSpeed - _cs.movementSpeed) * -1;
                    _givenJump = (_givenJump - _cs.jump) * -1;
                    _isRunning = false;
                }
                if (_durationLeft <= 0 && !_skip)
                {
                    _cs.movementSpeed -= _givenSpeed;
                    _cs.jump -= _givenJump;
                    _isRunning = true;
                }
                else if (_durationLeft <= 0)
                {
                    _skip = false;
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
                _cs.movementSpeed -= _givenSpeed;
                _cs.jump -= _givenJump;
                _durationLeft = 0.05f;
                _skip = true;
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
                _cs.movementSpeed -= _givenSpeed;
                _cs.jump -= _givenJump;
                _durationLeft = 0.05f;
                _skip = true;
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