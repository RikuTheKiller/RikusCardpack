using System.Reflection;
using ModdingUtils.RoundsEffects;
using UnboundLib;
using UnityEngine;

namespace RikusCardpack.HitEffects
{
    public class PetrifyingHitEffect : HitEffect
    {
        private bool _ranOnce = false;
        private int _stackCount = 1;
        private float _t = 1;
        public void RunAdder()
        {
            if (_ranOnce)
            {
                _stackCount += 1;
                _t += 1;

                return;
            }
            _ranOnce = true;
        }
        public override void DealtDamage(
            Vector2 damage,
            bool selfDamage,
            Player damagedPlayer = null
        )
        {
            if (damagedPlayer == null)
            {
                return;
            }

            var thisPetrifyHandler = damagedPlayer.gameObject.GetOrAddComponent<MonoBehaviours.PetrifyHandler>();
            if (thisPetrifyHandler != null)
            {
                thisPetrifyHandler.PetrifyPlayer(_t);
            }
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                Destroy(this);
            }
        }
    }
}