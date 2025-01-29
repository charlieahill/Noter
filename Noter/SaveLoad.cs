using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Noter
{
    public static class SaveLoad
    {
        /// <summary>
        /// Saves the list of notes to file
        /// </summary>
        /// <param name="notes">The list of note models to be saved</param>
        /// <param name="notFilename">The full filepath of the file containing these notes</param>
        public static void Save(this BindingList<NoteModel> notes, string notFilename)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(notFilename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(notFilename));

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BindingList<NoteModel>));
                using (StreamWriter writer = new StreamWriter(notFilename))
                    xmlSerializer.Serialize(writer, notes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save notes data. Changes to the day data made in this session will be lost." + Environment.NewLine + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Loads notes data from file
        /// </summary>
        /// <param name="notFilename">The filename of the file to load</param>
        /// <returns>The loaded program data</returns>
        public static BindingList<NoteModel> Load(string notFilename)
        {
            try
            {
                if (!File.Exists(notFilename)) return new BindingList<NoteModel>();

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BindingList<NoteModel>));
                using (StreamReader reader = new StreamReader(notFilename))
                    return (BindingList<NoteModel>)xmlSerializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load note data. A blank data model will be loaded in for this session." + Environment.NewLine + Environment.NewLine + ex.ToString());
                return new BindingList<NoteModel>();
            }
        }
    }
}
