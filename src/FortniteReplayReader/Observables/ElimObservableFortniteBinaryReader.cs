using FortniteReplayReader.Models;
using System.IO;

namespace FortniteReplayReader
{
    public class ElimObservableFortniteBinaryReader : ObservableFortniteBinaryReader<PlayerElimination>
    {
        public ElimObservableFortniteBinaryReader(Stream input) : base(input)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, int offset) : base(input, offset)
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
