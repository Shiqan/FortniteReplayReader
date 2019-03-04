using FortniteReplayReader.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace FortniteReplayReader
{
    public class ElimObservableFortniteBinaryReader : ObservableFortniteBinaryReader<PlayerElimination>
    {
        public ElimObservableFortniteBinaryReader(Stream input, bool autoLoad = true, Dictionary<PlayerElimination, int> cache = null) : base(input, autoLoad, cache)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true, Dictionary<PlayerElimination, int> cache = null) : base(input, offset, autoLoad, cache)
        {
        }

        protected override PlayerElimination ParseElimination(uint time)
        {
            var elim = base.ParseElimination(time);
            base.Notify(elim);
            return elim;
        }
    }
}
