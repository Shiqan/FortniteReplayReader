using FortniteReplayReader.Core.Exceptions;
using FortniteReplayReader.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FortniteReplayReader.Core.Contracts
{
    public abstract class FortniteObserver<T>
    {
        public abstract void Subscribe(IObservable<T> provider);

        public abstract void Unsubscribe();

        protected TSettings ReadSettingsFile<TSettings>()
        {
            var path = Path.Combine(new string[] { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FortniteReplayWatcher", $"{this.GetType().Name}.json" });

            if (File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<TSettings>(File.ReadAllText(path));
                }
                catch (Exception)
                {
                    throw new InvalidSettingsException();
                }
            }

            throw new SettingsNotFoundException();
        }
    }
}
