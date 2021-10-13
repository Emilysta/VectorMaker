using System.Linq;
using System.Xaml;
using System.Xml.Linq;

namespace SVG_XAML_Converter_Lib
{
    public enum AttributeParent
    { 
        Path,
        Geometry
    }


    public static class Mapper
    {
        public static XElement FindXAMLObjectReference(XElement svgElement)
        {
            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement pathElement = new XElement(xNamespace+"Path");
            XElement pathDataElement = new XElement(xNamespace + "Path.Data");
            XElement baseElementForGeometry = null;
            XElement geometryElement = null;
            
            switch (svgElement.Name.LocalName)
            {
                case "rect":
                    {
                        geometryElement = new XElement(xNamespace + "Rect");
                        baseElementForGeometry = new XElement(xNamespace + "RectangleGeometry",
                            new XElement(xNamespace+"RectangleGeometry.Rect",
                            geometryElement)
                            );
                        break;
                    }
                case "circle":
                    {
                        break;
                    }
                case "ellipse":
                    {
                        break;
                    }
                case "polyline":
                    {
                        break;
                    }
                case "polygon":
                    {
                        break;
                    }
                case "path":
                    {
                        break;
                    }
                case "use":
                    {
                        break;
                    }
                case "g":
                    {
                        break;
                    }
                default: geometryElement = null;
                    break;
            }
            if (geometryElement == null)
                return null;
            

            XAttribute xamlReferenceOfAttribute;
            foreach (XAttribute x in svgElement.Attributes())
            {
                xamlReferenceOfAttribute = FindXAMLAttributeReference(x);
                if (xamlReferenceOfAttribute != null)
                {
                    AttributeParent parent = CheckParentForAttribute(xamlReferenceOfAttribute);
                    if (parent == AttributeParent.Path)
                        pathElement.SetAttributeValue(xamlReferenceOfAttribute.Name, xamlReferenceOfAttribute.Value);
                    else
                        geometryElement.SetAttributeValue(xamlReferenceOfAttribute.Name, xamlReferenceOfAttribute.Value);
                }
            }
            if(baseElementForGeometry!=null)
                pathDataElement.Add(baseElementForGeometry);
            else
                pathDataElement.Add(geometryElement);
            pathElement.Add(pathDataElement);

            pathElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").First()?.Attributes("xmlns")?.Remove();
            
            return pathElement;
        }
        private static XAttribute FindXAMLAttributeReference(XAttribute svgAttribute)
        {
            XAttribute xamlAttribute = null;
            switch (svgAttribute.Name.LocalName)
            {
                case "style":
                    {
                        break;
                    }
                case "fill":
                    {
                        xamlAttribute = new XAttribute("Fill", svgAttribute.Value);
                        break;
                    }
                case "fill-opacity":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "opacity":
                    {
                        xamlAttribute = new XAttribute("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke":
                    {
                        xamlAttribute = new XAttribute("Stroke", svgAttribute.Value);
                        break;
                    }
                case "stroke-opacity":
                    {

                        xamlAttribute = new XAttribute("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke-width":
                    {
                        xamlAttribute = new XAttribute("StrokeThickness", svgAttribute.Value);
                        break;
                    }
                case "x":
                    {
                        xamlAttribute = new XAttribute("X", svgAttribute.Value);
                        break;
                    }
                case "y":
                    {
                        xamlAttribute = new XAttribute("Y", svgAttribute.Value);
                        break;
                    }
                case "cx":
                    {
                        xamlAttribute = new XAttribute("Center", svgAttribute.Value+",0");
                        break;
                    }
                case "cy":
                    {
                        xamlAttribute = new XAttribute("Center", "0," + svgAttribute.Value );
                        break;
                    }
                case "width":
                    {
                        xamlAttribute = new XAttribute("Width", svgAttribute.Value);
                        break;
                    }
                case "height":
                    {
                        xamlAttribute = new XAttribute("Height", svgAttribute.Value);
                        break;
                    }
                case "rx":
                    {
                        xamlAttribute = new XAttribute("RadiusX", svgAttribute.Value);
                        break;
                    }
                case "ry":
                    {
                        xamlAttribute = new XAttribute("RadiusY", svgAttribute.Value);
                        break;
                    }
                case "r":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "id":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "class":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "color":
                    {
                        xamlAttribute = new XAttribute("RadiusY", svgAttribute.Value);
                        break;
                    }
                case "clip-path":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "clip-rule":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "display":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "visibility":
                    {
                        xamlAttribute = null;
                        break;
                    }
                case "pathLength":
                    {
                        xamlAttribute = null;
                        break;
                    }
            }
            return xamlAttribute;
        }

        private static AttributeParent CheckParentForAttribute(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
            {
                case "Fill": return AttributeParent.Path; 
                case "Stroke": return AttributeParent.Path;
                case "StrokeThickness": return AttributeParent.Path;
                default: return AttributeParent.Geometry;
            }
        }
    }
}
