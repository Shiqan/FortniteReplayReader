using FortniteReplayReader.Models;
using System;

namespace FortniteReplayReader
{
    public class EliminationObserver : IObserver<PlayerElimination>
    {
        private IDisposable unsubsriber;

        public void OnCompleted()
        {
            Console.WriteLine("Goodbye world!");
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(PlayerElimination value)
        {
            Console.WriteLine("hello world!");
            Console.WriteLine(value.Eliminated);
        }

        public virtual void Subscribe(IObservable<PlayerElimination> provider)
        {
            if (provider != null)
            {
                unsubsriber = provider.Subscribe(this);
            }
        }

        public virtual void Unsubscribe()
        {
            unsubsriber.Dispose();
        }
    }
}