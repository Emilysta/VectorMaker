using System.Windows.Input;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand SaveasCommand { get; private set; }

        public MainWindowViewModel()
        {
            //SaveasCommand = new CommandBase();

        }
    }
}
