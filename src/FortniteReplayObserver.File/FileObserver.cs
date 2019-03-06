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

        public FileObserver()
        {
            var settings = ReadSettingsFile<FileSettings>();
            path = settings.Path;
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

    public class FileSettings
    {
        public string Path { get; set; }
    }
}