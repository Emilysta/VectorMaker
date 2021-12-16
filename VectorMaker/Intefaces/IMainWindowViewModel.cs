using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VectorMaker.ViewModel;

namespace VectorMaker.Intefaces
{
    /// <summary>
    /// Interface for <see cref="ViewModel.MainWindowViewModel"/> that holds all documents and tools..<br/>
    /// Contains: <br/>
    /// <list type="bullet">
    /// <item> <see cref="ActiveCanvasChanged"/> </item>
    /// <item> <see cref="ActiveDocument"/> </item>
    /// <item> <see cref="Tools"/> </item>
    /// <item> <see cref="Documents"/></item>
    /// <item> <see cref="Save(DocumentViewModelBase)"/></item>
    /// <item> <see cref="CloseAllDocuments"/></item>
    /// <item> <see cref="CloseTool(ToolBaseViewModel)"/></item>
    /// </list>
    /// </summary>
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
