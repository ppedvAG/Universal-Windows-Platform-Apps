using ContactsUWP.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace ContactsUWP.Data
{
    public class DataService
    {
        private static volatile DataService instance;
        private static object syncRoot = new object();

        private DataService()
        {
        }

        public static DataService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataService();
                    }
                }

                return instance;
            }
        }

        private Contacts _contacts = new Contacts();

        public Contacts Contacts
        {
            get { return _contacts; }
        }

        public async Task SaveAsync()
        {
            try
            {
                var Folder = ApplicationData.Current.LocalFolder;
                var file = await Folder.CreateFileAsync("Contacts.json", CreationCollisionOption.ReplaceExisting);
                var data = await file.OpenStreamForWriteAsync();

                using (StreamWriter r = new StreamWriter(data))
                {
                    var serelizedfile = JsonConvert.SerializeObject(Contacts as List<Contact>);
                    r.Write(serelizedfile);
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                var Folder = ApplicationData.Current.LocalFolder;
                var file = await Folder.GetFileAsync("Contacts.json");
                var data = await file.OpenReadAsync();

                using (StreamReader r = new StreamReader(data.AsStream()))
                {
                    string text = r.ReadToEnd();
                    Contacts contacts = new Contacts(JsonConvert.DeserializeObject<List<Contact>>(text));
                    _contacts = contacts;
                }
            }
            catch (Exception)
            {
            }
        }

        async public Task DeleteAsync()
        {
            var resume = await ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.json");
            await resume.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
    }
}
