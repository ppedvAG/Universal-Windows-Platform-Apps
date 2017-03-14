using ContactsUWP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ContactsUWP.Common.Helper
{
    public static class CortanaHelper
    {
        public static string GetRequestedContact(VoiceCommandActivatedEventArgs args)
        {
            // Voice command activation.
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                SpeechRecognitionResult speechRecognitionResult = args.Result;

                // Get the name of the voice command and the text spoken. 
                // See VoiceCommands.xml for supported voice commands.
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;

                // commandMode indicates whether the command was entered using speech or text.
                // Apps should respect text mode by providing silent (text) feedback.
                string commandMode = SemanticInterpretation("commandMode", speechRecognitionResult);
                string user = string.Empty;

                switch (voiceCommandName)
                {
                    case "zeige":
                        // Access the value of {destination} in the voice command.
                        user = SemanticInterpretation("nutzer", speechRecognitionResult);
                        break;
                    default:
                        break;
                }

                return user;
            }

            return null;
        }

        /// <summary>
        /// Returns the semantic interpretation of a speech result. 
        /// Returns null if there is no interpretation for that key.
        /// </summary>
        /// <param name="interpretationKey">The interpretation key.</param>
        /// <param name="speechRecognitionResult">The speech recognition result to get the semantic interpretation from.</param>
        /// <returns></returns>
        private static string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }

        private static async Task WriteToVoiceCommandDefinitionAsync(IEnumerable<Contact> contacts)
        {
            try
            {
                //create file in public folder
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile publicVoiceCommandFile = await storageFolder.CreateFileAsync("VoiceCommandDefinition.xml", CreationCollisionOption.ReplaceExisting);

                Uri uri = new Uri("ms-appx:///Common/Cortana/VoiceCommandDefinition.xml");
                StorageFile privateVoiceCommandFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                string stringData = await FileIO.ReadTextAsync(privateVoiceCommandFile);

                //write sring to created file
                await FileIO.WriteTextAsync(publicVoiceCommandFile, stringData);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex);
            }

            //StorageFile myFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Cortana/VoiceCommandDefinition.xml");
            StorageFile vcf = await ApplicationData.Current.LocalFolder.GetFileAsync("VoiceCommandDefinition.xml");
            using (IRandomAccessStream writeStream = await vcf.OpenAsync(FileAccessMode.ReadWrite))
            {
                XNamespace defaultNs = "http://schemas.microsoft.com/voicecommands/1.2";

                // convert IRandomAccessStream to IO.Stream
                Stream s = writeStream.AsStreamForWrite();

                //xml
                XDocument document = XDocument.Load(s);
                var element = document.Root.Elements().FirstOrDefault();
                element.Add(new XElement(defaultNs + "PhraseList", new XAttribute("Label", "nutzer")));

                var PhraseList = element.Element(defaultNs + "PhraseList");

                foreach (var item in contacts)
                {
                    PhraseList.Add(new XElement("Item", item.Name));
                }

                s.Seek(0, SeekOrigin.Begin);
                document.Save(s);
            }

            ////get asets folder
            //StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
            //StorageFolder assetsFolder = await appInstalledFolder.GetFolderAsync("Cortana");

            ////move file from public folder to assets
            //await vcf.MoveAsync(assetsFolder, "VoiceCommandDefinition.xml", NameCollisionOption.ReplaceExisting);
        }

        private static async void RegisterVoiceCommandDefinition()
        {
            try
            {
                // Install the main VCD.
                //create file in public folder
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile publicVoiceCommandFile = await storageFolder.GetFileAsync("VoiceCommandDefinition.xml");

                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(publicVoiceCommandFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Installing Voice Commands Failed: " + ex.ToString());
            }
        }

        public static async void WriteAndUpdateVoiceCommandDefinition(IEnumerable<Contact> contacts)
        {
            await WriteToVoiceCommandDefinitionAsync(contacts);
            RegisterVoiceCommandDefinition();
        }
    }
}
