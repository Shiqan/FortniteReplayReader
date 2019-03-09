using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Observerable.Contracts;
using System;

namespace FortniteReplayObservers.File
{
    public class FileObserver : FortniteObserver<PlayerElimination>, IFortniteObserver<PlayerElimination>
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

        public override void Subscribe(IFortniteObservable<PlayerElimination> provider)
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

        public void OnStart()
        {
            System.IO.File.AppendAllText(path, "started");
        }
    }

    public class FileSettings
    {
        public string Path { get; set; }
    }
}