using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusForever.Shared.Database.Auth.Model
{
    [Table("server")]
    public partial class Server
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
        public byte Id { get; set; }
        [StringLength(64), Column("name")]
        public string Name { get; set; }
        [StringLength(64), Column("host")]
        public string Host { get; set; }
        [Column("port")]
        public ushort Port { get; set; }
        [Column("type")]
        public byte Type { get; set; }
    }
}
