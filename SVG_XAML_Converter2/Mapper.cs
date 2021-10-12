using System.Xml;
using System.Xml.Linq;

namespace SVG_XAML_Converter_Lib
{
    public static class Mapper
    {
        public static void FindXAMLObjectReference(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case "rect":
                    {
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
        }
        public static void FindXAMLAttributeReference(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
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
        }
    }
}
