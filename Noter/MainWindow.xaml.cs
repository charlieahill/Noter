using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Noter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The full file path including root, filename, and extension of the file that contains the serialized list of notes
        /// </summary>
        /// <example>Documents/CHillSW/noter/notes.not</example>
        public string FilePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/CHillSW/noter/notes.not";

        /// <summary>
        /// Stores the currently displayed note
        /// </summary>
        public NoteModel CurrentNote { get; set; }

        /// <summary>
        /// The list of notes in the program
        /// </summary>
        BindingList<NoteModel> notes = new BindingList<NoteModel>();

        /// <summary>
        /// Shows archived topics
        /// </summary>
        public bool ShowArchived { get; set; } = false;

        /// <summary>
        /// Generates a new instance of the main window class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            notes = SaveLoad.Load(FilePath);
            RefreshListBoxItemsSource();
            NotesListBox.SelectedIndex = 0;
            //TODO: For now, this is set to only have 1 project, long term plan should be open and save to different files - have multiple different sets of notes
        }

        /// <summary>
        /// On close of window, save program data
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveCurrentNote();
            notes.Save(FilePath);
        }

        /// <summary>
        /// Save current note, and load in next note on selection changed
        /// </summary>
        private void DaysListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListBox.Items.Count == 0)
                return;

            SaveCurrentNote();
            LoadNextNote();
        }

        /// <summary>
        /// Saves the contents of the current note, and loads in the next note
        /// </summary>
        private void SaveCurrentNote()
        {
            if (CurrentNote != null)
                //Save rich text box
                RichTextBoxSave(CurrentNote);

            CurrentNote = (NoteModel)NotesListBox.SelectedItem;
        }

        /// <summary>
        /// Loads the next note
        /// </summary>
        private void LoadNextNote()
        {
            DataContext = CurrentNote;

            //Load rich text box
            RichTextBoxLoad(CurrentNote);
        }

        /// <summary>
        /// Saves the contents of the rich text box to file
        /// </summary>
        private void RichTextBoxSave(NoteModel currentNote)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            
            if(!Directory.Exists(Path.GetDirectoryName(currentNote.Link)))
            {
                Console.WriteLine($"Creating directory... {Path.GetDirectoryName(currentNote.Link)}...");
                Directory.CreateDirectory(Path.GetDirectoryName(currentNote.Link));
            }
            
            Console.WriteLine($"Saving rtf content to... {currentNote.Link}...");
            using (FileStream fileStream = new FileStream(currentNote.Link, FileMode.Create))
                range.Save(fileStream, DataFormats.Rtf);
        }

        /// <summary>
        /// Loads the contents of the rich text box from file
        /// </summary>
        private void RichTextBoxLoad(NoteModel currentNote)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            if (!File.Exists(currentNote.Link))
            {
                Console.WriteLine("rtf document doesn't exist, loading blank string into rtf...");
                range.Text = string.Empty;
                return;
            }

            using (FileStream fileStream = new FileStream(currentNote.Link, FileMode.Open))
            {
                Console.WriteLine($"Loading rtf content from... {currentNote.Link}...");
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        /// <summary>
        /// Creates a new note when the user clicks the add button
        /// </summary>
        private void AddNewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            //Working towards CHillSW/noter/notes01.rtf
            //Create the next note, working out the index, and building the link as above
            string fileName = $"{Path.GetDirectoryName(FilePath)}\\{Path.GetFileNameWithoutExtension(FilePath)}{notes.Count}.rtf";
            Console.WriteLine("FileName for new index: " + fileName);

            notes.Add(new NoteModel(fileName));

            RefreshListBoxItemsSource();
        }

        /// <summary>
        /// Clears the current binding to listbox, and creates a new one
        /// </summary>
        private void RefreshListBoxItemsSource()
        {
            if (ShowArchived)
            {
                NotesListBox.ItemsSource = null;
                NotesListBox.ItemsSource = notes;
            }

            if (!ShowArchived)
            {
                NotesListBox.ItemsSource = null;
                NotesListBox.ItemsSource = notes.Where(x => x.IsArchived == false).ToList();
            }

            if (NotesListBox.Items.Count > 0)
                NotesListBox.SelectedItem = NotesListBox.Items[NotesListBox.Items.Count - 1];
        }

        private void ShowArchivedButton_Click(object sender, RoutedEventArgs e)
        {
            ShowArchived = !ShowArchived;

            if (ShowArchived) ShowArchivedButton.Content = "Hide Archived";
            if (!ShowArchived) ShowArchivedButton.Content = "Show Archived";

            RefreshListBoxItemsSource();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshListBoxItemsSource();
        }
    }
}
 