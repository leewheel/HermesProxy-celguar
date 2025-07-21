using Framework;
using Framework.Logging;
using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;
using HermesProxy.World.Server.Packets;
using System;
using System.Collections.Generic;
using Framework.Logging;
using Game.Networking.Packets;
using static HermesProxy.World.Server.Packets.TradeUpdated;

namespace HermesProxy.World.Client
{
    public partial class WorldClient
    {
        // Handlers for SMSG opcodes coming the legacy world server
        [PacketHandler(Opcode.SMSG_UPDATE_TALENT_DATA)]
        void HandleUpdateTalentdata(WorldPacket packet)
        {
            UpdateTalentData updateTalentData = new();

            var isPet = packet.ReadUInt8();

            if (isPet > 0)
            {
                updateTalentData.IsPetTalents = true;
                Log.Print(LogType.Error, "Pet talents not supported yet!");
                return;
            }
            else
            {
                updateTalentData.IsPetTalents = false;
                updateTalentData.UnspentTalentPoints = (int) packet.ReadUInt32();
                var specCount = packet.ReadUInt8();
                updateTalentData.ActiveGroup = packet.ReadUInt8();
                
                for (byte specId = 0; specId < specCount; specId++)
                {
                    TalentGroupInfo talentGroupInfo = new();
                    talentGroupInfo.SpecID = specId;
                    talentGroupInfo.Talents = new();
                    
                    var talentIdCount = packet.ReadUInt8();

                    for (uint i = 0; i < talentIdCount; i++)
                    {
                        var talentId = packet.ReadUInt32();
                        var talentRank = packet.ReadUInt8();
                        talentGroupInfo.Talents.Add(new TalentInfo
                        {
                            TalentID = (int) talentId,
                            Rank = talentRank
                        });
                    }
                
                    var maxGlyphIndex = packet.ReadUInt8();

                    var glyphs = new List<int>();
                    for (int i = 0; i < maxGlyphIndex; ++i)
                    {
                        var glyphId = packet.ReadUInt16();
                        glyphs.Add(glyphId);
                    }

                    talentGroupInfo.GlyphIDs = glyphs.ToArray();
                    
                    updateTalentData.TalentGroupInfos.Add(talentGroupInfo);
                }
            }
           
            SendPacketToClient(updateTalentData);
        }

    }
}
