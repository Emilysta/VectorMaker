using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using VectorMaker.Utility;
using ColorDef = System.Windows.Media.Color;

namespace VectorMaker.Models
{
    internal class ShapeProperties : NotifyPropertyChangedBase
    {
        #region Fields
        private SolidColorBrush m_fillBrush = new SolidColorBrush(ColorDef.FromRgb(0, 0, 0));
        private SolidColorBrush m_strokeBrush = new SolidColorBrush(ColorDef.FromRgb(255, 0, 0));
        private double m_strokeThickness = 1;
        private int m_selectedStrokeDashArray = 0;
        private ObservableCollection<DoubleCollection> m_strokeTypes;
        #endregion

        #region Properties
        public SolidColorBrush FillBrush 
        {
            get => m_fillBrush;
            set
            {
                m_fillBrush = value;
                OnPropertyChanged(nameof(FillBrush));
            }
        }

        public SolidColorBrush StrokeBrush
        {
            get => m_strokeBrush;
            set
            {
                m_strokeBrush = value;
                OnPropertyChanged(nameof(StrokeBrush));
            }
        }

        public double StrokeThickness
        {
            get => m_strokeThickness;
            set
            {
                m_strokeThickness = value;
                OnPropertyChanged(nameof(StrokeThickness));
            }
        }

        public int SelectedStrokeDashArray
        {
            get => m_selectedStrokeDashArray;
            set
            {
                m_selectedStrokeDashArray = value;
                OnPropertyChanged(nameof(SelectedStrokeDashArray));
            }
        }
        public DoubleCollection StrokeDashArray
        {
            get => StrokeTypes?[m_selectedStrokeDashArray] ?? null;
        }

        public ObservableCollection<DoubleCollection> StrokeTypes
        {
            get => m_strokeTypes;
            set
            {
                m_strokeTypes = value;
                OnPropertyChanged(nameof(StrokeTypes));
            }
        }
        #endregion

        #region Singleton
        private static ShapeProperties m_instance;
        public static ShapeProperties Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ShapeProperties();
                return m_instance;
            }
        }
        #endregion

        public ShapeProperties()
        {
            m_strokeTypes = new ObservableCollection<DoubleCollection>()
            {
                new DoubleCollection(new List<double>(){1,1} ),
                new DoubleCollection(new List<double>(){1,6} ),
                new DoubleCollection(new List<double>(){6,1} ),
                new DoubleCollection(new List<double>(){0.25,1} ),
                new DoubleCollection(new List<double>(){4,1,1,1,1,1} ),
                new DoubleCollection(new List<double>(){5,5,1,5} ),
                new DoubleCollection(new List<double>(){1,2,4} ),
                new DoubleCollection(new List<double>(){4,2,4} ),
                new DoubleCollection(new List<double>(){4,2,4,1,1} )
            };
        }
    }
}
