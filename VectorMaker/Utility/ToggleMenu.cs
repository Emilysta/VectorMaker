using MahApps.Metro.IconPacks;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.ViewModel;

namespace VectorMaker.Utility
{
    public class ToggleButtonForMenu : NotifyPropertyChangedBase
    {
        public Action ButtonAction;
        public event Action OnClick;

        private PackIconMaterialKind m_toggleIconKind;
        private bool m_isChecked;
        private bool m_isToggle;
        private string m_toolTip;

        public ICommand ClickCommand { get; set; }
        public bool IsToggle => m_isToggle;

        public PackIconMaterialKind ToggleIconKind
        {
            get => m_toggleIconKind;
            set
            {
                m_toggleIconKind = value;
                OnPropertyChanged(nameof(ToggleIconKind));
            }
        }
        public bool IsChecked
        {
            get => m_isChecked;
            set
            {
                m_isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string ToolTip => m_toolTip;

        public ToggleButtonForMenu(PackIconMaterialKind kind, Action clickAction, bool isChecked = false, bool isToggleButton = true, string toolTip = "")
        {
            ToggleIconKind = kind;
            ButtonAction = clickAction;
            IsChecked = isChecked;
            m_isToggle = isToggleButton;
            ClickCommand = new CommandBase((_) => OnClick?.Invoke());
            m_toolTip = toolTip;
        }
    }
    internal class ToggleMenu
    {
        private ObservableCollection<ToggleButtonForMenu> m_toggleButtonsKinds;

        public ObservableCollection<ToggleButtonForMenu> ToggleButtonKinds => m_toggleButtonsKinds;

        public ICommand ToggleClickCommand { get; set; }

        public ToggleMenu()
        {
            m_toggleButtonsKinds = new ObservableCollection<ToggleButtonForMenu>();
            ToggleClickCommand = new CommandBase((obj) => ToggleClick(obj));
        }

        public void AddNewButton(ToggleButtonForMenu button)
        {
            m_toggleButtonsKinds.Add(button);
            button.OnClick += () => ToggleClick(button);
        }

        private void ToggleClick(object sender)
        {
            ToggleButtonForMenu button = sender as ToggleButtonForMenu;
            if (!button.IsChecked)
            {
                foreach (ToggleButtonForMenu buttonForMenu in ToggleButtonKinds)
                {
                    buttonForMenu.IsChecked = false;
                }
                if (button.IsToggle)
                    button.IsChecked = true;
                button.ButtonAction?.Invoke();
            }
        }
    }
}
