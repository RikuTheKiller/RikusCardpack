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
    class Autoloader : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been setup.");

            gun.bulletDamageMultiplier = 0.4f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            var thisAutoloaderEffect = player.gameObject.GetOrAddComponent<MonoBehaviours.AutoloaderEffect>();

            thisAutoloaderEffect.RunAdder(gunAmmo, player);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            var thisAutoloaderEffect = player.gameObject.GetComponent<MonoBehaviours.AutoloaderEffect>();
            if (thisAutoloaderEffect != null)
            {
                thisAutoloaderEffect.RunRemover();
            }
        }
        protected override string GetTitle()
        {
            return "Autoloader";
        }
        protected override string GetDescription()
        {
            return "Running out of ammo reloads your weapon instantly.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Reload Time",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-60%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return RikusCardpack.ModInitials;
        }
    }
}