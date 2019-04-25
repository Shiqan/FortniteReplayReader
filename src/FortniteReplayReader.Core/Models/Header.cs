using FortniteReplayReader.Core.Models.Enums;

namespace FortniteReplayReader.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L151
    /// </summary>
    public class Header
    {
        public NetworkVersionHistory Version { get; set; }
        public uint NetworkChecksum { get; set; }
        public EngineNetworkVersionHistory EngineNetworkVersionHistory { get; set; }
        public uint GameNetworkProtocolVersion { get; set; }
        public string Guid { get; set; } = "";
        public uint Major { get; set; }
        public uint Minor { get; set; }
        public uint Patch { get; set; }
        public uint Changelist { get; set; }
        public string Branch { get; set; } = "";
        public (string, uint)[] LevelNamesAndTimes { get; set; }
        public ReplayHeaderFlags Flags { get; set; }
        public uint Time { get; set; }
        public string[] GameSpecificData { get; set; }
    }
}
