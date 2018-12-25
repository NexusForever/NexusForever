using System;
using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerActionSet, MessageDirection.Server)]
    public class ServerActionSet : IWritable
    {
		public class ActionLocation : IWritable
        {   
            public byte   Unknown0 { get; set; } = 0;
            public ushort Unknown4 { get; set; } = 300;
            public uint   Location { get; set; } = (uint)UILocation.None;
            public uint   Spell4BaseId { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 4u);
                writer.Write(Unknown4, 9u);
                writer.Write(Location, 32u);
                writer.Write(Spell4BaseId, 32u);
            }
        }

		public byte ActionSetIndex = 0;
		public byte Unknown3 = 1;
		public byte Unknown5 = 1;
		public List<ActionLocation> actionLocation { get; set; } = new List<ActionLocation>();

        public void Write(GamePacketWriter writer)
        {
			writer.Write(ActionSetIndex, 3u);
			writer.Write(Unknown3, 2u);
			writer.Write(Unknown5, 6u);
	 		writer.Write(actionLocation.Count, 6u);
            actionLocation.ForEach(e => e.Write(writer));
        }
    }
}
