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
                XElement parentElement = new XElement(xNamespace + "Grid");
                XElement parentResourcesElement = new XElement(xNamespace + "Grid.Resources");
                parentElement.Add(parentResourcesElement);

                LoopThroughSVGElement(svgMainDocument, parentElement, parentResourcesElement);
                xamlDocument.Add(parentElement);
                return xamlDocument;
            }
            catch (Exception e)
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

        private static void LoopThroughSVGElement(XElement element, XElement parent, XElement mainResources)
        {
            List<XElement> mappedElements = Mapper.FindXAMLObjectReference(element);
            foreach (XElement mappedElement in mappedElements)
            {
                if (mappedElement != null)
                {
                    if (mappedElement.Name.LocalName == "Style")
                        mainResources.Add(mappedElement);
                    else
                        parent.Add(mappedElement);
                }
            }

            foreach (XElement descendantElement in element.Elements())
            {
                LoopThroughSVGElement(descendantElement, element.Name.LocalName == "svg" ? parent : mappedElements.Find(x=>x.Name!="Style"), mainResources);
            }

        }
    }
}
