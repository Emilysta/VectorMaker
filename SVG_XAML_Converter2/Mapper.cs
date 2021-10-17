using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace SVG_XAML_Converter_Lib
{
    public enum AttributeParent
    {
        Path,
        Geometry,
        TranslateTransform

    }


    public static class Mapper
    {
        public static List<XElement> FindXAMLObjectReference(XElement svgElement)
        {
            if (CheckIfContainsIdAttribute(svgElement))
            {
                return FindXamlReferenceWhenID(svgElement);
            }
            else
            {
                return new List<XElement>() { FindXamlReferenceWhenNoID(svgElement) };
            }
        }

        private static List<XElement> FindXamlReferenceWhenID(XElement svgElement)
        {
            throw new NotImplementedException();
        }

        private static XElement FindXamlReferenceWhenNoID(XElement svgElement)
        {

            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement pathElement;
            XElement pathDataElement;
            CreateDefaultPathElement(xNamespace,out pathElement, out pathDataElement);
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
                        geometryElement = new XElement(xNamespace + "Path");
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

            if (baseElementForGeometry != null) //if base element is available add it to pathDataElement
                pathDataElement.Add(baseElementForGeometry);
            else
                pathDataElement.Add(geometryElement); //else add geometry directly to path Data
            pathElement.Add(pathDataElement);

            SetAllAttributes(svgElement, pathElement); //set all atributes for whole object 

            pathElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").First()?.Attributes("xmlns")?.Remove(); //remove namespace for child Path.Data

            return pathElement;
        }

        private static void CreateDefaultPathElement(XNamespace xNamespace, out XElement pathElement, out XElement pathDataElement)
        {
            pathElement = new XElement(xNamespace + "Path");
            pathElement.SetAttributeValue("Fill", "Black");
            pathDataElement = new XElement(xNamespace + "Path.Data");
            XElement renderTransformElement = new XElement(xNamespace + "Path.RenderTransform");
            XElement transformGroupElement = new XElement(xNamespace + "TransformGroup");
            XElement translateTransform = new XElement(xNamespace + "TranslateTransform");
            XElement rotateTransform = new XElement(xNamespace + "RotateTransform");
            XElement skewTransform = new XElement(xNamespace + "SkewTransform");
            XElement scaleTransform = new XElement(xNamespace + "ScaleTransform");

            translateTransform.SetAttributeValue("X", 0);
            translateTransform.SetAttributeValue("Y", 0);
            
            rotateTransform.SetAttributeValue("Angle", 0);
            rotateTransform.SetAttributeValue("CenterX", 0);
            rotateTransform.SetAttributeValue("CenterY", 0);

            skewTransform.SetAttributeValue("AngleX", 0);
            skewTransform.SetAttributeValue("AngleY", 0);
            skewTransform.SetAttributeValue("CenterX", 0);
            skewTransform.SetAttributeValue("CenterY", 0);

            scaleTransform.SetAttributeValue("ScaleX", 0);
            scaleTransform.SetAttributeValue("ScaleY", 0);
            scaleTransform.SetAttributeValue("CenterX", 0);
            scaleTransform.SetAttributeValue("CenterY", 0);

            transformGroupElement.Add(
                translateTransform,
                rotateTransform,
                skewTransform,
                scaleTransform
                );
            renderTransformElement.Add(transformGroupElement);
            pathElement.Add(renderTransformElement);
        }

        private static void SetAllAttributes(XElement svgElement, XElement pathElement)
        {
            foreach (XAttribute x in svgElement.Attributes())
            {
               FindXAMLAttributeReference(x, pathElement);
            }
            //if (svgElement.Value != null && svgElement.Value.Length != 0) //add text or other content


            //    if (svgElement.Name.LocalName == "polyline")
            //    {

            //        string[] points = geometryElement.Attribute("Points").Value.Split(' ');
            //        if (points.Length != 0)
            //            geometryElement.Parent.SetAttributeValue("StartPoint", points[0]);
            //    }
            //if (svgElement.Name.LocalName == "polygon")
            //{

            //    string[] points = geometryElement.Attribute("Points").Value.Split(' ');
            //    if (points.Length != 0)
            //    {
            //        geometryElement.Parent.SetAttributeValue("StartPoint", points[0]);
            //        geometryElement.Parent.SetAttributeValue("IsClosed", "True");
            //    }
            //}
        }

        private static void FindXAMLAttributeReference(XAttribute svgAttribute, XElement pathElement)
        {
            XElement geometryElement = GetGeometryElement(pathElement);
            switch (svgAttribute.Name.LocalName)
            {
                case "style":
                    {
                        try
                        {
                            foreach (string s in svgAttribute.Value.Split(';'))
                            {
                                string[] values = s.Split(':');
                                FindXAMLAttributeReference(new XAttribute(values[0], values[1]), pathElement);
                            }
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
                case "fill":
                    {
                        if (svgAttribute.Value != "none")
                            pathElement.SetAttributeValue("Fill",
                                char.ToUpper(svgAttribute.Value[0]) + svgAttribute.Value[1..]);
                        else
                            pathElement.SetAttributeValue("Fill", "Transparent");
                        break;
                    }
                case "fill-opacity":
                    {
                        break;
                    }
                case "opacity":
                    {
                        pathElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke":
                    {
                        pathElement.SetAttributeValue("Stroke", svgAttribute.Value);
                        break;
                    }
                case "stroke-opacity":
                    {
                        pathElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke-width":
                    {
                        pathElement.SetAttributeValue("StrokeThickness", svgAttribute.Value);
                        break;
                    }
                case "x":
                    {
                        XElement renderTransformElement = GetRenderTransformElement(pathElement);
                        XElement element = new XElement("TranslateTransform");
                        element.SetAttributeValue("X", svgAttribute.Value);
                        break;
                    }
                case "y":
                    {
                        XElement renderTransformElement = GetRenderTransformElement(pathElement);
                        XElement element = new XElement("TranslateTransform");
                        element.SetAttributeValue("Y", svgAttribute.Value);
                        break;
                    }
                case "cx":
                    {
                        geometryElement.SetAttributeValue("Center", svgAttribute.Value + ",0");
                        break;
                    }
                case "cy":
                    {
                        geometryElement.SetAttributeValue("Center", "0," + svgAttribute.Value);
                        break;
                    }
                case "width":
                    {
                        geometryElement.SetAttributeValue("Width", svgAttribute.Value);
                        break;
                    }
                case "height":
                    {
                        geometryElement.SetAttributeValue("Height", svgAttribute.Value);
                        break;
                    }
                case "rx":
                    {
                        geometryElement.SetAttributeValue("RadiusX", svgAttribute.Value);
                        break;
                    }
                case "ry":
                    {
                        geometryElement.SetAttributeValue("RadiusY", svgAttribute.Value);
                        break;
                    }
                case "r":
                    {
                        geometryElement.SetAttributeValue("RadiusX", svgAttribute.Value);
                        geometryElement.SetAttributeValue("RadiusY", svgAttribute.Value);
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
                        if (svgAttribute.Value.Contains('#'))
                            pathElement.SetAttributeValue("Style", "{StaticResource " + svgAttribute.Value.TrimStart('#') + "}");
                        else
                        {
                            //todo hrefs 
                        }
                        break;
                    }
                case "transform":
                    {
                        string[] transforms = svgAttribute.Value.Split();
                        foreach(string transform in transforms)
                        {
                            string[] propertiesOfTransform = transform.Split('(');
                            SetTransformAttributes(pathElement,propertiesOfTransform);
                        }
                        break;
                    }
                case "d":
                    {
                        geometryElement.SetAttributeValue("Figures", svgAttribute.Value);
                        break;
                    }
                case "points":
                    {
                        string startPoint = svgAttribute.Value.Split(' ')[0];
                        geometryElement.SetAttributeValue("Points", svgAttribute.Value);
                        geometryElement.Parent.SetAttributeValue("StartPoint", startPoint);
                        break;
                    }
                default: break;
            }
        }

        private static void SetTransformAttributes(XElement pathElement,string[] transforms)
        {
            transforms[1] = transforms[1].TrimEnd(')');
            char[] splitters = { ' ', ',' };
            string[] values = transforms[1].Split(splitters);
            XElement renderTransformElement = GetRenderTransformElement(pathElement);
            XElement element = null;
            switch (transforms[0])
            {
                case "matrix":
                    {
                        break;
                    }
                case "rotate":
                    {
                        element = new XElement("RotateTransform");
                        element.SetAttributeValue("Angle", values[0]);
                        if(transforms.Length>1)
                        {
                            element.SetAttributeValue("CenterX", values[1]);
                            element.SetAttributeValue("CenterY", values[2]);
                        }
                        
                        break;
                    }
                case "translate":
                    {
                        element = new XElement("TranslateTransform");
                        element.SetAttributeValue("X", values[0]);
                        if (transforms.Length > 1)
                        {
                            element.SetAttributeValue("Y", values[1]);
                        }
                        break;
                    }
                case "scale":
                    {
                        element = new XElement("ScaleTransform");
                        element.SetAttributeValue("ScaleX", values[0]);
                        if (transforms.Length > 1)
                        {
                            element.SetAttributeValue("ScaleY", values[1]);
                        }
                        else
                            element.SetAttributeValue("ScaleY", values[0]);
                        element.SetAttributeValue("CenterX", values[0]);
                        element.SetAttributeValue("CenterY", values[0]);
                        break;
                    }
                case "skewX":
                    {
                        element = new XElement("SkewTransform");
                        element.SetAttributeValue("AngleX", values[0]);
                        break;
                    }
                case "skewY":
                    {
                        element = new XElement("SkewTransform");
                        element.SetAttributeValue("AngleY", values[0]);
                        break;
                    }
                default: //to do messege for not understanding 3D graphics
                    break;
            }
            renderTransformElement.Add(element);
        }

        private static AttributeParent CheckParentForAttribute(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
            {
                case "Fill": case "Stroke": case "StrokeThickness": case "Opacity": return AttributeParent.Path;
                case "X": case "Y": return AttributeParent.TranslateTransform;


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

        private static XElement GetGeometryElement(XElement parentElement)
        {
            XElement pathDataElement = parentElement.Descendants("Path.Data").First();
            return pathDataElement.Descendants().Last();
        }

        private static XElement GetRenderTransformElement(XElement parentElement)
        {
            return parentElement.Descendants("RenderTransform").First();
        }
    }
}
