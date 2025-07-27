using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;
using HermesProxy.World.Server.Packets;
using System;
using System.Collections.Generic;
using Framework.Logging;
using Game.Networking.Packets;
using CriteriaProgressPkt = Game.Networking.Packets.CriteriaProgressPkt;

namespace HermesProxy.World.Client
{
    public partial class WorldClient
    {
        // Handlers for SMSG opcodes coming the legacy world server
        [PacketHandler(Opcode.SMSG_ALL_ACHIEVEMENT_DATA)]
        void HandleAllAchievementData(WorldPacket packet)
        {
            var player = GetSession().GameState.CurrentPlayerGuid;
            // read until -1
            
            var achiMap = new Dictionary<int, uint>();

            var items = new List<EarnedAchievement>();
            var realm = GetSession().RealmManager.GetRealm(GetSession().RealmId);
            
            var probablyAchievementId = packet.ReadInt32();
            int cnt = 0;
            while (probablyAchievementId > -1)
            {
                Log.Print(LogType.Debug, $@"Iteration {cnt++}");
                var achievementTime = packet.ReadPackedTime();

                var ac = new EarnedAchievement
                {
                    Owner = player,
                    VirtualRealmAddress = realm.Id.GetAddress(),
                    Id = probablyAchievementId,
                    Date = achievementTime,
                };
                items.Add(ac);
                
                probablyAchievementId = packet.ReadInt32();
            }
            
            var criteria = new List<CriteriaProgressPkt>();
            
            var probablyCriteriaId = packet.ReadInt32();
            while (probablyCriteriaId > -1)
            {
                var counter = packet.ReadPackedGuid();
                var playerGuid = packet.ReadPackedGuid();
                var someZero = packet.ReadUInt32();
                var time1 = packet.ReadPackedTime();
                var time2 = packet.ReadUInt32();
                var time3 = packet.ReadUInt32();
                
                CriteriaProgressPkt progress = new();
                progress.Id = probablyCriteriaId;
                progress.Quantity = (long) counter.To128(GetSession().GameState).GetCounter();
                progress.Player = playerGuid.To128(GetSession().GameState);
                progress.Flags = 0;
                progress.Date = new RealmTime(new UnixTime64(time1));
                //progress.Date += _owner.GetSession().GetTimezoneOffset(); 
                progress.TimeFromStart = 0;
                progress.TimeFromCreate = 0;
                criteria.Add(progress);
                
                probablyCriteriaId = packet.ReadInt32();
            }
            
            AllAchievementData achievementData = new();
            AllAchievements data = new();
            data.Earned = items;
            data.Progress = criteria;

            achievementData.Data = data;
            SendPacketToClient(achievementData);
        }

      
    }
}
