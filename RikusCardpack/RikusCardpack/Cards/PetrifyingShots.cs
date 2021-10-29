using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;

namespace RikusCardpack.Cards
{
    class PetrifyingShots : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been setup.");

            statModifiers.movementSpeed = 0.8f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            var thisPetrifyingHitEffect = player.gameObject.GetOrAddComponent<HitEffects.PetrifyingHitEffect>();

            thisPetrifyingHitEffect.RunAdder();

            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                var thisPetrifyHandler = PlayerManager.instance.players[i].gameObject.GetOrAddComponent<MonoBehaviours.PetrifyHandler>();

                thisPetrifyHandler.RunAdder(PlayerManager.instance.players[i]);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RikusCardpack.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            var thisPetrifyingHitEffect = player.gameObject.GetComponent<HitEffects.PetrifyingHitEffect>();
            if (thisPetrifyingHitEffect != null)
            {
                thisPetrifyingHitEffect.RunRemover();
            }

            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                var thisPetrifyHandler = PlayerManager.instance.players[i].gameObject.GetComponent<MonoBehaviours.PetrifyHandler>();
                if (thisPetrifyHandler != null)
                {
                    thisPetrifyHandler.RunRemover();
                }
            }
        }
        protected override string GetTitle()
        {
            return "Petrifying Shots";
        }
        protected override string GetDescription()
        {
            return "Hitting a player petrifies them.";
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
                    stat = "Petrification Time",
                    amount = "+1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Movement Speed",
                    amount = "-20%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return RikusCardpack.ModInitials;
        }
    }
}