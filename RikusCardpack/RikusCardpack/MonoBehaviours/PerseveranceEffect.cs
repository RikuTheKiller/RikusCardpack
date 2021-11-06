using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.Extensions;
using HarmonyLib;
using ModdingUtils.MonoBehaviours;
using UnboundLib.GameModes;

namespace RikusCardpack.MonoBehaviours
{
    class PerseveranceEffect : MonoBehaviour
    {
        public Player _p;
        private CharacterStatModifiers _cs;
        private PurpleColor _purpleColor = null;
        private bool _ranOnce = false;
        private bool _happen = true;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private const float _duration = 0.3f;
        private float _durationLeft = 0;
        private float _defaultCooldown = 0;
        private float _cooldownLeft = 0;
        private float _additionalCooldown = 0;
        public void RunAdder(Player p, CharacterStatModifiers cs)
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _p = p;
            _cs = cs;
            _ranOnce = true;

            _p.data.healthHandler.reviveAction += OnRevive;
            _p.data.stats.WasDealtDamageAction += OnDealtDamage;
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
            if (_cooldownLeft > 0)
            {
                _cooldownLeft -= TimeHandler.deltaTime;
            }
            if (_durationLeft > 0)
            {
                _durationLeft -= TimeHandler.deltaTime;
                if (_durationLeft <= 0)
                {
                    if (_purpleColor != null)
                    {
                        Destroy(_purpleColor);
                        _purpleColor = null;
                    }
                    _cooldownLeft = _additionalCooldown;
                }
            }
            if (_stackCount < 1 && _durationLeft <= 0)
            {
                _p.data.healthHandler.reviveAction -= OnRevive;
                _p.data.stats.WasDealtDamageAction -= OnDealtDamage;

                Destroy(this);
            }
            if (_purpleColor != null && _durationLeft <= 0)
            {
                Destroy(_purpleColor);
                _purpleColor = null;
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
            _additionalCooldown = 0;
            _durationLeft = 0.05f;
            _cooldownLeft = 0;
        }
        public void OnDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (_durationLeft > 0)
            {
                _p.data.healthHandler.Heal(damage.magnitude);
                if (_additionalCooldown < 1.4f)
                {
                    _additionalCooldown += 0.1f;
                }
            }
            else if (_cooldownLeft <= 0)
            {
                _durationLeft = _duration * _stackCount;
                _purpleColor = _p.gameObject.GetOrAddComponent<PurpleColor>();
                _additionalCooldown = _defaultCooldown;
            }
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1 && _durationLeft <= 0)
            {
                _p.data.healthHandler.reviveAction -= OnRevive;
                _p.data.stats.WasDealtDamageAction -= OnDealtDamage;

                Destroy(this);
            }
        }
    }
    public class PurpleColor : ReversibleEffect //Totally not copied from HDC :D
    {
        private readonly Color colorMax = new Color(0.7f, 0f, 0.7f, 1f); //Lighter Purple
        private readonly Color colorMin = new Color(0.3f, 0f, 0.3f, 1f); //Darker Purple
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
            colorEffect.SetColorMin(colorMin);
            colorEffect.SetColorMax(colorMax);
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
