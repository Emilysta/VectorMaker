using System;
using System.Collections.Generic;
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
            if (CheckIfContainsIdAttribute(svgElement))
            {

            }
            {
                return FindXamlReferenceWhenNoID(svgElement);
            }
        }

        private static XElement FindXamlReferenceWhenNoID(XElement svgElement)
        {

            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement pathElement = new XElement(xNamespace + "Path");
            pathElement.SetAttributeValue("Fill", "Black");
            XElement pathDataElement = new XElement(xNamespace + "Path.Data");
            XElement baseElementForGeometry = null;
            XElement geometryElement = null;

            switch (svgElement.Name.LocalName) //set markup  reference
            {
                case "rect":
                    {
                        geometryElement = new XElement(xNamespace + "Rect");
                        baseElementForGeometry = new XElement(xNamespace + "RectangleGeometry",
                            new XElement(xNamespace + "RectangleGeometry.Rect",
                            geometryElement)
                            );
                        break;
                    }
                case "circle":
                    {
                        geometryElement = new XElement(xNamespace + "EllipseGeomentry");
                        break;
                    }
                case "ellipse":
                    {
                        geometryElement = new XElement(xNamespace + "EllipseGeomentry");
                        break;
                    }
                case "polyline":
                    {
                        geometryElement = new XElement(xNamespace + "PolyLineSegment");
                        baseElementForGeometry = new XElement(xNamespace + "PathGeometry",
                            new XElement(xNamespace + "PathGeometry.Figures",
                                new XElement(xNamespace + "PathFigure", geometryElement)
                                )
                            );
                        break;
                    }
                case "polygon":
                    {
                        geometryElement = new XElement(xNamespace + "PolyLineSegment");
                        baseElementForGeometry = new XElement(xNamespace + "PathGeometry",
                            new XElement(xNamespace + "PathGeometry.Figures",
                                new XElement(xNamespace + "PathFigure", geometryElement)
                                )
                            );
                        break;
                    }
                case "path":
                    {
                        geometryElement = new XElement(xNamespace + "PathGeometry");
                        break;
                    }
                case "use":
                    {
                        //display warning : style block is not supported
                        return null;
                    }
                case "g":
                    {
                        geometryElement = new XElement(xNamespace + "Grid");
                        geometryElement.SetAttributeValue("Tag", "group");
                        return geometryElement;
                    }
                case "text":
                    {
                        break;
                    }
                case "style":
                    {
                        //display warning : style block is not supported
                        break;
                    }
                case "glyph":
                    {
                        //depre
                        break;
                    }
                default:
                    return null;
            }

            SetAllAttributes(svgElement, pathElement, geometryElement); //set all atributes for whole object 

            if (baseElementForGeometry != null) //if base element is available add it to pathDataElement
                pathDataElement.Add(baseElementForGeometry);
            else
                pathDataElement.Add(geometryElement); //else add geometry directly to path Data
            pathElement.Add(pathDataElement);

            pathElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").First()?.Attributes("xmlns")?.Remove(); //remove namespace for child Path.Data
            
            return pathElement;
        }

        private static void SetAllAttributes(XElement svgElement, XElement pathElement, XElement geometryElement)
        {
            List<XAttribute> xamlReferenceOfAttributes;
            foreach (XAttribute x in svgElement.Attributes())
            {
                xamlReferenceOfAttributes = FindXAMLAttributeReference(x);
                if (xamlReferenceOfAttributes != null)
                {
                    foreach (XAttribute attribute in xamlReferenceOfAttributes)
                    {
                        AttributeParent parent = CheckParentForAttribute(attribute);
                        if (parent == AttributeParent.Path)
                            pathElement.SetAttributeValue(attribute.Name, attribute.Value);
                        else

                            geometryElement.SetAttributeValue(attribute.Name, attribute.Value);
                    }
                }
            }
            if(svgElement.Value!=null && svgElement.Value.Length !=0) //add text or other content

                
            if (svgElement.Name.LocalName == "polyline")
            {

                string[] points = geometryElement.Attribute("Points").Value.Split(' ');
                if (points.Length != 0)
                    geometryElement.Parent.SetAttributeValue("StartPoint", points[0]);
            }
            if (svgElement.Name.LocalName == "polygon")
            {

                string[] points = geometryElement.Attribute("Points").Value.Split(' ');
                if (points.Length != 0)
                {
                    geometryElement.Parent.SetAttributeValue("StartPoint", points[0]);
                    geometryElement.Parent.SetAttributeValue("IsClosed", "True");
                }
            }
        }

        private static List<XAttribute> FindXAMLAttributeReference(XAttribute svgAttribute)
        {
            List<XAttribute> xamlAttributesList = new List<XAttribute>();
            switch (svgAttribute.Name.LocalName)
            {
                case "style":
                    {
                        try
                        {
                            List <XAttribute> tempList = null;
                            foreach (string s in svgAttribute.Value.Split(';'))
                            {
                                string[] values = s.Split(':');
                                tempList = FindXAMLAttributeReference(new XAttribute(values[0], values[1]));
                                if(tempList!=null)
                                    xamlAttributesList.AddRange(tempList);
                            }
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Console.WriteLine(e.Message);
                            return null;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return null;
                        }
                        break;
                    }
                case "fill":
                    {
                        if (svgAttribute.Value != "none")
                            xamlAttributesList.Add( new XAttribute("Fill", 
                                char.ToUpper(svgAttribute.Value[0]) + svgAttribute.Value.Substring(1)
                                ));
                        else
                            xamlAttributesList.Add(new XAttribute("Fill", "transparent"));
                        break;
                    }
                case "fill-opacity":
                    {
                        break;
                    }
                case "opacity":
                    {
                        xamlAttributesList.Add(new XAttribute("Opacity", svgAttribute.Value));
                        break;
                    }
                case "stroke":
                    {
                        xamlAttributesList.Add(new XAttribute("Stroke", svgAttribute.Value));
                        break;
                    }
                case "stroke-opacity":
                    {
                        xamlAttributesList.Add(new XAttribute("Opacity", svgAttribute.Value));
                        break;
                    }
                case "stroke-width":
                    {
                        xamlAttributesList.Add(new XAttribute("StrokeThickness", svgAttribute.Value));
                        break;
                    }
                case "x":
                    {
                        xamlAttributesList.Add(new XAttribute("X", svgAttribute.Value));
                        break;
                    }
                case "y":
                    {
                        xamlAttributesList.Add(new XAttribute("Y", svgAttribute.Value));
                        break;
                    }
                case "cx":
                    {
                        xamlAttributesList.Add(new XAttribute("Center", svgAttribute.Value + ",0"));
                        break;
                    }
                case "cy":
                    {
                        xamlAttributesList.Add(new XAttribute("Center", "0," + svgAttribute.Value));
                        break;
                    }
                case "width":
                    {
                        xamlAttributesList.Add(new XAttribute("Width", svgAttribute.Value));
                        break;
                    }
                case "height":
                    {
                        xamlAttributesList.Add(new XAttribute("Height", svgAttribute.Value));
                        break;
                    }
                case "rx":
                    {
                        xamlAttributesList.Add(new XAttribute("RadiusX", svgAttribute.Value));
                        break;
                    }
                case "ry":
                    {
                        xamlAttributesList.Add(new XAttribute("RadiusY", svgAttribute.Value));
                        break;
                    }
                case "r":
                    {
                        xamlAttributesList.Add(new XAttribute("RadiusX", svgAttribute.Value));
                        xamlAttributesList.Add(new XAttribute("RadiusY", svgAttribute.Value));
                        break;
                    }
                case "class":
                    {
                        //display warning : style block is not supported
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
                case "href":
                    {

                        break;
                    }
                case "d":
                    {
                        xamlAttributesList.Add(new XAttribute("Figures", svgAttribute.Value));
                        break;
                    }
                case "points":
                    {
                        xamlAttributesList.Add(new XAttribute("Points", svgAttribute.Value));
                        break;
                    }
                default: return null;
            }
            return xamlAttributesList;
        }

        private static AttributeParent CheckParentForAttribute(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
            {
                case "Fill": case "Stroke": case "StrokeThickness": case "Opacity": return AttributeParent.Path;

                default: return AttributeParent.Geometry;
            }
        }

        private static bool CheckIfContainsSpecialHrefAttribute(XElement element)
        {
            foreach (XAttribute att in element.Attributes())
            {
                if (att.Value.Contains('#'))
                    return true;
            }
            return false;
        }

        private static bool CheckIfContainsIdAttribute(XElement element)
        {
            foreach (XAttribute att in element.Attributes())
            {
                if (att.Name.LocalName == "id")
                    return true;
            }
            return false;
        }
    }
}
