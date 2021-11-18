using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VectorMaker.ViewModel;

namespace VectorMaker.Intefaces
{
    internal interface IMainWindowViewModel
    {
        event EventHandler ActiveCanvasChanged;

        DocumentViewModelBase ActiveDocument { get; set; }

        ObservableCollection<ToolBaseViewModel> Tools { get; }

        IEnumerable<DocumentViewModelBase> Documents { get; }

        void Close(DocumentViewModelBase fileToClose);

        void AddFile(DocumentViewModelBase fileToAdd);

        void Save(DocumentViewModelBase fileToSave);

        void CloseAllDocuments();

        void CloseTool(ToolBaseViewModel tool);
    }
}
