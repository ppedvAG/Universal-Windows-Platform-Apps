using ContactsUWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace ContactsUWP.Common.Helper
{
    public static class TileHelper
    {
        public static async void GenerateSceduledTileNotifications(IEnumerable<Contact> contacts, int numOfTiles)
        {
            List<Contact> cList = contacts.Where(c => c.ContactImage != null).ToList();

            cList.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.CreationDate), Convert.ToDateTime(y.CreationDate)));

            StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("LiveTileImages", CreationCollisionOption.OpenIfExists);

            foreach (var item in await storageFolder.GetFilesAsync())
            {
                await item.DeleteAsync();
            }

            int count = cList.Count >= numOfTiles ? numOfTiles : cList.Count;

            for (int i = 0; i < count; i++)
            {
                Contact c_tmp = cList[i];
                StorageFile tilePic = await storageFolder.CreateFileAsync(c_tmp.Id + ".png", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(tilePic, c_tmp.ContactImage);
                // not over 200K or 1024px or TemporaryFolder
                var imageUri = new Uri("ms-appdata:///local/LiveTileImages/" + c_tmp.Id + ".png", UriKind.Absolute);


                string tileXmlString = "<tile>"
           + "<visual>"
           + "<binding template='TileMedium' branding='name'>"
           + "<image id='1' src='" + imageUri + "'/>"
           + "<text id='1'>" + c_tmp.Name + "</text>"
           + "</binding>"
           + "</visual>"
           + "</tile>";

                XmlDocument tileXml = new XmlDocument();
                tileXml.LoadXml(tileXmlString);
                ScheduledTileNotification sceduleNotification = new ScheduledTileNotification(tileXml, DateTime.Now.AddSeconds(15));
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(sceduleNotification);
            }
        }

        public static async Task<bool> UpdateUnpinnedSecondaryTilesInContacts(IEnumerable<Contact> contacts)
        {
            IReadOnlyList<SecondaryTile> secondaryTiles = await SecondaryTile.FindAllAsync();
            bool changed = false;
            // check model consistency
            foreach (var item in contacts)
            {
                if (item.IsPinned == true && secondaryTiles.FirstOrDefault(t => t.TileId == item.Id) == null)
                {
                    item.IsPinned = false;
                }
            }

            return changed;
        }
    }
}
