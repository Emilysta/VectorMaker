using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectorMaker.ViewModel;

namespace VectorMaker.Intefaces
{
    internal interface IMainWindowViewModel
    {
        event EventHandler ActiveCanvasChanged;

        DrawingCanvasViewModel ActiveCanvas { get; set; }

        IEnumerable<ToolBaseViewModel> Tools { get; }

        IEnumerable<DrawingCanvasViewModel> Documents { get; }

        void Close(DrawingCanvasViewModel fileToClose);

        void AddFile(DrawingCanvasViewModel fileToAdd);

        void Save(DrawingCanvasViewModel fileToSave);

        Task<DrawingCanvasViewModel> OpenAsync(string filepath);

        void CloseAllDocuments();
    }
}
