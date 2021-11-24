using System.Collections.Specialized;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;

namespace VectorMaker.ViewModel
{

    internal class ObjectAlignmentViewModel : ToolBaseViewModel
    {
        enum AlignRelative
        {
            Page,
            Last,
            First
        }

        #region Fields
        private AlignRelative m_alignRelative = AlignRelative.Page;
        private bool m_isFirstToggleButtonChecked = false;
        private bool m_isLastToggleButtonChecked = false;
        private bool m_isPageToggleButtonChecked = true;
        #endregion

        #region Properties
        public bool IsFirstToggleButtonChecked
        {
            get { return m_isFirstToggleButtonChecked; }
            set
            {
                m_isFirstToggleButtonChecked = value;
                OnPropertyChanged(nameof(IsFirstToggleButtonChecked));
            }
        }

        public bool IsLastToggleButtonChecked
        {
            get { return m_isLastToggleButtonChecked; }
            set
            {
                m_isLastToggleButtonChecked = value;
                OnPropertyChanged(nameof(IsLastToggleButtonChecked));
            }
        }

        public bool IsPageToggleButtonChecked
        {
            get { return m_isPageToggleButtonChecked; }
            set
            {
                m_isPageToggleButtonChecked = value;
                OnPropertyChanged(nameof(IsPageToggleButtonChecked));
            }
        }

        protected override string m_title { get; set; } = "Alignment";
        #endregion

        #region Commands
        public ICommand AlignTopCommand { get; set; }
        public ICommand AlignBottomCommand { get; set; }
        public ICommand AlignLeftCommand { get; set; }
        public ICommand AlignRightCommand { get; set; }
        public ICommand AlignCenterVerticalCommand { get; set; }
        public ICommand AlignCenterHorizontalCommand { get; set; }
        public ICommand DistributeHorizontalCommand { get; set; }
        public ICommand DistributeVerticalCommand { get; set; }
        public ICommand AlignRelativeToPageCommand { get; set; }
        public ICommand AlignRelativeToLastCommand { get; set; }
        public ICommand AlignRelativeToFirstCommand { get; set; }
        #endregion

        #region Constructors

        public ObjectAlignmentViewModel(IMainWindowViewModel interfaceMainWindowVM) : base(interfaceMainWindowVM)
        {
            SetCommands();
        }

        #endregion Constructors

        #region EventHandlers

        public override void OnActiveCanvasChanged(object sender, System.EventArgs e)
        {
            //if (m_interfaceMainWindowVM.ActiveDocument is DrawingCanvasViewModel drawingCanvasViewModel)
            //{
            //    if (m_selectedObjects != null)
            //        m_selectedObjects.CollectionChanged -= SelectedObjectsCollectionChanged;

            //    m_selectedObjects = drawingCanvasViewModel.SelectedObjects;
            //    m_selectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
            //}
            //m_selectedObjects = m_interfaceMainWindowVM.ActiveCanvas.SelectedObjects;
            //m_selectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
        }

        private void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (IsOneObjectSelected)
            //    BlendVisibility = Visibility.Hidden;
            //else
            //    BlendVisibility = Visibility.Visible;

            //if (m_selectedObjects.Count > 0)
            //    m_adorner = m_selectedObjects[0];
            //else
            //    m_adorner = null;
            //m_transform = m_adorner?.AdornedElement.RenderTransform;
        }

        #endregion

        #region Methods

        private void SetCommands()
        {
            AlignTopCommand = new CommandBase((obj) => AlignTop());
            AlignBottomCommand = new CommandBase((obj) => AlignBottom());
            AlignLeftCommand = new CommandBase((obj) => AlignLeft());
            AlignRightCommand = new CommandBase((obj) => AlignRight());
            AlignCenterVerticalCommand = new CommandBase((obj) => AlignCenterVertical());
            AlignCenterHorizontalCommand = new CommandBase((obj) => AlignCenterHorizontal());
            DistributeHorizontalCommand = new CommandBase((obj) => DistributeHorizontal());
            DistributeVerticalCommand = new CommandBase((obj) => DistributeVertical());
            AlignRelativeToPageCommand = new CommandBase((obj) => AlignRelativeToPage());
            AlignRelativeToLastCommand = new CommandBase((obj) => AlignRelativeToLast());
            AlignRelativeToFirstCommand = new CommandBase((obj) => AlignRelativeToFirst());
        }

        private void AlignTop()
        {

        }

        private void AlignBottom()
        {

        }

        private void AlignLeft()
        {

        }

        private void AlignRight()
        {

        }

        private void AlignCenterVertical()
        {

        }

        private void AlignCenterHorizontal()
        {

        }

        private void DistributeHorizontal()
        {

        }

        private void DistributeVertical()
        {

        }

        private void AlignRelativeToPage()
        {
            IsFirstToggleButtonChecked = false;
            IsLastToggleButtonChecked = false;
            m_alignRelative = AlignRelative.Page;
        }

        private void AlignRelativeToLast()
        {
            IsFirstToggleButtonChecked = false;
            IsPageToggleButtonChecked = false;
            m_alignRelative = AlignRelative.Last;
        }

        private void AlignRelativeToFirst()
        {
            IsLastToggleButtonChecked = false;
            IsPageToggleButtonChecked = false;
            m_alignRelative = AlignRelative.First;
        }
        #endregion
    }
}
