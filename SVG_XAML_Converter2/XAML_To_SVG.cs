using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SVG_XAML_Converter_Lib
{
    public static class XAML_To_SVG
    {
        public static XDocument svgDocument = null;
        public static XDocument ConvertXAMLToSVGCode(XDocument xamlDocument)
        {
            XNamespace xNamespace = "http://www.w3.org/2000/svg";

            try
            {
                XDocument svgDocument = new XDocument();
                //toDo stworzenie svg + xml
                XElement mainSVGElement = new XElement(xNamespace + "svg");
                LoopThroughXAMLElements(xamlDocument.Root.Elements(), mainSVGElement);
                svgDocument.Add(mainSVGElement);
                return svgDocument;
            }
            catch (Exception e)
            {
                Console.WriteLine("Blad przy konwersji XAML to SVG \n" + e.Message);
                return null;
            }
        }

        //private static XDocument LoadSVGFile(string fileName)
        //{
        //    XDocument document = null;
        //    try
        //    {
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", "./xml.xsd");
        //        xmlReaderSettings.Schemas.Add("http://www.w3.org/1999/xlink", "./xlink.xsd");
        //        xmlReaderSettings.Schemas.Add("http://www.w3.org/2000/svg", "./SVG.xsd");
        //        xmlReaderSettings.Schemas.Compile();
        //        document = XDocument.Load(fileName);
        //        document.Validate(xmlReaderSettings.Schemas, SVGValidationEventHandler);

        //        return document;
        //    }
        //    catch (XmlSchemaException e)
        //    {
        //        Console.WriteLine("Schema error\n" + e.Message + "\n" + e.SourceUri + " " + e.LineNumber);
        //        Console.WriteLine(e.StackTrace);
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception caught" + e.Message);
        //        return null;
        //    }
        //}

        private static void LoopThroughXAMLElements(IEnumerable<XElement> elements, XElement parent)
        {
            foreach (XElement element in elements)
            {

                XElement temp = MapperToSVG.FindSVGObjectReference(element);
                if (temp != null)
                {
                    parent.Add(temp);
                    if (element.Name.LocalName == "Canvas")
                    {
                        LoopThroughXAMLElements(element.Elements(), temp);
                    }
                }

            }
        }

    }
}
