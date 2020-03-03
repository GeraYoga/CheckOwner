using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using static GY.CheckOwner.CheckOwner;

namespace GY.CheckOwner
{
    public class CommandWhois : IRocketCommand
    {
        public async void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer) caller;
            var objTransform = RayCastUtil.GetComponent(player, Instance.Configuration.Instance.Distance);
            
            var ownerInfo = await objTransform.CheckOwner();
            
            if (ownerInfo == null)
            {
                UnturnedChat.Say(player, Instance.Translate("object_null"), Color.red);
                return;
            }

            UnturnedChat.Say(player, Instance.Translate("object_info", ownerInfo.Id, ownerInfo.Hp, ownerInfo.OwnerName + $"({ownerInfo.Owner})", ownerInfo.GroupName + $"({ownerInfo.Group})"), Color.magenta);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Whois";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> {"gy.command.whois"};
    }
}