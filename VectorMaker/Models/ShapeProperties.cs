using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace VectorMaker.Models
{
    internal class ShapeProperties
    {
        #region Fields
        private static SolidColorBrush m_fillBrush = Brushes.Black;
        private static SolidColorBrush m_strokeBrush = Brushes.Red;
        private static double m_strokeThickness = 1;
        private static int m_selectedStrokeDashArray = 0;
        private static ObservableCollection<DoubleCollection> m_strokeTypes;
        #endregion

        #region Properties
        public static SolidColorBrush FillBrush
        {
            get => m_fillBrush;
            set
            {
                m_fillBrush = value;
            }
        }

        public static SolidColorBrush StrokeBrush
        {
            get => m_strokeBrush;
            set
            {
                m_strokeBrush = value;
            }
        }

        public static double StrokeThickness
        {
            get => m_strokeThickness;
            set
            {
                m_strokeThickness = value;
            }
        }

        public static int SelectedStrokeDashArray
        {
            get => m_selectedStrokeDashArray;
            set
            {
                m_selectedStrokeDashArray = value;
            }
        }
        public static ObservableCollection<DoubleCollection> StrokeTypes
        {
            get => m_strokeTypes;
            set
            {
                m_strokeTypes = value;
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
