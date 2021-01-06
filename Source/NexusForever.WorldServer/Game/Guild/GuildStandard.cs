using System;
using System.Linq;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;
using NetworkGuildStandard = NexusForever.WorldServer.Network.Message.Model.Shared.GuildStandard;

namespace NexusForever.WorldServer.Game.Guild
{
    public class GuildStandard : IBuildable<NetworkGuildStandard>
    {
        public class GuildStandardPart : IBuildable<NetworkGuildStandard.GuildStandardPart>
        {
            public GuildStandardPartType Type { get; }
            public GuildStandardPartEntry GuildStandardPartEntry { get; }
            public ushort DyeColorRampId1 { get; }
            public ushort DyeColorRampId2 { get; }
            public ushort DyeColorRampId3 { get; }

            /// <summary>
            /// Create a new <see cref="GuildStandardPart"/> with supplied parameters.
            /// </summary>
            public GuildStandardPart(GuildStandardPartType type, ushort guildStandardPartId,
                ushort dyeColorRampId1, ushort dyeColorRampId2, ushort dyeColorRampId3)
            {
                GuildStandardPartEntry entry = GameTableManager.Instance.GuildStandardPart.GetEntry(guildStandardPartId);
                if (entry == null)
                    throw new ArgumentException();

                Type                   = type;
                GuildStandardPartEntry = entry;
                DyeColorRampId1        = dyeColorRampId1;
                DyeColorRampId2        = dyeColorRampId2;
                DyeColorRampId3        = dyeColorRampId3;
            }

            /// <summary>
            /// Returns if <see cref="GuildStandardPart"/> contains valid data.
            /// </summary>
            public bool Validate()
            {
                if (GuildStandardPartEntry == null || (GuildStandardPartType)GuildStandardPartEntry.GuildStandardPartTypeEnum != Type)
                    return false;

                ushort[] colourRamps = { DyeColorRampId1, DyeColorRampId2, DyeColorRampId3 };
                return colourRamps.All(c => c == 0 || GameTableManager.Instance.DyeColorRamp.GetEntry(c) != null);
            }

            public NetworkGuildStandard.GuildStandardPart Build()
            {
                return new NetworkGuildStandard.GuildStandardPart
                {
                    GuildStandardPartId = (ushort)GuildStandardPartEntry.Id,
                    DyeColorRampId1     = DyeColorRampId1,
                    DyeColorRampId2     = DyeColorRampId2,
                    DyeColorRampId3     = DyeColorRampId3
                };
            }
        }

        public GuildStandardPart BackgroundIcon { get; }
        public GuildStandardPart ForegroundIcon { get; }
        public GuildStandardPart ScanLines { get; }

        /// <summary>
        /// Create a new <see cref="GuildStandard"/> from an existing database model.
        /// </summary>
        public GuildStandard(GuildDataModel model)
        {
            BackgroundIcon = new GuildStandardPart(GuildStandardPartType.Background, model.BackgroundIconPartId, 0, 0, 0);
            ForegroundIcon = new GuildStandardPart(GuildStandardPartType.Foreground, model.ForegroundIconPartId, 0, 0, 0);
            ScanLines      = new GuildStandardPart(GuildStandardPartType.ScanLines, model.ScanLinesPartId, 0, 0, 0);
        }

        /// <summary>
        /// Create a new <see cref="GuildStandard"/> from a network model.
        /// </summary>
        public GuildStandard(NetworkGuildStandard model)
        {
            BackgroundIcon = new GuildStandardPart(GuildStandardPartType.Background, model.BackgroundIcon.GuildStandardPartId,
                model.BackgroundIcon.DyeColorRampId1, model.BackgroundIcon.DyeColorRampId2, model.BackgroundIcon.DyeColorRampId3);
            ForegroundIcon = new GuildStandardPart(GuildStandardPartType.Foreground, model.ForegroundIcon.GuildStandardPartId,
                model.ForegroundIcon.DyeColorRampId1, model.ForegroundIcon.DyeColorRampId2, model.ForegroundIcon.DyeColorRampId3);
            ScanLines      = new GuildStandardPart(GuildStandardPartType.ScanLines, model.ScanLines.GuildStandardPartId,
                model.ScanLines.DyeColorRampId1, model.ScanLines.DyeColorRampId2, model.ScanLines.DyeColorRampId3);
        }

        /// <summary>
        /// Create a new <see cref="GuildStandard"/> from supplied ids.
        /// </summary>
        public GuildStandard(ushort backgroundIconPartId, ushort foregroundIconPartId, ushort scanLinesPartId)
        {
            BackgroundIcon = new GuildStandardPart(GuildStandardPartType.Background, backgroundIconPartId, 0, 0, 0);
            ForegroundIcon = new GuildStandardPart(GuildStandardPartType.Foreground, foregroundIconPartId, 0, 0, 0);
            ScanLines      = new GuildStandardPart(GuildStandardPartType.ScanLines, scanLinesPartId, 0, 0, 0);
        }

        /// <summary>
        /// Returns if <see cref="GuildStandard"/> contains valid data.
        /// </summary>
        public bool Validate()
        {
            GuildStandardPart[] parts = { BackgroundIcon, ForegroundIcon, ScanLines };
            return parts.All(part => part.Validate());
        }

        public NetworkGuildStandard Build()
        {
            return new NetworkGuildStandard
            {
                BackgroundIcon = BackgroundIcon.Build(),
                ForegroundIcon = ForegroundIcon.Build(),
                ScanLines      = ScanLines.Build()
            };
        }
    }
}
