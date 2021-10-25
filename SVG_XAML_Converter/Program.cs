using System;
using System.Xml.Linq;
using SVG_XAML_Converter_Lib;

namespace SVG_XAML_Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Podaj ścieżkę: ");
            //string line = Console.ReadLine();
            XDocument document = SVG_To_XAML.ConvertSVGToXamlCode("C://Users//Emilia//Desktop//test.svg");
            //XDocument document = SVG_To_XAML.ConvertSVGToXamlCode("C://Users//emili//OneDrive//Pulpit//UseTest.svg");
            if (document != null)
                Console.WriteLine(document.ToString());

            XDocument svgDocument = XAML_To_SVG.ConvertXAMLToSVGCode(document);
            if (svgDocument != null)
                Console.WriteLine(svgDocument.ToString());
        }
    }
}
