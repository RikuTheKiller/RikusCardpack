using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class PetrifyHandler : MonoBehaviour
    {
        private Player _p;
        private CharacterData _cd;
        private CharacterStatModifiers _cs;
        private StoneColor _stoneColor = null;
        private bool _ranOnce = false;
        private bool _isRunning = true;
        private bool _isAllowed = true;
        private bool _happen = true;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private float _t = 0.05f;
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
                _cd = _p.GetComponent<CharacterData>();
                _cs = _p.GetComponent<CharacterStatModifiers>();
                _happen = true;
            }
            if (_t > 0)
            {
                _t -= TimeHandler.deltaTime;
                if (_isRunning)
                {
                    _movementSpeedHolder = _p.data.stats.movementSpeed;
                    _jumpHolder = _p.data.stats.jump;
                    _stoneColor = _p.gameObject.AddComponent<StoneColor>();
                    _isRunning = false;
                }
                _cs.movementSpeed = 0;
                _cs.jump = 0;
                _cd.input.silencedInput = true;
                if (_t <= 0)
                {
                    _cd.input.silencedInput = false;
                    _cs.movementSpeed = _movementSpeedHolder;
                    _cs.jump = _jumpHolder;
                    _isAllowed = true;
                    _isRunning = true;
                    if (_stoneColor != null)
                    {
                        Destroy(_stoneColor);
                        _stoneColor = null;
                    }
                }
            }
            if (_stackCount < 1 && _isAllowed)
            {
                _p.data.healthHandler.reviveAction -= OnRevive;

                Destroy(this);
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
        private void OnRevive()
        {
            _t = 0.05f;
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
                _p.data.healthHandler.reviveAction -= OnRevive;

                Destroy(this);
            }
        }
    }
    public class StoneColor : ReversibleEffect //Totally not copied from HDC :D
    {
        private readonly Color color = new Color(0.3f, 0.3f, 0.3f, 1f); //Dark Gray
        private ReversibleColorEffect colorEffect = null;

        public override void OnOnEnable()
        {
            if (colorEffect != null)
            {
                colorEffect.Destroy();
            }
        }
        public override void OnStart()
        {
            colorEffect = base.player.gameObject.AddComponent<ReversibleColorEffect>();
            colorEffect.SetColor(this.color);
            colorEffect.SetLivesToEffect(1);
        }
        public override void OnOnDisable()
        {
            if (colorEffect != null)
            {
                colorEffect.Destroy();
            }
        }
        public override void OnOnDestroy()
        {
            if (colorEffect != null)
            {
                colorEffect.Destroy();
            }
        }
    }
}