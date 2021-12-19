using System.Windows;
namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for DialogBoxWithText.xaml
    /// </summary>
    public partial class DialogBoxWithText : Window
    {
        public DialogBoxWithText(string text)
        {
            InitializeComponent();
            TextBlockObject.Text = text;
            this.Show();
        }
    }
}
