
using System.Windows.Media;
using VectorMaker.Utility;
using ColorDef = System.Windows.Media.Color;

namespace VectorMaker.Models
{
    internal class DrawingDocumentData : NotifyPropertyChangedBase
    {
        #region Fields
        private double m_width = 250;
        private double m_height = 250;
        private string m_title = "";
        private string m_description = "";
        private SolidColorBrush m_background = new SolidColorBrush(ColorDef.FromArgb(0,255,255,255));

        #endregion

        #region Properties
        public double Width { get { return m_width; } set { m_width = value; OnPropertyChanged(nameof(Width)); } }
        public double Height { get { return m_height; } set { m_height = value; OnPropertyChanged(nameof(Height)); } }
        public string Title { get { return m_title; } set { m_title = value; OnPropertyChanged(nameof(Title)); } }
        public string Description { get { return m_description; } set { m_description = value; OnPropertyChanged(nameof(Description)); } }
        public SolidColorBrush Background { get { return m_background; } set { m_background = value; OnPropertyChanged(nameof(Background)); } }
        #endregion

        #region Constructors
        public DrawingDocumentData(double width=250, double height=250) {
            Width = width;
            Height = height;
        }
        #endregion
    }
}
