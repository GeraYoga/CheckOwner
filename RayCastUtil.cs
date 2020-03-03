﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace GY.CheckOwner
{
    public static class RayCastUtil
    {
        public static Transform GetComponent(UnturnedPlayer player, float distance)
        {
            if (!Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out var hit, distance, RayMasks.BARRICADE_INTERACT)) return null;
            var transform = hit.transform;
            return transform;
        }
        
        public static async Task<OwnerModel> CheckOwner(this Transform obj)
        {
            if (obj == null)
            {
                return null;
            }
            
            var transform = obj;

            var vehicle = obj.GetComponent<InteractableVehicle>();
            
            if (vehicle != null)
            {
                var nameAndGroup = await CheckPair(vehicle.lockedOwner, vehicle.lockedGroup);
                
                return new OwnerModel
                {
                    Group = vehicle.lockedGroup, GroupName = nameAndGroup.Value,
                    Hp = vehicle.health+"/"+vehicle.asset.health, Owner = vehicle.lockedOwner, OwnerName = nameAndGroup.Key,
                    Id = vehicle.id
                };
            }
                    
            if (obj.GetComponent<InteractableDoorHinge>() != null)
            {
                transform = obj.parent.parent;
            }

            if (BarricadeManager.tryGetInfo(transform, out _, out _, out _, out var index, out var r))
            {
                var data = r.barricades[index];
                var owner = (CSteamID)data.owner;
                var group = (CSteamID) data.group;
                
                var nameAndGroup = await CheckPair(owner, group);
                
                return new OwnerModel
                {
                    Group = group, GroupName = nameAndGroup.Value,
                    Hp = data.barricade.health+"/"+data.barricade.asset.health, Owner = owner, OwnerName = nameAndGroup.Key,
                    Id = data.barricade.id
                };
            }

            if (!StructureManager.tryGetInfo(transform, out _, out _, out index, out var s)) return null;
            {
                var data = s.structures[index];
                var owner = (CSteamID)data.owner;
                var group = (CSteamID) data.@group;
                
                var nameAndGroup = await CheckPair(owner, group);
                
                return new OwnerModel
                {
                    Group = group, GroupName = nameAndGroup.Value,
                    Hp = data.structure.health+"/"+data.structure.asset.health, Owner = owner, OwnerName = nameAndGroup.Key,
                    Id = data.structure.id
                };
            }
        }

        private static async Task<KeyValuePair<string, string>> CheckPair(CSteamID steamP, CSteamID steamG)
        {
            var player = UnturnedPlayer.FromCSteamID(steamP);
            var isOnline = player != null;
            var nameAndGroup = await RequestUtil.GetNameAndGroup(steamP, steamG);
            return new KeyValuePair<string, string>(isOnline ? player.CharacterName : nameAndGroup.Key, nameAndGroup.Value);
        }
    }
    
    public class OwnerModel
    {
        public string OwnerName { get; set; }
        public CSteamID Owner { get; set; }
        public string GroupName { get; set; }
        public CSteamID Group { get; set; }
        public string Hp { get; set; } 
        
        public ushort Id { get; set; }
        
    }
}