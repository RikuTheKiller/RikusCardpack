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
        private RedColor _redColor = null;
        private bool _ranOnce = false;
        private bool _happen = true;
        private bool _isRunning = true;
        private bool _skip = false;
        private bool _skipRevive = false;
        private static bool _forceDestroy = false;
        private int _stackCount = 1;
        private const float _duration = 5;
        private float _durationLeft = 0;
        private float _givenSpeed = 0;
        private float _givenJump = 0;
        private float _givenDamage = 0;
        private float _givenProjectileSpeed = 0;
        public void RunAdder(Player p, GunAmmo ga)
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _p = p;
            _ga = ga;
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
                            _p.data.healthHandler,
                            new object[] { new Vector2(0, 1) }
                        );
                    });
                    _skipRevive = true;
                    _isRunning = true;
                }
                else if (_durationLeft <= 0)
                {
                    _isRunning = true;
                    _skip = false;
                }
            }
            if (_durationLeft > 0 && _p.data.weaponHandler.gun.ammo < _ga.maxAmmo)
            {
                if (_p.data.weaponHandler.gun.ammo > 0)
                {
                    _p.data.weaponHandler.gun.ammo = _ga.maxAmmo;
                }
                else
                {
                    _ga.ReloadAmmo(false);
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
            if (_p.data.health - damage.magnitude <= 0 && _durationLeft <= 0 && _p.data.stats.remainingRespawns <= 0)
            {
                _p.data.healthHandler.Heal(_p.data.health - damage.magnitude + _p.data.maxHealth);
                _redColor = _p.gameObject.GetOrAddComponent<RedColor>();
                _durationLeft = _duration * _stackCount;
            }
            if (_durationLeft > 0)
            {
                _p.data.healthHandler.Heal(damage.magnitude * 0.5f);
            }
        }
        private void OnRevive()
        {
            if (!_skipRevive && _durationLeft > 0)
            {
                ReverseStats(true);
            }
            _skipRevive = false;
        }
        private void AddStats()
        {
            _givenSpeed = _p.data.stats.movementSpeed;
            _givenJump = _p.data.stats.jump;
            _givenDamage = _p.data.weaponHandler.gun.damage;
            _givenProjectileSpeed = _p.data.weaponHandler.gun.projectileSpeed;
            _p.data.stats.movementSpeed *= 2f + (_stackCount - 1) * 1f;
            _p.data.stats.jump *= 1.5f + (_stackCount - 1) * 0.5f;
            _p.data.weaponHandler.gun.damage *= 1.5f + (_stackCount - 1) * 0.5f;
            _p.data.weaponHandler.gun.projectileSpeed *= 1.5f + (_stackCount - 1) * 0.5f;
            _givenSpeed = Mathf.Abs(_givenSpeed - _p.data.stats.movementSpeed);
            _givenJump = Mathf.Abs(_givenJump - _p.data.stats.jump);
            _givenDamage = Mathf.Abs(_givenDamage - _p.data.weaponHandler.gun.damage);
            _givenProjectileSpeed = Mathf.Abs(_givenProjectileSpeed - _p.data.weaponHandler.gun.projectileSpeed);
        }
        private void ReverseStats(bool skip = false)
        {
            if (!_skip)
            {
                _p.data.stats.movementSpeed -= _givenSpeed;
                _p.data.stats.jump -= _givenJump;
                _p.data.weaponHandler.gun.damage -= _givenDamage;
                _p.data.weaponHandler.gun.projectileSpeed -= _givenProjectileSpeed;
                if (_redColor != null)
                {
                    Destroy(_redColor);
                    _redColor = null;
                }
            }
            else
            {
                if (_durationLeft > 0)
                {
                    _p.data.stats.movementSpeed -= _givenSpeed;
                    _p.data.stats.jump -= _givenJump;
                    _p.data.weaponHandler.gun.damage -= _givenDamage;
                    _p.data.weaponHandler.gun.projectileSpeed -= _givenProjectileSpeed;
                    if (_redColor != null)
                    {
                        Destroy(_redColor);
                        _redColor = null;
                    }
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
                _p.data.stats.WasDealtDamageAction -= OnDealtDamage;
                _p.data.healthHandler.reviveAction -= OnRevive;

                Destroy(this);
            }
        }
    }
    public class RedColor : ReversibleEffect //Totally not copied from HDC :D
    {
        private readonly Color colorMax = new Color(0.9f, 0f, 0f, 1f); //Lighter Red
        private readonly Color colorMin = new Color(0.5f, 0f, 0f, 1f); //Darker Red
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