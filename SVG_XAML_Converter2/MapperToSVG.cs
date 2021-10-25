using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SVG_XAML_Converter_Lib
{
    public static class MapperToSVG
    {
        private static XNamespace m_xmlnsNamespace = "http://www.w3.org/2000/svg";

        public static XElement FindSVGObjectReference(XElement xamlElement)
        {
            XElement geometryElement = null;

            switch (xamlElement.Name.LocalName) //set markup  reference
            {
                case "Rectangle":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "rect");
                        break;
                    }
                case "Ellipse":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "ellipse");
                        break;
                    }
                case "Polyline":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "polyline");
                        break;
                    }
                case "Polygon":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "polygon");
                        break;
                    }
                case "Path":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "path");
                        break;
                    }
                case "Line":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "line");
                        break;
                    }
                case "Grid.Resources":
                    {
                        if (xamlElement.Elements().Any())
                            geometryElement = CreateResources(xamlElement);
                        else
                            return null;
                        //throw new NotImplementedException();
                        //return null ;
                        break;
                    }
                case "Canvas":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "g");
                        break;
                    }
                default:
                    return null;
            }
            
            string transform = ConvertRenderTransform(xamlElement); //convert all render transforms to string
            geometryElement.SetAttributeValue("transform", transform); 
            SetAllAttributes(xamlElement, geometryElement); //set all atributes for whole object 
            return geometryElement;
        }

        private static void SetAllAttributes(XElement xamlElement, XElement svgElement)
        {
            foreach (XAttribute x in xamlElement.Attributes())
            {
                FindSVGAttributeReference(x, svgElement);
            }
        }

        private static void FindSVGAttributeReference(XAttribute xamlAttribute, XElement svgElement)
        {
            switch (xamlAttribute.Name.LocalName)
            {
                case "Style":
                    {
                        try
                        {
                            string[] values = xamlAttribute.Value.Split(' ');
                            string resourceName = values[1].TrimEnd('}');
                            svgElement.SetAttributeValue("href", "#" + resourceName);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
                case "Fill":
                    {
                        if (xamlAttribute.Value == "Transparent")
                        {
                            svgElement.SetAttributeValue("fill", "none");
                        }
                        else
                        {
                            svgElement.SetAttributeValue("fill",
                                xamlAttribute.Value);
                        }
                        break;
                    }
                case "Opacity":
                    {
                        svgElement.SetAttributeValue("opacity", xamlAttribute.Value);
                        break;
                    }
                case "Stroke":
                    {
                        svgElement.SetAttributeValue("stroke", xamlAttribute.Value);
                        break;
                    }
                case "StrokeStartLineCap":
                    {
                        SetLineCap(svgElement, xamlAttribute.Value);
                        break;
                    }
                case "StrokeDashCap":
                    {
                        SetLineCap(svgElement, xamlAttribute.Value);
                        break;
                    }
                case "StrokeEndLineCap":
                    {
                        SetLineCap(svgElement, xamlAttribute.Value);
                        break;
                    }
                case "StrokeLineJoin":
                    {
                        svgElement.SetAttributeValue("stroke-linejoin", char.ToLower(xamlAttribute.Value[0]) + xamlAttribute.Value[1..]);
                        break;
                    }
                case "StrokeDashArray":
                    {
                        svgElement.SetAttributeValue("StrokeDashArray", xamlAttribute.Value);
                        break;
                    }
                case "StrokeDashOffset":
                    {
                        svgElement.SetAttributeValue("stroke-dashoffset", xamlAttribute.Value);
                        break;
                    }
                case "StrokeMiterLimit":
                    {
                        svgElement.SetAttributeValue("stroke-miterlimit", xamlAttribute.Value);
                        break;
                    }
                case "StrokeThickness":
                    {
                        svgElement.SetAttributeValue("stroke-width", xamlAttribute.Value);
                        break;
                    }
                case "X1":
                    {
                        svgElement.SetAttributeValue("x1", xamlAttribute.Value);
                        break;
                    }
                case "X2":
                    {
                        svgElement.SetAttributeValue("x2", xamlAttribute.Value);
                        break;
                    }
                case "Y1":
                    {
                        svgElement.SetAttributeValue("y1", xamlAttribute.Value);
                        break;
                    }
                case "Y2":
                    {
                        svgElement.SetAttributeValue("y2", xamlAttribute.Value);
                        break;
                    }
                case "Width":
                    {
                        if (svgElement.Name.LocalName == "ellipse")
                        {
                            svgElement.SetAttributeValue("rx", (double)(Double.Parse(xamlAttribute.Value)/2));
                        }
                        else
                        {
                            svgElement.SetAttributeValue("width", xamlAttribute.Value);
                        }
                        break;
                    }
                case "Height":
                    {
                        if (svgElement.Name.LocalName == "ellipse")
                        {
                            svgElement.SetAttributeValue("ry", (double)(Double.Parse(xamlAttribute.Value) / 2));
                        }
                        else
                        {
                            svgElement.SetAttributeValue("height", xamlAttribute.Value);
                        }
                        break;
                    }
                case "RadiusX":
                    {
                        if (svgElement.Name.LocalName == "rect")
                        {
                            svgElement.SetAttributeValue("rx", xamlAttribute.Value);
                        }
                        else
                        {
                            //svgElement.SetAttributeValue("Width", xamlAttribute.Value);
                        }

                        break;
                    }
                case "RadiusY":
                    {
                        if (svgElement.Name.LocalName == "rect")
                        {
                            svgElement.SetAttributeValue("ry", xamlAttribute.Value);
                        }
                        else
                        {
                            //svgElement.SetAttributeValue("Height", xamlAttribute.Value);
                        }
                        break;
                    }
                case "Data":
                    {
                        svgElement.SetAttributeValue("d", xamlAttribute.Value);
                        break;
                    }
                case "Points":
                    {
                        svgElement.SetAttributeValue("points", xamlAttribute.Value);
                        break;
                    }
                case "Visibility":
                    {
                        svgElement.SetAttributeValue("id", xamlAttribute.Value);
                        break;
                    }
                case "x:Name":
                    {

                        break;
                    }
                default: break;
            }
        }

        private static void SetLineCap(XElement svgElement, string value)
        {
            switch(value)
            {
                case "Square":
                    {
                        svgElement.SetAttributeValue("stroke-linecap", "square");
                        break;
                    }
                case "Round":
                    {
                        svgElement.SetAttributeValue("stroke-linecap", "round");
                        break;
                    }
                case "Flat":
                    {
                        svgElement.SetAttributeValue("stroke-linecap", "butt");
                        break;
                    }
            }
        }

        private static string ConvertRenderTransform(XElement xamlElement)
        {
            string transformString = "";
            foreach(XElement element in xamlElement.Descendants())
            {
                if(element.Name.LocalName != "TransformGroup")
                {
                    transformString += ConvertTransformAttributes(element);
                }
            }
            return transformString;
        }

        private static string ConvertTransformAttributes(XElement element)
        {
            string singleTransform = "";
            switch (element.Name.LocalName)
            {
                case "TranslateTransform":
                    {
                        XAttribute X = element.Attributes().Where(x => x.Name.LocalName == "X").FirstOrDefault();
                        XAttribute Y = element.Attributes().Where(x => x.Name.LocalName == "Y").FirstOrDefault();
                        string x = X == null ? "0" : X.Value;
                        string y = Y == null ? "0" : Y.Value;
                        singleTransform = $"translate({x} {y}) "; 
                        break;
                    }
                case "RotateTransform":
                    {
                        XAttribute CenterX = element.Attributes().Where(x => x.Name.LocalName == "CenterX").FirstOrDefault();
                        XAttribute CenterY = element.Attributes().Where(x => x.Name.LocalName == "CenterY").FirstOrDefault();
                        XAttribute Angle = element.Attributes().Where(x => x.Name.LocalName == "Angle").FirstOrDefault();
                        string centerX = CenterX == null ? "0" : CenterX.Value;
                        string centerY = CenterY == null ? "0" : CenterY.Value;
                        string angle = Angle == null ? "0" : Angle.Value;
                        singleTransform = $"rotate({angle} {centerX} {centerY}) ";
                        break;
                    }
                case "SkewTransform":
                    {
                        XAttribute CenterX = element.Attributes().Where(x => x.Name.LocalName == "CenterX").FirstOrDefault();
                        XAttribute CenterY = element.Attributes().Where(x => x.Name.LocalName == "CenterY").FirstOrDefault();
                        XAttribute AngleX = element.Attributes().Where(x => x.Name.LocalName == "AngleX").FirstOrDefault();
                        XAttribute AngleY = element.Attributes().Where(x => x.Name.LocalName == "AngleY").FirstOrDefault();
                        string centerX = CenterX == null ? "0" : CenterX.Value;
                        string centerY = CenterY == null ? "0" : CenterY.Value;
                        string angleX = AngleX == null ? "0" : AngleX.Value;
                        string angleY = AngleY == null ? "0" : AngleY.Value;
                        singleTransform = $"skewX({angleX}) ";
                        singleTransform += $"skewY({angleY}) ";
                        singleTransform += $"translate({centerX} {centerY}) ";
                        break;
                    }
                case "ScaleTransform":
                    {
                        XAttribute CenterX = element.Attributes().Where(x => x.Name.LocalName == "CenterX").FirstOrDefault();
                        XAttribute CenterY = element.Attributes().Where(x => x.Name.LocalName == "CenterY").FirstOrDefault();
                        XAttribute ScaleX = element.Attributes().Where(x => x.Name.LocalName == "ScaleX").FirstOrDefault();
                        XAttribute ScaleY = element.Attributes().Where(x => x.Name.LocalName == "ScaleY").FirstOrDefault();
                        string centerX = CenterX == null ? "0" : CenterX.Value;
                        string centerY = CenterY == null ? "0" : CenterY.Value;
                        string scaleX = ScaleX == null ? "0" : ScaleX.Value;
                        string scaleY = ScaleY == null ? "0" : ScaleY.Value;
                        singleTransform = $"scale({scaleX} {scaleY}) ";
                        singleTransform += $"translate({centerX} {centerY}) ";
                        break;
                    }
            }
            return singleTransform;
        }

        private static XElement CreateResources(XElement xamlResources)
        {
            XElement element = new XElement("defs");
            foreach(XElement resource in xamlResources.Elements())
            {
                element.Add(ConvertToSVGResource(resource));
            }
            return element;
        }

        private static XElement ConvertToSVGResource(XElement xamlResource)
        {
            XElement element;
            XAttribute attribute = xamlResource.Attributes().Where(x => x.Name.LocalName == "TargetType").FirstOrDefault();
            XAttribute id = xamlResource.Attributes().Where(x => x.Name.LocalName == "Key").FirstOrDefault();
            switch (attribute.Name.LocalName)
            {
                case "Rectangle":
                    {
                        element = ConvertStyleToObjectXAML(xamlResource);
                        break;
                    }
                case "Ellipse":
                    {
                        element = new XElement("ellipse");
                        break;
                    }
                case "Line":
                    {
                        element = new XElement("rect");
                        break;
                    }
                case "Polyline":
                    {
                        element = new XElement("rect");
                        break;
                    }
            }
            
            return null;
        }

        private static XElement ConvertStyleToObjectXAML(XElement element)
        {
            foreach (XElement setter in element.Elements())
            {
                XAttribute attribute = element.Attribute(m_xmlnsNamespace + "Property");
            }
            return null;
        }
    }
}
