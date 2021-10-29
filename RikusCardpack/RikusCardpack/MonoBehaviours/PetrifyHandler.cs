using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using ModdingUtils.Utils;

namespace RikusCardpack.MonoBehaviours
{
    class PetrifyHandler : MonoBehaviour
    {
        private Player _p;
        private bool _ranOnce = false;
        private bool _isRunning = true;
        private bool _isAllowed = true;
        private int _stackCount = 1;
        private float _t = 0;
        private float _movementSpeedHolder = 0;
        private float _jumpHolder = 0;
        public void RunAdder(Player p)
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _p = p;
            _ranOnce = true;
        }
        void Update()
        {
            if (_t > 0)
            {
                if (PlayerStatus.PlayerAliveAndSimulated(_p))
                {
                    _t -= TimeHandler.deltaTime;
                }
                else
                {
                    _t = 0;
                }
                _p.data.isSilenced = true;
                _p.data.silenceTime = 1;
                if (_isRunning)
                {
                    _movementSpeedHolder = _p.data.stats.movementSpeed;
                    _jumpHolder = _p.data.stats.jump;
                    _isRunning = false;
                }
                _p.data.stats.movementSpeed = 0;
                _p.data.stats.jump = 0;
                if (_t <= 0)
                {
                    _p.data.silenceTime = 0;
                    _p.data.stats.movementSpeed = _movementSpeedHolder;
                    _p.data.stats.jump = _jumpHolder;
                    if (_stackCount < 1)
                    {
                        Destroy(this);
                    }
                    _isRunning = true;
                    _isAllowed = true;
                }
            }
        }
        public void PetrifyPlayer(float t)
        {
            _t = t;
            _isAllowed = false;
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1 && _isAllowed)
            {
                Destroy(this);
            }
        }
    }
}