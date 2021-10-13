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
            XElement pathElement = new XElement("Path");
            XElement pathDataElement = new XElement("Path.Data");
            XElement geometryElement = null;
            switch (svgElement.Name.LocalName)
            {
                case "rect":
                    {
                        geometryElement = new XElement("RectangleGeometry");
                        XAttribute xamlReferenceOfAttribute;
                        foreach (XAttribute x in svgElement.Attributes())
                        {
                            xamlReferenceOfAttribute = FindXAMLAttributeReference(x);
                            if(xamlReferenceOfAttribute!=null)
                            {
                                AttributeParent parent = CheckParentForAttribute(xamlReferenceOfAttribute);
                                if (parent == AttributeParent.Path)
                                    pathElement.SetAttributeValue(xamlReferenceOfAttribute.Name, xamlReferenceOfAttribute.Value);
                                else
                                    geometryElement.SetAttributeValue(xamlReferenceOfAttribute.Name, xamlReferenceOfAttribute.Value);
                            }
                        }
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
            }
            if(geometryElement!= null)
                pathDataElement.Add(geometryElement);
            pathElement.Add(pathDataElement);
            return pathElement;
        }
        public static XAttribute FindXAMLAttributeReference(XAttribute svgAttribute)
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
                        break;
                    }
                case "fill-opacity":
                    {
                        break;
                    }
                case "opacity":
                    {
                        break;
                    }
                case "stroke":
                    {
                        break;
                    }
                case "stroke-opacity":
                    {
                        break;
                    }
                case "stroke-width":
                    {
                        break;
                    }
                case "x":
                    {
                        break;
                    }
                case "y":
                    {
                        break;
                    }
                case "cx":
                    {
                        break;
                    }
                case "cy":
                    {
                        break;
                    }
                case "width":
                    {
                        break;
                    }
                case "height":
                    {
                        break;
                    }
                case "rx":
                    {
                        break;
                    }
                case "ry":
                    {
                        break;
                    }
                case "r":
                    {
                        break;
                    }
                case "id":
                    {
                        break;
                    }
                case "class":
                    {
                        break;
                    }
                case "color":
                    {
                        break;
                    }
                case "clip-path":
                    {
                        break;
                    }
                case "clip-rule":
                    {
                        break;
                    }
                case "display":
                    {
                        break;
                    }
                case "visibility":
                    {
                        break;
                    }
                case "pathLength":
                    {
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
