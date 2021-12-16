using System.Threading.Tasks;
using System.Windows.Input;

namespace VectorMaker.Intefaces
{
    /// <summary>
    /// Interface for all documents. It's a start base for <see cref="ViewModel.DocumentViewModelBase"/>.<br/>
    /// Contains: <br/>
    /// <list type="bullet">
    /// <item> <see cref="FileName"/> </item>
    /// <item> <see cref="FilePath"/> </item>
    /// <item> <see cref="IsSaved"/> </item>
    /// <item> <see cref="CloseCommand"/></item>
    /// <item> <see cref="SaveCommand"/></item>
    /// <item> <see cref="SaveAsCommand"/></item>
    /// </list>
    /// </summary>
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
