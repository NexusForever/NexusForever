﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMailResult, MessageDirection.Server)]
    public class ServerMailResult : IWritable
    {
       public uint Action { get; set; }
       public ulong MailId { get; set; }
       public GenericError Result { get; set; }

       public void Write(GamePacketWriter writer)
       {
           writer.Write(Action);
           writer.Write(MailId);
           writer.Write(Result, 8u);
       }
    }
}
