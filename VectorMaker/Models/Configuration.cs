using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ColorDef = System.Windows.Media.Color;

namespace VectorMaker.Utility
{
    /// <summary>
    /// Class in singleton pattern. Hold configuration of whole app. Derives from <see cref="NotifyPropertyChangedBase"/>.
    /// It loads config file if it exists else it creates one.
    /// </summary>
    internal class Configuration : NotifyPropertyChangedBase
    {
        #region Fields
        private string m_author = "";
        private string m_rights = "";
        private string m_publisher = "";
        private string m_language = "";
        private string m_identifier = "";
        private bool m_isDateToSave = false;
        private bool m_isBorderVisible = true;
        private bool m_isBorderShadow = true;
        private bool m_isBackgroundCheckered = true;
        private SolidColorBrush m_borderColor = new SolidColorBrush(ColorDef.FromRgb(96, 96, 99));
        private SolidColorBrush m_backgroundColor = new SolidColorBrush(ColorDef.FromRgb(48, 48, 51));
        private SolidColorBrush m_checkColor = new SolidColorBrush(ColorDef.FromRgb(30, 30, 33));
        private GeneralMetadata m_metadata = new GeneralMetadata();
        [JsonIgnore]
        private const string CONFIG_FILE_PATH = "\\VectorMaker.config";
        [JsonIgnore]
        private string m_configFilePath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + CONFIG_FILE_PATH;
        #endregion

        #region Properties
        /// <summary>
        /// Author name property
        /// </summary>
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
        /// <summary>
        /// Rights string value property
        /// </summary>
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
        /// <summary>
        /// Publisher string value property
        /// </summary>
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
        /// <summary>
        /// Language string value property
        /// </summary>
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
        /// <summary>
        /// Identifier string value property
        /// </summary>
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
        /// <summary>
        /// IsDateToSave bool value property
        /// </summary>
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

        /// <summary>
        /// IsBorderVisible bool value property
        /// </summary>
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
        /// <summary>
        /// IsBorderShadow bool value property
        /// </summary>
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
        /// <summary>
        /// IsBackgroundCheckered bool value property
        /// </summary>
        public bool IsBackgroundCheckered
        {
            get => m_isBackgroundCheckered;
            set
            {
                m_isBackgroundCheckered = value;
                OnPropertyChanged(nameof(IsBackgroundCheckered));
            }
        }

        /// <summary>
        /// Property holding color of drawing area border.
        /// </summary>
        public SolidColorBrush BorderColor
        {
            get => m_borderColor;
            set
            {
                m_borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }

        /// <summary>
        /// Property holding color of whole document background.
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get => m_backgroundColor;
            set
            {
                m_backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        /// <summary>
        /// Property holding color of checkered background.
        /// </summary>
        public SolidColorBrush CheckColor
        {
            get => m_checkColor;
            set
            {
                m_checkColor = value;
                OnPropertyChanged(nameof(CheckColor));
            }
        }

        /// <summary>
        /// Property holding <see cref="Visibility"/> of drawing area border.
        /// </summary>
        public Visibility BorderVisibility => IsBorderVisible ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// Property holding <see cref="Visibility"/> of checkered background.
        /// </summary>
        public Visibility CheckColorVisibility => IsBackgroundCheckered ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// Property holding <see cref="Visibility"/> of drawing area border shadow.
        /// </summary>
        public Visibility BorderShadowVisibility => IsBorderShadow ? Visibility.Visible : Visibility.Hidden;
        #endregion
        private static Configuration m_instance;

        /// <summary>
        /// Implements singleton property.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Configuration();
                return m_instance;
            }
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        private Configuration()
        {
            //Trace.WriteLine(m_configFilePath);
            LoadConfigIfExists();
        }

        #region Methods

        /// <summary>
        /// Method that saves config file
        /// </summary>
        public void SaveToFile()
        {
            string value = JsonConvert.SerializeObject(this, Formatting.Indented);
            //.WriteLine(value); //toDo comment out
            //Trace.WriteLine(m_configFilePath);
            FileStream fileStream = new FileStream(m_configFilePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Method that loads config file if it exists
        /// </summary>
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

        /// <summary>
        /// Method that reset config to it's default state
        /// </summary>
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
            BorderColor = new SolidColorBrush(ColorDef.FromRgb(96, 96, 99));
            BackgroundColor = new SolidColorBrush(ColorDef.FromRgb(48, 48, 51));
            CheckColor = new SolidColorBrush(ColorDef.FromRgb(30, 30, 33));
        }

        /// <summary>
        /// Method that gets metadata from config file<br/>
        /// <returns>Returns: metadata as string in XML format </returns>
        /// </summary>
        public string GetMetadataFromConfig()
        {
            m_metadata.Date.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return m_metadata.MetaData.ToString();
        }

        /// <summary>
        /// Method that creates config file
        /// </summary>
        private void CreateFile()
        {
            string value = JsonConvert.SerializeObject(this, Formatting.Indented);
            //Trace.WriteLine(value); //toDo comment out
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
