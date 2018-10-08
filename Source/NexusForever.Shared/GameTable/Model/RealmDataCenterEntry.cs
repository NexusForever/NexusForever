namespace NexusForever.Shared.GameTable.Model
{
    public class RealmDataCenterEntry
    {
        public uint Id;
        public uint Flags;
        public uint DeploymentRegionEnum;
        public uint DeploymentTypeEnum;
        public uint LocalizedTextId;
        public string AuthServer;
        public string NcClientAuthServer;
        public string NcRedirectUrlTemplate;
        public string NcRedirectUrlTemplateSignature;
        public string NcAppGroupCode;
        public uint NcProgramAuth;
        public string SteamSignatureUrlTemplate;
        public string SteamNCoinUrlTemplate;
        public string StoreBannerDataUrlTemplate;
    }
}
