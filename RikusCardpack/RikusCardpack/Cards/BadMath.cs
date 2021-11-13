using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RikusCardpack.Cards
{
    class BadMath : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been setup.");

            gun.bulletDamageMultiplier = 0.9f;
            gun.reloadTimeAdd = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            var thisBadMathEffect = player.gameObject.GetOrAddComponent<MonoBehaviours.BadMathEffect>();

            thisBadMathEffect.RunAdder(player, gun, gunAmmo);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            var thisBadMathEffect = player.gameObject.GetComponent<MonoBehaviours.BadMathEffect>();
            if (thisBadMathEffect != null)
            {
                thisBadMathEffect.RunRemover();
            }
        }
        protected override string GetTitle()
        {
            return "Bad Math";
        }
        protected override string GetDescription()
        {
            return "You're so bad at math that you defy the laws of physics.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Max Ammo On Full Reload",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },

                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },

                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.75s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }
        public override string GetModName()
        {
            return RikusCardpack.ModInitials;
        }
    }
}
