using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Exceptions;
using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Enums;
using FortniteReplayReader.Extensions;
using System;
using System.IO;

namespace FortniteReplayReader
{
    public class FortniteBinaryReader : CustomBinaryReader
    {
        public const uint FileMagic = 0x1CA2E27F;

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L107
        /// </summary>
        public const uint NetworkMagic = 0x2CF5A13D;

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L111
        /// </summary>
        public const uint MetadataMagic = 0x3D06B24E;

        protected Replay Replay { get; set; }

        public FortniteBinaryReader(Stream input) : base(input)
        {
            this.Replay = new Replay();
        }

        public FortniteBinaryReader(Stream input, int offset) : base(input)
        {
            if (input.Length < offset)
            {
                throw new EndOfStreamException();
            }

            this.Replay = new Replay();
            BaseStream.Position = offset;
        }

        public virtual Replay ReadFile()
        {
            if (BaseStream.Position == 0)
            {
                this.Replay.Metadata = this.ParseMetadata();
            }

            this.ParseChunks();
            return this.Replay;
        }

        public virtual ReplayMetadata ParseMetadata()
        {
            var magicNumber = ReadUInt32();

            if (magicNumber != FileMagic)
            {
                throw new InvalidReplayException("Invalid replay file");
            }

            var fileVersion = ReadUInt32AsEnum<ReplayVersionHistory>();

            var meta = new ReplayMetadata()
            {
                FileVersion = fileVersion,
                LengthInMs = ReadUInt32(),
                NetworkVersion = ReadUInt32(),
                Changelist = ReadUInt32(),
                FriendlyName = ReadFString(),
                IsLive = ReadUInt32AsBoolean()
            };

            if (fileVersion >= ReplayVersionHistory.RecordedTimestamp)
            {
                meta.Timestamp = DateTime.FromBinary(ReadInt64());
            }

            if (fileVersion >= ReplayVersionHistory.Compression)
            {
                meta.IsCompressed = ReadUInt32AsBoolean();
            }

            return meta;
        }

        public virtual void ParseChunks()
        {
            while (BaseStream.Position < BaseStream.Length)
            {
                var chunkType = ReadUInt32AsEnum<ReplayChunkType>();
                var chunkSize = ReadInt32();
                var offset = BaseStream.Position;

                if (chunkType == ReplayChunkType.Checkpoint)
                {
                    ParseCheckPoint();
                }

                else if (chunkType == ReplayChunkType.Event)
                {
                    ParseEvent();
                }

                else if (chunkType == ReplayChunkType.ReplayData)
                {
                    ParseReplayData();
                }

                else if (chunkType == ReplayChunkType.Header)
                {
                    Replay.Header = ParseHeader();
                }

                BaseStream.Seek(offset + chunkSize, SeekOrigin.Begin);
            }
        }

        public virtual void ParseCheckPoint()
        {
        }

        public virtual IEvent ParseEvent()
        {
            var metadata = new EventMetadata
            {
                Id = ReadFString(),
                Group = ReadFString(),
                Metadata = ReadFString(),
                StartTime = ReadUInt32(),
                EndTime = ReadUInt32(),
                SizeInBytes = ReadInt32()
            };

            if (metadata.Group == ReplayEventTypes.PLAYER_ELIMINATION)
            {
                var elimination = ParseElimination(metadata);
                Replay.Eliminations.Add(elimination);
                return elimination;
            }

            else if (metadata.Metadata == ReplayEventTypes.MATCH_STATS)
            {
                Replay.Stats = ParseMatchStats(metadata);
                return Replay.Stats;
            }

            else if (metadata.Metadata == ReplayEventTypes.TEAM_STATS)
            {
                Replay.TeamStats = ParseTeamStats(metadata);
                return Replay.Stats;
            }

            else if (metadata.Metadata == ReplayEventTypes.ENCRYPTION_KEY)
            {
                return null;
            }

            throw new UnknownEventException();
        }

        public virtual TeamStats ParseTeamStats(EventMetadata metadata)
        {
            return new TeamStats()
            {
                EventMetadata = metadata,
                Unknown = ReadUInt32(),
                Position = ReadUInt32(),
                TotalPlayers = ReadUInt32()
            };
        }

        public virtual Stats ParseMatchStats(EventMetadata metadata)
        {
            SkipBytes(4);
            return new Stats()
            {
                EventMetadata = metadata,
                Accuracy = ReadSingle(),
                Assists = ReadUInt32(),
                Eliminations = ReadUInt32(),
                WeaponDamage = ReadUInt32(),
                OtherDamage = ReadUInt32(),
                Revives = ReadUInt32(),
                DamageTaken = ReadUInt32(),
                DamageToStructures = ReadUInt32(),
                MaterialsGathered = ReadUInt32(),
                MaterialsUsed = ReadUInt32(),
                TotalTraveled = ReadUInt32()
            };
        }

        public virtual PlayerElimination ParseElimination(EventMetadata metadata)
        {
            var branch = Replay.Header.Branch;
            switch (branch)
            {
                case "++Fortnite+Release-4.0":
                    SkipBytes(12);
                    break;
                case "++Fortnite+Release-4.2":
                    SkipBytes(40);
                    break;
                default:
                    SkipBytes(45);
                    break;
            }

            try
            {
                return new PlayerElimination
                {
                    EventMetadata = metadata,
                    Eliminated = ReadFString(),
                    Eliminator = ReadFString(),
                    GunType = ReadByteAsEnum<GunType>(),
                    Knocked = ReadInt32() == 1,
                    Time = metadata.StartTime.MillisecondsToTimeStamp()
                };
            }
            catch (Exception ex)
            {
                throw new PlayerEliminationException($"Error while parsing PlayerElimination at timestamp {metadata.StartTime}", ex);
            }

        }

        public virtual void ParseReplayData()
        {
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L191
        /// </summary>
        /// <returns>Header</returns>
        public virtual Header ParseHeader()
        {
            var magic = ReadUInt32();

            if (magic != NetworkMagic)
            {
                throw new InvalidReplayException($"Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}");
            }

            var header = new Header
            {
                Version = ReadUInt32AsEnum<NetworkVersionHistory>()
            };

            if (header.Version <= NetworkVersionHistory.HISTORY_EXTRA_VERSION)
            {
                throw new InvalidReplayException($"Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: {header.Version}, MIN_NETWORK_DEMO_VERSION: {NetworkVersionHistory.HISTORY_EXTRA_VERSION}");
            }

            header.NetworkChecksum = ReadUInt32();
            header.EngineNetworkVersionHistory = ReadUInt32AsEnum<EngineNetworkVersionHistory>();
            header.GameNetworkProtocolVersion = ReadUInt32();

            if (header.Version >= NetworkVersionHistory.HISTORY_HEADER_GUID)
            {
                header.Guid = ReadGUID();
            }

            if (header.Version >= NetworkVersionHistory.HISTORY_SAVE_FULL_ENGINE_VERSION)
            {
                header.Major = ReadUInt16();
                header.Minor = ReadUInt16();
                header.Patch = ReadUInt16();
                header.Changelist = ReadUInt16();
                header.Branch = ReadFString();                
            }
            else
            {
                header.Changelist = ReadUInt16();
            }

            if (header.Version <= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
            {
                throw new NotImplementedException();
            }
            else
            {
                //  TODO array of tuple parser
                if (ReadUInt32AsBoolean())
                {
                    header.LevelNamesAndTimes = new (string, uint)[1] { (ReadFString(), ReadUInt32()) };
                }
            }

            if (header.Version >= NetworkVersionHistory.HISTORY_HEADER_FLAGS)
            {
                header.Flags = ReadUInt32AsEnum<ReplayHeaderFlags>();
            }

            //  TODO array of strings parser 
            if (ReadUInt32AsBoolean())
            {
                header.GameSpecificData = new string[1] { ReadFString() };
            }

            return header;
        }
    }
}
