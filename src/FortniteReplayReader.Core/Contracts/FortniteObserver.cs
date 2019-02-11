using System;
using System.Collections.Generic;
using System.Text;

namespace FortniteReplayReader.Core.Contracts
{
    public abstract class FortniteObserver<T>
    {
        public abstract void Subscribe(IObservable<T> provider);

        public abstract void Unsubscribe();
    }
}
