using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SVG_XAML_Converter_Lib
{
    public static class SVG_To_XAML
    {
        public static void ConvertSVGToXamlCode()
        {
            Console.WriteLine("Podaj ścieżkę: ");
            string line = Console.ReadLine();
            LoadSVGFile(line);

        }

        private static void LoadSVGFile(string fileName)
        {
            XmlReader xmlReader = null;
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();

                xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(SVGValidationEventHandler);
                xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
                xmlReaderSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", "./xml.xsd");
                xmlReaderSettings.Schemas.Add("http://www.w3.org/1999/xlink", "./xlink.xsd");
                xmlReaderSettings.Schemas.Add("http://www.w3.org/2000/svg", "./SVG.xsd");
                xmlReaderSettings.Schemas.Compile();
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                xmlReader = XmlReader.Create(fileName, xmlReaderSettings);

                while (xmlReader.Read())
                {
                }

                Console.WriteLine("passed validation");
            }
            catch (XmlSchemaException e)
            {
                Console.WriteLine("Not passed validation\n" + e.Message +"\n"+ e.SourceUri + " "+e.LineNumber);
                Console.WriteLine(e.StackTrace);
            }
            finally 
            { 
                if(xmlReader!=null)
                {
                    xmlReader.Close();
                }
            }
        }

        private static void SVGValidationEventHandler(object e, ValidationEventArgs args)
        {
            Console.WriteLine("Not passed validation" + args.Message);
            //throw new Exception("Validation error");
        }
    }
}
