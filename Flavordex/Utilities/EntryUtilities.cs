using CsvHelper;
using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Data.Json;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Common methods for managing journal entries.
    /// </summary>
    public class EntryUtilities
    {
        /// <summary>
        /// The format string for the sharing subject.
        /// </summary>
        private static readonly string _shareSubjectFormat = ResourceLoader.GetForCurrentView().GetString("Share/Subject");

        /// <summary>
        /// The format string for the sharing body.
        /// </summary>
        private static readonly string _shareBodyFormat = ResourceLoader.GetForCurrentView().GetString("Share/Body");

        /// <summary>
        /// Opens the sharing UI for a journal entry.
        /// </summary>
        /// <param name="entry">The Entry to share.</param>
        public static void ShareEntry(Entry entry)
        {
            DataTransferManager.GetForCurrentView().DataRequested += (DataTransferManager sender, DataRequestedEventArgs args) =>
            {
                args.Request.Data.Properties.Title = string.Format(_shareSubjectFormat, entry.Title);
                args.Request.Data.SetText(string.Format(_shareBodyFormat, entry.Title, entry.Rating));
            };
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Deletes a journal entry.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        public static async void DeleteEntry(Entry entry)
        {
            await DatabaseHelper.DeleteEntryAsync(entry);
            PhotoUtilities.DeleteThumbnail(entry.ID);
        }

        /// <summary>
        /// Container for CSV records.
        /// </summary>
        private class CsvRecord
        {
            public string uuid { get; set; }
            public string title { get; set; }
            public string cat { get; set; }
            public string maker { get; set; }
            public string origin { get; set; }
            public string price { get; set; }
            public string location { get; set; }
            public string date { get; set; }
            public double rating { get; set; }
            public string notes { get; set; }
            public string extras { get; set; }
            public string flavors { get; set; }
            public string photos { get; set; }
        }

        /// <summary>
        /// Exports a list of journal entries to a CSV file.
        /// </summary>
        /// <param name="entryIds">
        /// The list of primary IDs for the journal entries to export.
        /// </param>
        /// <returns>Whether the export was completed successfully.</returns>
        public static async Task<bool> ExportEntriesAsync(Collection<long> entryIds)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Comma Separated Value", new List<string>() { ".csv" });
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "flavordex_" + DateTime.Now.ToString("yyyy_MM_dd");
            picker.DefaultFileExtension = ".csv";
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var writer = new StreamWriter(stream);
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.Configuration.QuoteAllFields = true;
                        csv.WriteHeader(typeof(CsvRecord));
                        foreach (var entryId in entryIds)
                        {
                            var entry = await DatabaseHelper.GetEntryAsync(entryId);
                            var record = new CsvRecord()
                            {
                                uuid = entry.UUID,
                                title = entry.Title,
                                cat = entry.Category,
                                maker = entry.Maker,
                                origin = entry.Origin,
                                price = entry.Price,
                                location = entry.Location,
                                date = entry.Date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm'Z'"),
                                rating = entry.Rating,
                                notes = entry.Notes,
                                extras = await SerializeExtrasAsync(entry.ID),
                                flavors = await SerializeFlavorsAsync(entry.ID),
                                photos = await SerializePhotosAsync(entry.ID)
                            };
                            csv.WriteRecord(record);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Serializes the extra fields for a journal entry into a JSON object string.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A JSON object string.</returns>
        private static async Task<string> SerializeExtrasAsync(long entryId)
        {
            var json = new JsonObject();
            foreach (var extra in await DatabaseHelper.GetEntryExtrasAsync(entryId))
            {
                json[extra.Name] = JsonValue.CreateStringValue(extra.Value);
            }
            return json.Stringify();
        }

        /// <summary>
        /// Serializes the flavors for a journal entry into a JSON object string.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A JSON object string.</returns>
        private static async Task<string> SerializeFlavorsAsync(long entryId)
        {
            var json = new JsonObject();
            foreach (var flavor in await DatabaseHelper.GetEntryFlavorsAsync(entryId))
            {
                json[flavor.Name] = JsonValue.CreateNumberValue(flavor.Value);
            }
            return json.Stringify();
        }

        /// <summary>
        /// Serializes the photos for a journal entry into a JSON array string.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A JSON array string.</returns>
        private static async Task<string> SerializePhotosAsync(long entryId)
        {
            var json = new JsonArray();
            foreach (var photo in await DatabaseHelper.GetEntryPhotosAsync(entryId))
            {
                json.Add(JsonValue.CreateStringValue(photo.Path));
            }
            return json.Stringify();
        }

        /// <summary>
        /// Imports journal entries from a CSV file.
        /// </summary>
        /// <returns>Whether the import was completed successfully.</returns>
        public static async Task<bool> ImportEntriesAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var reader = new StreamReader(stream);
                    using (var csv = new CsvReader(reader))
                    {
                        var records = new Collection<ImportRecord>();
                        while (csv.Read())
                        {
                            var row = csv.GetRecord<CsvRecord>();
                            var entry = new Entry()
                            {
                                UUID = row.uuid,
                                Title = row.title,
                                Category = row.cat,
                                Maker = row.maker,
                                Origin = row.origin,
                                Price = row.price,
                                Location = row.location,
                                Date = DateTime.Parse(row.date),
                                Rating = row.rating,
                                Notes = row.notes
                            };

                            records.Add(new ImportRecord()
                            {
                                Entry = entry,
                                Extras = ParseExtras(row),
                                Flavors = ParseFlavors(row),
                                Photos = ParsePhotos(row)
                            });
                        }

                        return await new ImportDialog(records).ShowAsync() == ContentDialogResult.Primary;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parse the extra fields from a CSV record.
        /// </summary>
        /// <param name="record">The CsvRecord.</param>
        /// <returns>A Collection of EntryExtras.</returns>
        private static Collection<EntryExtra> ParseExtras(CsvRecord record)
        {
            var list = new Collection<EntryExtra>();
            JsonObject json;
            if (JsonObject.TryParse(record.extras, out json))
            {
                foreach (var item in json)
                {
                    list.Add(new EntryExtra()
                    {
                        Name = item.Key,
                        Value = item.Value.GetString()
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// Parse the flavors from a CSV record.
        /// </summary>
        /// <param name="record">The CsvRecord.</param>
        /// <returns>A Collection of EntryFlavors.</returns>
        private static Collection<EntryFlavor> ParseFlavors(CsvRecord record)
        {
            var list = new Collection<EntryFlavor>();
            JsonObject json;
            if (JsonObject.TryParse(record.flavors, out json))
            {
                foreach (var item in json)
                {
                    list.Add(new EntryFlavor()
                    {
                        Name = item.Key,
                        Value = (long)item.Value.GetNumber()
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// Parse the photos from a CSV record.
        /// </summary>
        /// <param name="record">The CsvRecord.</param>
        /// <returns>A Collection of Photos.</returns>
        private static Collection<Photo> ParsePhotos(CsvRecord record)
        {
            var list = new Collection<Photo>();
            JsonArray json;
            if (JsonArray.TryParse(record.photos, out json))
            {
                foreach (var item in json)
                {
                    list.Add(new Photo()
                    {
                        Path = item.GetString()
                    });
                }
            }
            return list;
        }
    }
}
