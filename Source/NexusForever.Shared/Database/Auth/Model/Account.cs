using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusForever.Shared.Database.Auth.Model
{
    [Table("account")]
    public partial class Account
    {
        [Column("id")] // Should this be identity?
        public uint Id { get; set; }
        [Column("email"), StringLength(128)]
        public string Email { get; set; }
        [Column("s"), StringLength(32)]
        public string S { get; set; }
        [Column("v"), StringLength(512)]
        public string V { get; set; }
        [Column("gameToken"), StringLength(32)]
        public string GameToken { get; set; }
        [Column("sessionKey"), StringLength(32)]
        public string SessionKey { get; set; }
        [Column("createTime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
