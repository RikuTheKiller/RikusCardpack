using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using RikusCardpack.MonoBehaviours;

namespace RikusCardpack.Cards
{
    class Template : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            var thisEffect = player.gameObject.GetOrAddComponent<TemplateEffect>();

            thisEffect.RunAdder();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            var thisEffect = player.gameObject.GetComponent<TemplateEffect>();
            if (thisEffect != null)
            {
                thisEffect.RunRemover();
            }
        }
        protected override string GetTitle()
        {
            return "CardName";
        }
        protected override string GetDescription()
        {
            return "CardDescription";
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
                    stat = "Effect",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return RikusCardpack.ModInitials;
        }
    }
}