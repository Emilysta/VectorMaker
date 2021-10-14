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
            string line = Console.ReadLine();
            XDocument document = SVG_To_XAML.ConvertSVGToXamlCode(line);
            Console.WriteLine(document.ToString());
        }
    }
}
