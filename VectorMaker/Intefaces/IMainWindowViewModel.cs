using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectorMaker.ViewModel;

namespace VectorMaker.Intefaces
{
    internal interface IMainWindowViewModel
    {
        event EventHandler ActiveCanvasChanged;

        DocumentViewModelBase ActiveDocument { get; set; }

        IEnumerable<ToolBaseViewModel> Tools { get; }

        IEnumerable<DocumentViewModelBase> Documents { get; }

        void Close(DocumentViewModelBase fileToClose);

        void AddFile(DocumentViewModelBase fileToAdd);

        void Save(DocumentViewModelBase fileToSave);

        Task<DocumentViewModelBase> OpenAsync(string filepath);

        void CloseAllDocuments();
    }
}
