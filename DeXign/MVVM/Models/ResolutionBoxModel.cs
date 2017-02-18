using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DeXign.Resources;

namespace DeXign.Models
{
    public class ResolutionBoxModel : BaseNotifyModel
    {
        private Dictionary<Platform, IList> resolutions;

        private IList _items;
        public IList Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        ResolutionItemModel selectedItem;
        public ResolutionItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public ResolutionBoxModel()
        {
            resolutions = new Dictionary<Platform, IList>();

            foreach (Platform p in Enum.GetValues(typeof(Platform)))
                resolutions[p] = ResourceManager.GetResource<IList>($"{p.ToString()}Resolutions");
        }

        internal void SetPlatform(Platform platform)
        {
            Items = resolutions[platform];
        }
    }
}
