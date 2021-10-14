using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SVG_XAML_Converter_Lib
{
    public static class SVG_To_XAML
    {
        public static XDocument ConvertSVGToXamlCode(string fileName)
        {
            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            try
            {
                XDocument document = LoadSVGFile(fileName);
                if (document == null)
                    throw new Exception();
                XElement svgMainDocument = document.Elements().Where(x => x.Name.LocalName == "svg").First();
                XDocument xamlDocument = new XDocument();
                XElement geometryGroup = new XElement(xNamespace + "Grid");
                foreach (var element in LoopThroughSVGElement(svgMainDocument))
                {
                    if (element != null)
                    {
                        geometryGroup.Add(element);
                    }
                }
                xamlDocument.Add(geometryGroup);
                return xamlDocument;
            }
            catch(Exception e)
            {
                Console.WriteLine("Blad przy konwersji \n" + e.Message);
                return null;   
            }
        }

        private static XDocument LoadSVGFile(string fileName)
        {
            XDocument document = null;
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", "./xml.xsd");
                xmlReaderSettings.Schemas.Add("http://www.w3.org/1999/xlink", "./xlink.xsd");
                xmlReaderSettings.Schemas.Add("http://www.w3.org/2000/svg", "./SVG.xsd");
                xmlReaderSettings.Schemas.Compile();
                document = XDocument.Load(fileName);
                document.Validate(xmlReaderSettings.Schemas, SVGValidationEventHandler);

                return document;
            }
            catch (XmlSchemaException e)
            {
                Console.WriteLine("Schema error\n" + e.Message + "\n" + e.SourceUri + " " + e.LineNumber);
                Console.WriteLine(e.StackTrace);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught" + e.Message);
                return null;
            }
        }

        private static void SVGValidationEventHandler(object e, ValidationEventArgs args)
        {
            Console.WriteLine("Not passed validation" + args.Message);
        }

        private static IEnumerable<XElement> LoopThroughSVGElement(XElement element)
        {
            XElement mappedElement = Mapper.FindXAMLObjectReference(element);
            yield return mappedElement;
            IEnumerable<XElement> descendants = element.Elements();
            if (descendants.Count() != 0)
            {
                foreach (XElement descendantElement in descendants)
                {
                    foreach (var x in LoopThroughSVGElement(descendantElement))
                        yield return x;
                }
            }
        }
    }
}
