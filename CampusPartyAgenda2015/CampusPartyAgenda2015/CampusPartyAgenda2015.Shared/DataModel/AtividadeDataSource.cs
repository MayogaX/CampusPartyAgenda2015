using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace CampusPartyAgenda2015.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class AtividadeDataItem
    {
        public AtividadeDataItem(String uniqueId, String title, String subtitle, string start, string color, String description, string end)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Color = color;
            this.Description = description;
            this.Start = start;
            this.End = end;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Color { get; private set; }
        public string Description { get; private set; }
        public string Start { get; private set; }
        public string End { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class AtividadeDataGroup
    {
        public AtividadeDataGroup(String uniqueId, String title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Items = new ObservableCollection<AtividadeDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<AtividadeDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// AtividadeDataSource initializes with data read from a static json file included in the 
    /// project.  This provides Atividade data at both design-time and run-time.
    /// </summary>
    public sealed class AtividadeDataSource
    {
        private static AtividadeDataSource _AtividadeDataSource = new AtividadeDataSource();

        private ObservableCollection<AtividadeDataGroup> _groups = new ObservableCollection<AtividadeDataGroup>();
        public ObservableCollection<AtividadeDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<AtividadeDataGroup>> GetGroupsAsync()
        {
            await _AtividadeDataSource.GetAtividadeDataAsync();

            return _AtividadeDataSource.Groups;
        }

        public static async Task<AtividadeDataGroup> GetGroupAsync(string uniqueId)
        {
            await _AtividadeDataSource.GetAtividadeDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _AtividadeDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<AtividadeDataItem> GetItemAsync(string uniqueId)
        {
            await _AtividadeDataSource.GetAtividadeDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _AtividadeDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetAtividadeDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/AtividadeData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                AtividadeDataGroup group = new AtividadeDataGroup(
                                                       groupObject["UniqueId"].GetString(),
                                                       groupObject["Title"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new AtividadeDataItem(
                        groupObject["UniqueId"].GetString(),
                                                            itemObject["Title"].GetString(),
                                                            itemObject["Subtitle"].GetString(),
                                                            itemObject["Start"].GetString(),
                                                            itemObject["Color"].GetString(),
                                                            itemObject["Description"].GetString(),
                                                            itemObject["End"].GetString()));
                }
                this.Groups.Add(group);
            }
        }
    }
}