namespace NexusForever.GameTable.Model
{
    public class RealmDataCenterEntry
    {
        public uint Id { get; set; }
        public uint Flags { get; set; }
        public uint DeploymentRegionEnum { get; set; }
        public uint DeploymentTypeEnum { get; set; }
        public uint LocalizedTextId { get; set; }
        public string AuthServer { get; set; }
        public string NcClientAuthServer { get; set; }
        public string NcRedirectUrlTemplate { get; set; }
        public string NcRedirectUrlTemplateSignature { get; set; }
        public string NcAppGroupCode { get; set; }
        public uint NcProgramAuth { get; set; }
        public string SteamSignatureUrlTemplate { get; set; }
        public string SteamNCoinUrlTemplate { get; set; }
        public string StoreBannerDataUrlTemplate { get; set; }
    }
}
