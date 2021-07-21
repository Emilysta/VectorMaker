using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VectorMaker.PropertiesPanel
{
    /// <summary>
    /// Interaction logic for ObjectProperties.xaml
    /// </summary>
    public partial class ObjectProperties : Page
    {
        public ObjectProperties()
        {

            //var icon = this.IconKind;

            //string buttonContent = this.GetValue(m_content).ToString();
            //Trace.WriteLine(iconKind);
            //Trace.WriteLine(buttonContent);
            //if (iconKind != PackIconMaterialDesignKind.None.ToString()
            //    && buttonContent == "")
            //{
            //    IconColumnSpan = 2;
            //}
            //else if (iconKind == PackIconMaterialDesignKind.None.ToString()
            //    && buttonContent != "")
            //{
            //    ContentColumnSpan = 2;
            //}
            //else if (iconKind != PackIconMaterialDesignKind.None.ToString()
            //     && buttonContent != "")
            //{
            //    ContentColumn = 1;
            //    IconHeight = (int)(IconHeight * 0.8f);
            //    IconWidth = (int)(IconWidth * 0.8f);
            //    FontSize = FontSize * 0.8;
            //}
            InitializeComponent();

        }
    }
}
