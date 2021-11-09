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
        private PerseveranceEffect _thisPerseveranceEffect = null;
        private bool _ranOnce = false;
        private bool _happen = true;
        private bool _isRunning = true;
        private bool _skip = false;
        private bool _statsAdded = false;
        private bool _skipDealt = false;
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
                    AddStats();
                    _isRunning = false;
                }
                if (_durationLeft <= 0 && !_skip)
                {
                    ReverseStats();
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
            if (!_statsAdded && _durationLeft > 0 && !_skip)
            {
                _durationLeft = 0.05f;
                _skip = true;
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
            _thisPerseveranceEffect = _p.gameObject.GetComponent<PerseveranceEffect>();
            if (_thisPerseveranceEffect != null && _thisPerseveranceEffect._durationLeft > 0 && !_thisPerseveranceEffect._skipDetermination)
            {
                _skipDealt = true;
            }
            if (_cd.health <= 0 && _durationLeft <= 0 && _cs.remainingRespawns <= 0 && !_skipDealt)
            {
                _cd.healthHandler.Heal(_cd.health * -1 + _cd.maxHealth);
                _redColor = _p.gameObject.GetOrAddComponent<RedColor>();
                _durationLeft = _duration + (_stackCount - 1) * _durationOnStack;
            }
            else if (_durationLeft > 0)
            {
                _cd.healthHandler.Heal(damage.magnitude * 0.5f);
            }
            if (_thisPerseveranceEffect != null && _thisPerseveranceEffect._durationLeft > 0 && _thisPerseveranceEffect._skipDetermination)
            {
                _thisPerseveranceEffect._skipDetermination = false;
            }
            _thisPerseveranceEffect = null;
            _skipDealt = false;
        }
        private void OnRevive()
        {
            ReverseStats(true);
        }
        private void AddStats()
        {
            if (!_statsAdded)
            {
                if (_b.counter < _b.Cooldown())
                {
                    _b.ResetCD(true);
                }
                _givenSpeed = _cs.movementSpeed;
                _givenJump = _cs.jump;
                _givenDamage = _g.damage;
                _givenProjectileSpeed = _g.projectileSpeed;
                _givenBlockSpeed = _b.cdMultiplier;
                _givenAttackSpeed = _g.attackSpeedMultiplier;
                _givenReloadSpeed = _ga.reloadTimeMultiplier;
                _cs.movementSpeed *= 2f;
                _cs.jump *= 1.5f;
                _g.bulletDamageMultiplier *= 1.5f;
                _g.projectileSpeed *= 1.5f;
                _b.cdMultiplier *= 0.5f;
                _g.attackSpeedMultiplier *= 1.3f;
                _ga.reloadTimeMultiplier *= 0.5f;
                _givenSpeed = (_givenSpeed - _cs.movementSpeed) * -1;
                _givenJump = (_givenJump - _cs.jump) * -1;
                _givenDamage = (_givenDamage - _g.bulletDamageMultiplier) * -1;
                _givenProjectileSpeed = (_givenProjectileSpeed - _g.projectileSpeed) * -1;
                _givenBlockSpeed = (_givenBlockSpeed - _b.cdMultiplier) * -1;
                _givenAttackSpeed = (_givenAttackSpeed - _g.attackSpeedMultiplier) * -1;
                _givenReloadSpeed = (_givenReloadSpeed - _ga.reloadTimeMultiplier) * -1;
                _statsAdded = true;
            }
        }
        private void ReverseStats(bool skip = false)
        {
            if (!_skip && _statsAdded)
            {
                _cs.movementSpeed -= _givenSpeed;
                _cs.jump -= _givenJump;
                _g.bulletDamageMultiplier -= _givenDamage;
                _g.projectileSpeed -= _givenProjectileSpeed;
                _b.cdMultiplier -= _givenBlockSpeed;
                _g.attackSpeedMultiplier -= _givenAttackSpeed;
                _ga.reloadTimeMultiplier -= _givenReloadSpeed;
                if (_redColor != null)
                {
                    Destroy(_redColor);
                    _redColor = null;
                }
                _statsAdded = false;
            }
            else if (_statsAdded)
            {
                _cs.movementSpeed -= _givenSpeed;
                _cs.jump -= _givenJump;
                _g.bulletDamageMultiplier -= _givenDamage;
                _g.projectileSpeed -= _givenProjectileSpeed;
                _b.cdMultiplier -= _givenBlockSpeed;
                _g.attackSpeedMultiplier -= _givenAttackSpeed;
                _ga.reloadTimeMultiplier -= _givenReloadSpeed;
                if (_redColor != null)
                {
                    Destroy(_redColor);
                    _redColor = null;
                }
                _durationLeft = 0.05f;
                _skip = true;
                _statsAdded = false;
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
                _cs.WasDealtDamageAction -= OnDealtDamage;
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