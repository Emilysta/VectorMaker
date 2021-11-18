using System;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Utility;
using VectorMaker.Views;

namespace VectorMaker.ViewModel
{
    internal class AppSettingsViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        private AppSettingsView m_window;
        #endregion

        #region Properties
        public Configuration Model => Configuration.Instance;

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
            SetCommand();
            m_window = new AppSettingsView();
            m_window.Owner = Application.Current.MainWindow;
            m_window.DataContext = this;
            m_window.Show();

        }
        #endregion

        #region Methods
        private void SetCommand()
        {
            WindowMouseLeftButtonDownCommand = new CommandBase((obj) => DragSettingsWindow(obj as MouseButtonEventArgs));
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
            BorderColorPickCommand = new CommandBase((obj) => BorderColorPick());
            BackgroundColorPickCommand = new CommandBase((obj) => BackgroundColorPick());
            CheckColorPickCommand = new CommandBase((obj) => CheckColorPick());
        }

        private void DragSettingsWindow(MouseButtonEventArgs e)
        {
            try
            {
                if (m_window.IsMouseOver)
                {
                    m_window.DragMove();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //toDo check why exception was before if
            }
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

        private void BorderColorPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(Model.BorderColor);
            colorPicker.ShowWindowAndWaitForResult();
        }

        private void CheckColorPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(Model.CheckColor);
            colorPicker.ShowWindowAndWaitForResult();
        }

        private void BackgroundColorPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(Model.BackgroundColor);
            colorPicker.ShowWindowAndWaitForResult();
        }

        #endregion
    }
}
