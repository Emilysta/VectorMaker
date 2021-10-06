using System.Xml;
namespace SVG_XAML_Converter
{
    static class SVG_To_XAML
    {
        public static void ConvertSVGToXamlCode()
        {
            
        }

        private static void LoadSVGFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            try { doc.Load(fileName); } //check if .svg can be readed
            catch (System.IO.FileNotFoundException)
            {
                //to do
            }
        }
    }
}
