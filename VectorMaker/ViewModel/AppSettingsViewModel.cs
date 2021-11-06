using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class AppSettingsViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        #endregion

        #region Properties
        public Configuration Model => Configuration.Instance;
        public Visibility BorderVisibility => Model.IsBorderVisible ? Visibility.Visible : Visibility.Hidden;
        public Visibility CheckColorVisibility => Model.IsBackgroundCheckered ? Visibility.Visible : Visibility.Hidden;
        #endregion

        #region Commands
        public ICommand CloseSettingsWindowCommand { get; set; }
        public ICommand WindowMouseLeftButtonDownCommand { get; set; }
        public ICommand SaveSettingsCommand { get; set; }
        public ICommand ResetSettingsCommand { get; set; }
        public ICommand ResetToDefaultSettingsCommand { get; set; }
        public ICommand CheckeredBgOnCommand { get; set; }
        public ICommand CheckeredBgOffCommand { get; set; }
        public ICommand BorderVisiblityOnCommand { get; set; }
        public ICommand BorderVisiblityOffCommand { get; set; }
        public ICommand BorderShadowOnCommand { get; set; }
        public ICommand BorderShadowOffCommand { get; set; }
        public ICommand BorderColorPickCommand { get; set; }
        public ICommand BackgroundColorPickCommand { get; set; }
        public ICommand CheckColorPickCommand { get; set; }
        #endregion

        #region Constructors
        public AppSettingsViewModel()
        {
            Model.PropertyChanged += ModelPropertyChanged;
            SetCommand();
        }
        #endregion

        #region Methods
        private void SetCommand()
        {
            WindowMouseLeftButtonDownCommand = new CommandBase((obj) => DragSettingsWindow(obj as Window));
            CloseSettingsWindowCommand = new CommandBase((obj) => CloseSettingsWindow(obj as Window));
            SaveSettingsCommand = new CommandBase((obj) => SaveSettings());
            ResetSettingsCommand = new CommandBase((obj) => ResetSetting());
            ResetToDefaultSettingsCommand = new CommandBase((obj) => ResetSettingsToDefault());
            CheckeredBgOnCommand = new CommandBase((obj) => { });
            CheckeredBgOffCommand = new CommandBase((obj) => { });
            BorderVisiblityOnCommand = new CommandBase((obj) => { });
            BorderVisiblityOffCommand = new CommandBase((obj) => { });
            BorderShadowOnCommand = new CommandBase((obj) => { });
            BorderShadowOffCommand = new CommandBase((obj) => { });
            BorderColorPickCommand = new CommandBase((obj) => { });
            BackgroundColorPickCommand = new CommandBase((obj) => { });
            CheckColorPickCommand = new CommandBase((obj) => { });
        }

        private void DragSettingsWindow(Window window)
        {
            window.DragMove();
        }

        private void CloseSettingsWindow(Window window)
        {
            //toDo PopUP before closing
            window.Close();
        }

        private void SaveSettings()
        {
            Model.SaveToFile();
        }

        private void ResetSetting()
        {
            Model.LoadConfigIfExists();
        }

        private void ResetSettingsToDefault()
        {
            Model.ResetToDefault();
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.IsBorderVisible))
            {
                OnPropertyChanged(nameof(BorderVisibility));
            }
            if (e.PropertyName == nameof(Model.IsBackgroundCheckered))
            {
                OnPropertyChanged(nameof(CheckColorVisibility));
            }
        }
        #endregion
    }
}
