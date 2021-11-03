using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.Extensions;
using HarmonyLib;
using ModdingUtils.MonoBehaviours;

namespace RikusCardpack.MonoBehaviours
{
    class PerseveranceEffect : MonoBehaviour
    {
        public Player _p;
        private CharacterStatModifiers _cs;
        private PurpleColor _purpleColor = null;
        private bool _ranOnce = false;
        private bool _isInvincible = false;
        private int _stackCount = 1;
        private const float _duration = 0.2f;
        private float _durationLeft = 0;
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
        void Update()
        {
            if (_durationLeft > 0)
            {
                _durationLeft -= TimeHandler.deltaTime;
                if (_durationLeft <= 0)
                {
                    _isInvincible = false;
                    if (_purpleColor != null)
                    {
                        Destroy(_purpleColor);
                        _purpleColor = null;
                    }
                }
            }
            if (_stackCount < 1 && !_isInvincible)
            {
                _p.data.healthHandler.reviveAction -= OnRevive;
                _p.data.stats.WasDealtDamageAction -= OnDealtDamage;

                Destroy(this);
            }
        }
        private void OnRevive()
        {
            _durationLeft = 0.05f;
        }
        public void OnDealtDamage(Vector2 damage, bool selfDamage)
        {
            _purpleColor = _p.gameObject.GetOrAddComponent<PurpleColor>();
            if (!_isInvincible)
            {
                _durationLeft = _duration * _stackCount;
                _isInvincible = true;
            }
            else
            {
                _p.data.healthHandler.Heal(damage.magnitude);
            }
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1 && !_isInvincible)
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
