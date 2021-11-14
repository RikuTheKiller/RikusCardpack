using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.GameModes;
using ModdingUtils.MonoBehaviours;
using HarmonyLib;
using RikusCardpack.Extensions;

namespace RikusCardpack.MonoBehaviours
{
    class DeterminationEffect : MonoBehaviour
    {
        private Player _p;
        private GunAmmo _ga;
        private CharacterStatModifiers _cs;
        private Gun _g;
        private Block _b;
        private CharacterData _cd;
        private RedColor _redColor = null;
        private bool _ranOnce = false;
        private bool _initialized = false;
        private bool _isRunning = true;
        private bool _skip = false;
        private bool _statsAdded = false;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private const float _duration = 4f;
        private const float _durationOnStack = 2f;
        private float _durationLeft = 0;
        private float _givenSpeed = 0;
        private float _givenJump = 0;
        private float _givenDamage = 0;
        private float _givenProjectileSpeed = 0;
        private float _givenBlockSpeed = 0;
        private float _givenAttackSpeed = 0;
        private float _givenReloadSpeed = 0;
        public void RunAdder(Player p, GunAmmo ga, CharacterStatModifiers cs, Gun g, Block b, CharacterData cd)
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _p = p;
            _ga = ga;
            _cs = cs;
            _g = g;
            _b = b;
            _cd = cd;
            _ranOnce = true;

            _p.data.stats.WasDealtDamageAction += OnDealtDamage;
            _p.data.healthHandler.reviveAction += OnRevive;
        }
        void Update()
        {
            if (!_initialized)
            {
                GameModeManager.AddHook(GameModeHooks.HookGameEnd, OnGameEnd);
                _initialized = true;
            }
            if (_durationLeft > 0)
            {
                _durationLeft -= TimeHandler.deltaTime;
                if (_isRunning)
                {
                    AddStats();
                    _isRunning = false;
                }
                if (_durationLeft <= 0 && !_skip)
                {
                    ReverseStats();
                    _cs.GetAdditionalData().skipDiePatch = true;
                    Unbound.Instance.ExecuteAfterSeconds(0f, () =>
                    {
                        typeof(HealthHandler).InvokeMember(
                            "RPCA_Die",
                            BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                            null,
                            _cd.healthHandler,
                            new object[] { new Vector2(0, 1) }
                        );
                    });
                    _isRunning = true;
                }
                else if (_durationLeft <= 0)
                {
                    _isRunning = true;
                    _skip = false;
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
        private void OnDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (_durationLeft > 0)
            {
                _cd.healthHandler.Heal(damage.magnitude * 0.5f);
            }
        }
        public void OnDeath()
        {
            _cd.healthHandler.Heal(_cd.maxHealth + _cd.health * -1);
            _redColor = _p.gameObject.GetOrAddComponent<RedColor>();
            _durationLeft = _duration + (_stackCount - 1) * _durationOnStack;
        }
        private void OnRevive()
        {
            ReverseStats(true);
            _cs.GetAdditionalData().inDetermination = false;
            _cs.GetAdditionalData().skipDiePatch = false;
        }
        private void AddStats()
        {
            if (!_statsAdded)
            {
                if (_b.counter < _b.Cooldown())
                {
                    _b.ResetCD(true);
                }
                _cs.movementSpeed *= 2f;
                _cs.jump *= 1.5f;
                _g.bulletDamageMultiplier *= 1.5f;
                _g.projectileSpeed *= 1.5f;
                _b.cdMultiplier *= 0.5f;
                _g.attackSpeedMultiplier *= 1.3f;
                _ga.reloadTimeMultiplier *= 0.5f;
                _givenSpeed += _cs.movementSpeed - _cs.movementSpeed / 2f;
                _givenJump += _cs.jump - _cs.jump / 1.5f;
                _givenDamage += _g.bulletDamageMultiplier - _g.bulletDamageMultiplier / 1.5f;
                _givenProjectileSpeed += _g.projectileSpeed - _g.projectileSpeed / 1.5f;
                _givenBlockSpeed += _b.cdMultiplier - _b.cdMultiplier / 0.5f;
                _givenAttackSpeed += _g.attackSpeedMultiplier - _g.attackSpeedMultiplier / 1.3f;
                _givenReloadSpeed += _ga.reloadTimeMultiplier - _ga.reloadTimeMultiplier / 0.5f;
                _statsAdded = true;
            }
        }
        private void ReverseStats(bool skip = false)
        {
            if (_statsAdded)
            {
                _cs.movementSpeed -= _givenSpeed;
                _cs.jump -= _givenJump;
                _g.bulletDamageMultiplier -= _givenDamage;
                _g.projectileSpeed -= _givenProjectileSpeed;
                _b.cdMultiplier -= _givenBlockSpeed;
                _g.attackSpeedMultiplier -= _givenAttackSpeed;
                _ga.reloadTimeMultiplier -= _givenReloadSpeed;
                _givenSpeed = 0;
                _givenJump = 0;
                _givenDamage = 0;
                _givenProjectileSpeed = 0;
                _givenBlockSpeed = 0;
                _givenAttackSpeed = 0;
                _givenReloadSpeed = 0;
                if (_redColor != null)
                {
                    Destroy(_redColor);
                    _redColor = null;
                }
                _statsAdded = false;
                if (skip)
                {
                    _durationLeft = 0.05f;
                    _skip = true;
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
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                ReverseStats(true);
                _cd.healthHandler.reviveAction -= OnRevive;

                Destroy(this);
            }
        }
    }
    public class RedColor : ReversibleEffect //Totally not copied from HDC :D
    {
        private readonly Color colorMax = new Color(0.6f, 0f, 0f, 1f); //Lighter Red
        private readonly Color colorMin = new Color(0.3f, 0f, 0f, 1f); //Darker Red
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