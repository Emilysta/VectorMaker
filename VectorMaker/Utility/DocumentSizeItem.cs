
using MahApps.Metro.IconPacks;

namespace VectorMaker.Utility
{
    internal class DocumentSizeItem
    {
        #region Fields
        private double m_width;
        private double m_height;
        private double m_dpi;
        private PackIconBootstrapIconsKind m_kind;
        private string m_name;
        private bool m_isVertical = true;

        #endregion

        #region Properties
        public double Width { get => m_width; set => m_width = value; }
        public double Height { get => m_height; set => m_height = value; }

        public double Dpi { get => m_dpi; set => m_dpi = value; }

        public string SizeLabel
        {
            get
            {
                return $"{Width}x{Height}";
            }
        }

        public PackIconBootstrapIconsKind Kind { get => m_kind; set => m_kind = value; }

        public string Name { get => m_name; set => m_name = value; }

        public bool IsVertical { get => m_isVertical; set => m_isVertical = value; }
        #endregion

        #region Constructors
        public DocumentSizeItem(double width,double height,PackIconBootstrapIconsKind kind, string name, double dpi, bool isVertical = true)
        {
            Width = width;
            Height = height;
            Kind = kind;
            Name = name;
            Dpi = dpi;
            IsVertical = isVertical;
        }
        #endregion
    }
}
