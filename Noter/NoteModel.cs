using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noter
{
    [Serializable]
    public class NoteModel : INotifyPropertyChanged
    {
        private string title;
        /// <summary>
        /// The title that defines the note, and shows in the tabs window
        /// </summary>
        public string Title
        {
            get { return title; }
            set 
            { 
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string Link { get; set; }
        public NoteModel(string link)
        {
            Title = "new page";
            Link = link;
        }
        /// <summary>
        /// Paramaterless constructor required for serialisation
        /// </summary>
        public NoteModel()
        {

        }

        protected void RaisePropertyChanged(string propertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        [field:NonSerializedAttribute]
        public event PropertyChangedEventHandler PropertyChanged;
        //TODO: Add in iNotify Property change to get real time update in the display list
    }
}
