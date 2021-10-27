using System.Reflection;
using ModdingUtils.RoundsEffects;
using UnboundLib;
using UnityEngine;

namespace RikusCardpack.HitEffects
{
    public class InstaKillHitEffect : HitEffect
    {
        public bool _hasHit = true;
        public Player _damagedPlayer;
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

            if (!_hasHit)
            {
                Unbound.Instance.ExecuteAfterSeconds(0f, () =>
                {
                    typeof(HealthHandler).InvokeMember(
                        "RPCA_Die",
                        BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                        null,
                        damagedPlayer.data.healthHandler,
                        new object[] { new Vector2(0, 1) }
                    );
                });
            }

            _hasHit = true;
            _damagedPlayer = damagedPlayer;
        }
    }
}