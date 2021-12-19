using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;

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
        private Visibility m_blendVisibiity;
        private Adorner m_adorner = null;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        private double m_objectRight = 0;
        private double m_objectBottom = 0;
        private double m_objectTop = 0;
        private double m_objectLeft = 0;
        private double m_canvasWidth = 0;
        private double m_canvasHeight = 0;
        private double m_objectHalfWidth = 0;
        private double m_objectHalfHeight = 0;
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

        public Visibility BlendVisibility
        {
            get => m_blendVisibiity;
            set
            {
                m_blendVisibiity = value;
                OnPropertyChanged("BlendVisibility");
            }
        }
        private bool IsOneObjectSelected => m_selectedObjects?.Count == 1;

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
            if (m_interfaceMainWindowVM.ActiveDocument is DrawingCanvasViewModel drawingCanvasViewModel)
            {
                if (m_selectedObjects != null)
                    m_selectedObjects.CollectionChanged -= SelectedObjectsCollectionChanged;

                m_selectedObjects = drawingCanvasViewModel.SelectedObjects;
                m_selectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
                m_canvasWidth = drawingCanvasViewModel.Data.Width;
                m_canvasHeight = drawingCanvasViewModel.Data.Height;
            }
        }

        private void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsOneObjectSelected)
                BlendVisibility = Visibility.Hidden;
            else
                BlendVisibility = Visibility.Visible;

            if (m_selectedObjects.Count > 0)
                m_adorner = m_selectedObjects[0];
            else
                m_adorner = null;
            SetValuesForAlignment();
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
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.Y = m_objectTop;
                OverrideTransform(transform,element);
            }
        }

        private void AlignBottom()
        {
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.Y = m_objectBottom - element.RenderSize.Height;
                OverrideTransform(transform, element);
            }
        }

        private void AlignLeft()
        {
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.X = m_objectLeft;
                OverrideTransform(transform, element);
            }
        }

        private void AlignRight()
        {
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.X = m_objectRight - element.RenderSize.Width;
                OverrideTransform(transform, element);
            }
        }

        private void AlignCenterVertical()
        {
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.Y = m_objectBottom - m_objectHalfHeight - element.RenderSize.Height / 2;
                OverrideTransform(transform, element);
            }
        }

        private void AlignCenterHorizontal()
        {
            foreach (ResizingAdorner obj in m_selectedObjects)
            {
                UIElement element = obj.AdornedElement;
                TranslateTransform transform = CalculateAlignment(element);
                transform.X = m_objectRight - m_objectHalfWidth - element.RenderSize.Width / 2;
                OverrideTransform(transform, element);
            }
        }

        private void DistributeHorizontal()
        {
            //Trace.WriteLine("NotImplemented - distruibte Horizontal");
        }

        private void DistributeVertical()
        {
            //Trace.WriteLine("NotImplemented - distruibte Vertical");
        }

        private void AlignRelativeToPage()
        {
            IsFirstToggleButtonChecked = false;
            IsLastToggleButtonChecked = false;
            m_alignRelative = AlignRelative.Page;
            SetValuesForAlignment();
        }

        private void AlignRelativeToLast()
        {
            IsFirstToggleButtonChecked = false;
            IsPageToggleButtonChecked = false;
            m_alignRelative = AlignRelative.Last;
            SetValuesForAlignment();
        }

        private void AlignRelativeToFirst()
        {
            IsLastToggleButtonChecked = false;
            IsPageToggleButtonChecked = false;
            m_alignRelative = AlignRelative.First;
            SetValuesForAlignment();
        }

        private TranslateTransform CalculateAlignment(UIElement element)
        {
            Matrix matrix = element.RenderTransform.Value;
            TranslateTransform translate = new TranslateTransform();
            translate.X = matrix.OffsetX;
            translate.Y = matrix.OffsetY;
            return translate;
        }

        private void OverrideTransform(TranslateTransform translate,UIElement element)
        {
            Matrix matrix = element.RenderTransform.Value;
            matrix.OffsetX = translate.X;
            matrix.OffsetY = translate.Y;
            element.RenderTransform = new MatrixTransform(matrix);
        }

        private void SetValuesForAlignment()
        {
            switch (m_alignRelative)
            {
                case AlignRelative.Page:
                    {
                        SetAlignmentToPage();
                        break;
                    }
                case AlignRelative.First:
                    {
                        UIElement element = m_selectedObjects?.First()?.AdornedElement;
                        if(element != null)
                            SetAlignmentInRealtiveToUIElement(element); 
                        else 
                            goto default;
                        break;
                    }
                case AlignRelative.Last:
                    {
                        UIElement element = m_selectedObjects?.Last()?.AdornedElement;
                        if (element != null)
                            SetAlignmentInRealtiveToUIElement(element);
                        else
                            goto default;
                        break;
                    }
                default:
                    {
                        SetAlignmentToPage();
                        break;
                    }
            }
        }

        private void SetAlignmentInRealtiveToUIElement(UIElement element)
        {
            Transform transform = element.RenderTransform;
            if (transform != null)
            {
                m_objectLeft = transform.Value.OffsetX;
                m_objectTop = transform.Value.OffsetY;
                m_objectBottom = transform.Value.OffsetY + element.RenderSize.Height;
                m_objectRight = transform.Value.OffsetX + element.RenderSize.Width;
                m_objectHalfHeight = element.RenderSize.Height/2;
                m_objectHalfWidth = element.RenderSize.Width/2;
            }
        }

        private void SetAlignmentToPage()
        {
            m_objectLeft = 0;
            m_objectTop = 0;
            m_objectBottom = m_canvasHeight;
            m_objectRight = m_canvasWidth;
            m_objectHalfHeight = m_canvasHeight / 2;
            m_objectHalfWidth = m_canvasWidth / 2;
        }
        #endregion
    }
}
