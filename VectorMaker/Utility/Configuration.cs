using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
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
        private bool m_isBorderVisible = true;
        private bool m_isBorderShadow = true;
        private bool m_isBackgroundCheckered = true;
        private ColorDef m_borderColor = ColorDef.FromRgb(96, 96, 96);
        private ColorDef m_backgroundColor = ColorDef.FromRgb(48, 48, 48);
        private ColorDef m_checkColor = ColorDef.FromRgb(20, 20, 20);

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
            }
        }
        public string Rights
        {
            get => m_rights;
            set
            {
                m_rights = value;
                OnPropertyChanged(nameof(Rights));
            }
        }
        public string Publisher
        {
            get => m_publisher;
            set
            {
                m_publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }
        public string Language
        {
            get => m_language;
            set
            {
                m_language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        public string Identifier
        {
            get => m_identifier;
            set
            {
                m_identifier = value;
                OnPropertyChanged(nameof(Identifier));
            }
        }
        public bool IsBorderVisible
        {
            get => m_isBorderVisible;
            set
            {
                m_isBorderVisible = value;
                OnPropertyChanged(nameof(IsBorderVisible));
            }
        }
        public bool IsBorderShadow
        {
            get => m_isBorderShadow;
            set
            {
                m_isBorderShadow = value;
                OnPropertyChanged(nameof(IsBorderShadow));
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
        public ColorDef BorderColor
        {
            get => m_borderColor;
            set
            {
                m_borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }
        public ColorDef BackgroundColor
        {
            get => m_backgroundColor;
            set
            {
                m_backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
        public ColorDef CheckColor
        {
            get => m_checkColor;
            set
            {
                m_checkColor = value;
                OnPropertyChanged(nameof(CheckColor));
            }
        }
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
            IsBorderVisible = true;
            IsBorderShadow = true;
            IsBackgroundCheckered = true;
            BorderColor = ColorDef.FromRgb(96, 96, 96);
            BackgroundColor = ColorDef.FromRgb(48, 48, 48);
            CheckColor = ColorDef.FromRgb(20, 20, 20);
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
