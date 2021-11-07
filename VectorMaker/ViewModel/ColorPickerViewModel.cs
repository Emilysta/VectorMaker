using ColorDef = System.Windows.Media.Color;
using System.Windows.Media;
using VectorMaker.Utility;
using System.Windows.Input;
using VectorMaker.Commands;
using System.Windows;
using VectorMaker.Views;

namespace VectorMaker.ViewModel
{
    internal class ColorPickerViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        private ColorDef m_selectedColor = Colors.Black;
        private ColorDef m_startColor = Colors.Black;
        private bool m_isDefaultPaletteChecked = true;
        private bool m_isCustomPaletteChecked => !m_isDefaultPaletteChecked;

        private SolidColorBrush m_brushToEdit;
        #endregion

        #region Properties
        public ColorDef SelectedColor
        {
            get => m_selectedColor;
            set
            {
                m_selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
                m_brushToEdit.Color = m_selectedColor;
                
            }
        }

        public bool IsDefaultPaletteChecked
        {
            get => m_isDefaultPaletteChecked;
            set
            {
                m_isDefaultPaletteChecked = value;
                OnPropertyChanged(nameof(IsDefaultPaletteChecked));
                OnPropertyChanged(nameof(IsCustomPaletteChecked));
                OnPropertyChanged(nameof(DefaultPaletteVisibility));
                OnPropertyChanged(nameof(CustomPaletteVisibility));
            }
        }
        public bool IsCustomPaletteChecked
        {
            get => m_isCustomPaletteChecked;
            set
            {
                IsDefaultPaletteChecked = !value;
                OnPropertyChanged(nameof(IsDefaultPaletteChecked));
                OnPropertyChanged(nameof(IsCustomPaletteChecked));
                OnPropertyChanged(nameof(DefaultPaletteVisibility));
                OnPropertyChanged(nameof(CustomPaletteVisibility));
            }
        }

        public Visibility DefaultPaletteVisibility => IsDefaultPaletteChecked ? Visibility.Visible : Visibility.Hidden;
        public Visibility CustomPaletteVisibility => IsCustomPaletteChecked ? Visibility.Visible : Visibility.Hidden;

        #endregion

        #region Commands
        public ICommand WindowMouseLeftButtonDownCommand { get; set; }
        public ICommand CloseColorPickerWindowCommand { get; set; }
        public ICommand ResetColorCommand { get; set; }
        public ICommand ApplyColorCommand { get; set; }
        public ICommand DefaultPaletteCheckedCommand { get; set; }
        public ICommand CustomPaletteCheckedCommand { get; set; }
        #endregion

        #region Constructors
        public ColorPickerViewModel()
        {
            SetCommands();
        }

        public ColorPickerViewModel(Brush brush)
        {
            m_brushToEdit = (SolidColorBrush)brush;
            SelectedColor = m_brushToEdit.Color;
            m_startColor = m_brushToEdit.Color;
            SetCommands();
        }
        #endregion

        #region Methods
        public void ShowWindowAndWaitForResult()
        {
            Window window = new ColorPickerView();
            window.DataContext = this;
            window.ShowDialog();
        }
        private void SetCommands()
        {
            WindowMouseLeftButtonDownCommand = new CommandBase((obj) => { });
            CloseColorPickerWindowCommand = new CommandBase((obj) => ApplyColor(obj as Window));
            ResetColorCommand = new CommandBase((obj) => ResetColor(obj as Window));
            ApplyColorCommand = new CommandBase((obj) => ApplyColor(obj as Window));
            DefaultPaletteCheckedCommand = new CommandBase((obj) => DefaultPaletteClick());
            CustomPaletteCheckedCommand = new CommandBase((obj) => CustomPaletteClick());
        }
        private void ResetColor(Window window)
        {
            SelectedColor = m_startColor;
        }
        private void ApplyColor(Window window)
        {
            window.Close();
        }
        private void DefaultPaletteClick()
        {
            IsDefaultPaletteChecked = !m_isDefaultPaletteChecked;
        }
        private void CustomPaletteClick()
        {
            IsCustomPaletteChecked = !m_isCustomPaletteChecked;
        }
        #endregion
    }
}
