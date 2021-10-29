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
    class SniperLogic : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been setup.");

            statModifiers.health = 0.9f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            var thisSniperLogicEffect = player.gameObject.GetOrAddComponent<MonoBehaviours.SniperLogicEffect>();

            thisSniperLogicEffect.RunAdder(gun);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            var thisSniperLogicEffect = player.gameObject.GetComponent<MonoBehaviours.SniperLogicEffect>();
            if (thisSniperLogicEffect != null)
            {
                thisSniperLogicEffect.RunRemover();
            }
        }
        protected override string GetTitle()
        {
            return "Sniper Logic";
        }
        protected override string GetDescription()
        {
            return "Because of sniper logic, your bullets get stronger and faster the higher your reload time is. Don't question it.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Sniper Logic",
                    amount = "+999%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return RikusCardpack.ModInitials;
        }
    }
}