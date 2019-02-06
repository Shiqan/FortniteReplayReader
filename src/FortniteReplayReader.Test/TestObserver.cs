using FortniteReplayReader.Models;
using System.IO;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class TestObserver
    {
        [Fact]
        public void TestAthenaMatchStats3()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.33.41.replay";

            var observer = new EliminationObserver();
            using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var provider = new ObservableFortniteBinaryReader<PlayerElimination>(stream);
                observer.Subscribe(provider);
            }
        }
    }
}
