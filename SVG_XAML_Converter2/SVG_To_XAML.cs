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
        public static XDocument ConvertSVGToXamlCode()
        {
            Console.WriteLine("Podaj ścieżkę: ");
            string line = Console.ReadLine();
            XDocument document = LoadSVGFile(line);
            var desc = document.DescendantNodes();
            XElement svgMainDocument = document.Descendants().Where(x => x.Name.LocalName == "svg").First();
            XDocument xamlDocument = new XDocument();
            foreach (var element in LoopThroughSVGElement(svgMainDocument))
            {
                xamlDocument.Add(element);
            }
            return xamlDocument;
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
            //throw new Exception("Validation error");
        }

        private static IEnumerable<XElement> LoopThroughSVGElement(XElement element)
        {
            XElement mappedElement = Mapper.FindXAMLObjectReference(element);
            yield return mappedElement;
            IEnumerable<XElement> descendants = element.Descendants();
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
