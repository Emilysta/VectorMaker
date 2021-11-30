using System;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;

namespace VectorMaker.ViewModel
{
    internal class FileSettingPopupViewModel 
    {
        #region Fields
        private DrawingCanvasViewModel m_drawing = null;
        #endregion

        #region Properties

        public DrawingCanvasViewModel Drawing { get { return m_drawing; } }

        #endregion

        #region Commands

        public ICommand BackgroundPickCommand { get; set; }
        public ICommand MetaWindowOpenCommand { get; set; }

        #endregion

        #region Constructors
        public FileSettingPopupViewModel( DrawingCanvasViewModel drawingCanvasViewModel) 
        {
            m_drawing = drawingCanvasViewModel;
            SetCommands();
        }

        #endregion

        #region Methods

        private void SetCommands()
        {
            BackgroundPickCommand = new CommandBase((_) => BackgroundPick());
            MetaWindowOpenCommand = new CommandBase((_) => throw new NotImplementedException());
        }

        private void BackgroundPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(Drawing.Data.Background);
            colorPicker.ShowWindowAndWaitForResult();
        }
        #endregion
    }
}
