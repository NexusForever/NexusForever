using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Path")]
    public class PathCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public PathCommandHandler()
            : base(true, "path")
        {
        }

        [SubCommandHandler("activate", "pathId - Activate a path for this player.")]
        public Task AddPathActivateSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint newPath = 0;
            if (parameters.Length > 0)
                newPath = uint.Parse(parameters[0]);

            context.Session.Player.PathManager.ActivatePath((Path)newPath);

            return Task.CompletedTask;
        }

        [SubCommandHandler("unlock", "[pathId] - Unlock a path for this player.")]
        public Task AddPathUnlockSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                uint unlockPath = uint.Parse(parameters[0]);
                context.Session.Player.PathManager.UnlockPath((Path)unlockPath);
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("test", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTestSubCommand(CommandContext context, string command, string[] parameters)
        {
            //uint unk0 = uint.Parse(parameters[0]);
            //context.Session.EnqueueMessageEncrypted(new Server06B5
            //{
            //    EpisodeId = 28, // EpisodeID
            //    Missions = new List<Server06B5.Mission>{
            //        new Server06B5.Mission
            //        {
            //            MissionId = 42, // MissionID
            //            Unknown1 = true,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 160,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 648,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        }
            //    }
            //});

            //context.Session.EnqueueMessageEncrypted(new Server06B5
            //{
            //    EpisodeId = 18, // EpisodeID
            //    Missions = new List<Server06B5.Mission>{
            //        new Server06B5.Mission
            //        {
            //            MissionId = 1048, // MissionID
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 1056,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 1057,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        }
            //    }
            //});

            return Task.CompletedTask;
        }

        [SubCommandHandler("test1", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest1SubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new ServerPathCurrentEpisode
            {
                Unknown0 = 426, //
                EpisodeId = 28  // EpisodeID
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test2", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest2SubCommand(CommandContext context, string command, string[] parameters)
        {
            ushort unk0 = ushort.Parse(parameters[0]);
            uint unk1 = uint.Parse(parameters[1]);
            context.Session.EnqueueMessageEncrypted(new Server06B6
            {
                Unknown0 = unk0, // EpisodeID
                Unknown1 = unk1
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test3", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest3SubCommand(CommandContext context, string command, string[] parameters)
        {
            ushort unk0 = ushort.Parse(parameters[0]);
            context.Session.EnqueueMessageEncrypted(new Server06B7
            {
                Unknown0 = unk0 // EpisodeID
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test4", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest4SubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new Server06B8
            {
                Unknown0 = 0,
                Unknown1 = 28 // EpisodeID
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test5", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest5SubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new Server06B9
            {
                Unknown0 = 28, // EpisodeID
                Unknown1 = 0,
                Unknown2 = 0
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test6", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest6SubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new ServerPathMissionActivate
            {
                Missions = new List<ServerPathMissionActivate.Mission>{
                    new ServerPathMissionActivate.Mission
                    {
                        MissionId = 160, 
                        Completed = false, 
                        ProgressPercent = 0, 
                        MissionStep = 1, 
                        Unknown4 = 1,
                        Unknown5 = 0
                    }
                }
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("test7", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTest7SubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new ServerPathMissionUpdate
            {
                MissionId = 42,
                Completed = true,
                ProgressPercent = 0,
                MissionStep = 0 
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("testexplorer", "This should show Northern Wilds with 1 mission discovered/complete & Algoroc with 0 missions discovered")]
        public Task AddPathTestExplorerSubCommand(CommandContext context, string command, string[] parameters)
        {
            // TODO: Figure out the missing packet(s) to "show" Server06B5 to the client

            //context.Session.EnqueueMessageEncrypted(new Server06B5
            //{
            //    EpisodeId = 9,
            //    Missions = new List<Server06B5.Mission>{
            //        new Server06B5.Mission
            //        {
            //            MissionId = 35,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 36,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 158,
            //            Unknown1 = false,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        },
            //        new Server06B5.Mission
            //        {
            //            MissionId = 1254,
            //            Unknown1 = true,
            //            Unknown2 = 0,
            //            Unknown3 = 0
            //        }
            //    }
            //});

            context.Session.EnqueueMessageEncrypted(new ServerPathCurrentEpisode
            {
                Unknown0 = 23,
                EpisodeId = 39
            });

            return Task.CompletedTask;
        }
    }
}
