using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace GY.CheckOwner
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CheckOwner : RocketPlugin<Config>
    {
        public static CheckOwner Instance;

        protected override void Load()
        {
            Instance = this;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEventsOnOnPlayerUpdateGesture;
        }


        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"object_null", "Невозможно определить или найти объект на который вы смотрите!"},
            {"object_info", "ID: {0}, HP: {1}, Owner: {2}, Group: {3}."}
        };

        private async void UnturnedPlayerEventsOnOnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            var cfg = Configuration.Instance;

            if (!cfg.UseGesture || !player.HasPermission(cfg.GesturePermission)) return;

            if (gesture != UnturnedPlayerEvents.PlayerGesture.Point) return;

            var objTransform = RayCastUtil.GetComponent(player, cfg.Distance);
            var ownerInfo = await objTransform.CheckOwner();
            if (ownerInfo == null)
            {
                UnturnedChat.Say(player, Translate("object_null"), Color.red);
                return;
            }

            UnturnedChat.Say(player, Translate("object_info", ownerInfo.Id, ownerInfo.Hp, ownerInfo.OwnerName + $"({ownerInfo.Owner})", ownerInfo.GroupName + $"({ownerInfo.Group})"), Color.magenta);
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEventsOnOnPlayerUpdateGesture;
            Instance = null;
        }
    }
}