using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using System;
using System.Collections.Generic;

namespace FortniteReplayObservers.File
{
    public class FileObserver : FortniteObserver<PlayerElimination>, IObserver<PlayerElimination>
    {
        private IDisposable unsubscriber;
        private string path = "";
        
        private Dictionary<PlayerElimination, int> _playerEliminations;

        public FileObserver(Dictionary<PlayerElimination, int> playerEliminations)
        {
            _playerEliminations = playerEliminations ?? new Dictionary<PlayerElimination, int>();
            path = @"D:\Projects\FortniteReplayReader\src\ConsoleReader\bin\Debug\netcoreapp2.1\test.txt";
        }


        public void OnCompleted()
        {
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            Unsubscribe();
        }

        private string CreateMessagePayload(PlayerElimination e)
        {
            var type = (e.Knocked) ? "knocked" : "eliminated";
            return $"{e.Eliminator} {type} {e.Eliminated} with {e.GunType} \n";

        }
        public void OnNext(PlayerElimination value)
        {
            if (_playerEliminations.ContainsKey(value)) return;

            System.IO.File.AppendAllText(path, CreateMessagePayload(value));
        }

        public override void Subscribe(IObservable<PlayerElimination> provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }

        public override void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }
}