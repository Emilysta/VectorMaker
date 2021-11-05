using System.Threading.Tasks;
using System.Windows.Input;

namespace VectorMaker.Intefaces
{
    internal interface IDocumentViewModel
    {
        string FileName { get; }

        string FilePath { get; }

        bool IsSaved { get; set; }

        ICommand CloseCommand { get; }

        ICommand SaveCommand { get; }

        ICommand SaveAsCommand { get; }
    }
}
