using System;
using System.Collections.Generic;
using System.IO;

namespace FortniteReplayReader
{
    public class ObservableFortniteBinaryReader<T> : FortniteBinaryReader, IObservable<T>
    {
        private IList<IObserver<T>> _observers;

        public ObservableFortniteBinaryReader(Stream input) : base(input)
        {
            this._observers = new List<IObserver<T>>();

            this.ParseMeta();
            this.ParseChunks();
            this.OnCompleted();
        }

        public ObservableFortniteBinaryReader(Stream input, int offset) : base(input)
        {
            this._observers = new List<IObserver<T>>();

            this.ParseChunks();
            this.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubsriber<T>(_observers, observer);
        }

        private void Notify(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }

        private class Unsubsriber<V> : IDisposable
        {
            private IList<IObserver<V>> _observers;
            private readonly IObserver<V> _observer;

            public Unsubsriber(IList<IObserver<V>> observers, IObserver<V> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }
            
            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
