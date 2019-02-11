using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FortniteReplayReader
{
    public abstract class ObservableFortniteBinaryReader<T> : FortniteBinaryReader, IObservable<T>
    {
        private IList<IObserver<T>> _observers;

        public ObservableFortniteBinaryReader(Stream input) : base(input)
        {
            _observers = new List<IObserver<T>>();
        }

        public ObservableFortniteBinaryReader(Stream input, int offset) : base(input, offset)
        {
            _observers = new List<IObserver<T>>();

            var desiredType = typeof(IObserver<T>);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type => desiredType.IsAssignableFrom(type));

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as FortniteObserver<T>;
                instance.Subscribe(this);
            }
        }

        public override Replay ReadFile()
        {
            var replay = base.ReadFile();
            OnCompleted();
            return replay;
        }


        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubsriber<T>(_observers, observer);
        }

        internal void Notify(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers.ToArray())
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
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
