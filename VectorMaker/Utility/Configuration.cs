using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ColorDef = System.Windows.Media.Color;

namespace VectorMaker.Utility
{
    internal class Configuration : NotifyPropertyChangedBase
    {
        #region Fields
        private string m_author = "";
        private string m_rights = "";
        private string m_publisher = "";
        private string m_language = "";
        private string m_identifier = "";
        private string m_title = "";
        private bool m_isDateToSave = false;
        private bool m_isBorderVisible = true;
        private bool m_isBorderShadow = true;
        private bool m_isBackgroundCheckered = true;
        private SolidColorBrush m_borderColor = new SolidColorBrush(ColorDef.FromRgb(96, 96, 96));
        private SolidColorBrush m_backgroundColor = new SolidColorBrush(ColorDef.FromRgb(48, 48, 48));
        private SolidColorBrush m_checkColor = new SolidColorBrush(ColorDef.FromRgb(30, 30, 30));
        private Metadata m_metadata = new Metadata();
        [JsonIgnore]
        private const string CONFIG_FILE_PATH = "/VectorMaker.config";
        [JsonIgnore]
        private string m_configFilePath => Environment.SpecialFolder.LocalApplicationData + CONFIG_FILE_PATH;
        #endregion

        #region Properties
        public string Author
        {
            get => m_author;
            set
            {
                m_author = value;
                OnPropertyChanged(nameof(Author));
                m_metadata.Author.Value = value;
            }
        }
        public string Rights
        {
            get => m_rights;
            set
            {
                m_rights = value;
                OnPropertyChanged(nameof(Rights));
                m_metadata.Rights.Value = value;
            }
        }
        public string Publisher
        {
            get => m_publisher;
            set
            {
                m_publisher = value;
                OnPropertyChanged(nameof(Publisher));
                m_metadata.Publisher.Value = value;
            }
        }
        public string Language
        {
            get => m_language;
            set
            {
                m_language = value;
                OnPropertyChanged(nameof(Language));
                m_metadata.Language.Value = value;
            }
        }
        public string Identifier
        {
            get => m_identifier;
            set
            {
                m_identifier = value;
                OnPropertyChanged(nameof(Identifier));
                m_metadata.Identifier.Value = value;
            }
        }
        public string Title
        {
            get => m_title;
            set
            {
                m_title = value;
                OnPropertyChanged(nameof(Title));
                m_metadata.Title.Value = value;
            }
        }
        public bool IsDateToSave
        {
            get => m_isDateToSave;
            set
            {
                m_isDateToSave = value;
                OnPropertyChanged(nameof(IsDateToSave));
                m_metadata.SetDateInMetadata(value);
            }
        }
        public bool IsBorderVisible
        {
            get => m_isBorderVisible;
            set
            {
                m_isBorderVisible = value;
                OnPropertyChanged(nameof(IsBorderVisible));
                OnPropertyChanged(nameof(BorderVisibility));
            }
        }
        public bool IsBorderShadow
        {
            get => m_isBorderShadow;
            set
            {
                m_isBorderShadow = value;
                OnPropertyChanged(nameof(IsBorderShadow));
                OnPropertyChanged(nameof(CheckColorVisibility));
            }
        }
        public bool IsBackgroundCheckered
        {
            get => m_isBackgroundCheckered;
            set
            {
                m_isBackgroundCheckered = value;
                OnPropertyChanged(nameof(IsBackgroundCheckered));
            }
        }
        public SolidColorBrush BorderColor
        {
            get => m_borderColor;
            set
            {
                m_borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }
        public SolidColorBrush BackgroundColor
        {
            get => m_backgroundColor;
            set
            {
                m_backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
        public SolidColorBrush CheckColor
        {
            get => m_checkColor;
            set
            {
                m_checkColor = value;
                OnPropertyChanged(nameof(CheckColor));
            }
        }
        public Visibility BorderVisibility => IsBorderVisible ? Visibility.Visible : Visibility.Hidden;
        public Visibility CheckColorVisibility => IsBackgroundCheckered ? Visibility.Visible : Visibility.Hidden;
        public Visibility BorderShadowVisibility => IsBorderShadow ? Visibility.Visible : Visibility.Hidden;
        #endregion
        private static Configuration m_instance;
        public static Configuration Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Configuration();
                return m_instance;
            }
        }
        private Configuration()
        {
            LoadConfigIfExists();
        }

        #region Methods
        public void SaveToFile()
        {
            string value = JsonConvert.SerializeObject(this, Formatting.Indented);
            Trace.WriteLine(value); //toDo comment out
            FileStream fileStream = new FileStream(m_configFilePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(value);
            }
        }
        public void LoadConfigIfExists()
        {
            if (!File.Exists(m_configFilePath))
            {
                CreateFile();
            }
            else
            {
                FileStream fileStream = File.OpenRead(m_configFilePath);
                string fileContents;
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContents = reader.ReadToEnd();
                }

                JsonConvert.PopulateObject(fileContents, this);
            }
        }
        public void ResetToDefault()
        {
            Author = "";
            Rights = "";
            Publisher = "";
            Language = "";
            Identifier = "";
            IsDateToSave = false;
            IsBorderVisible = true;
            IsBorderShadow = true;
            IsBackgroundCheckered = true;
            BorderColor = new SolidColorBrush(ColorDef.FromRgb(96, 96, 96));
            BackgroundColor = new SolidColorBrush(ColorDef.FromRgb(48, 48, 48));
            CheckColor = new SolidColorBrush(ColorDef.FromRgb(30, 30, 30));
        }
        public string GetMetadataFromConfig()
        {
            m_metadata.Date.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return m_metadata.MetaData.ToString();
        }
        private void CreateFile()
        {
            string value = JsonConvert.SerializeObject(this, Formatting.Indented);
            Trace.WriteLine(value); //toDo comment out
            Directory.CreateDirectory(Environment.SpecialFolder.LocalApplicationData.ToString());
            FileStream fileStream = new FileStream(m_configFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(value);
            }
        }
        #endregion
    }
}
