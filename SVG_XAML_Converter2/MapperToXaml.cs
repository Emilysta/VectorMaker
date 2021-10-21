using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Drawing;

namespace SVG_XAML_Converter_Lib
{
    public enum AttributeParent
    {
        Path,
        Geometry,
        TranslateTransform

    }

    public static class MapperToXaml
    {
        private static XNamespace m_xmlnsNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static XNamespace m_xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        private static XNamespace m_presentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options"; 

        public static List<XElement> FindXAMLObjectReference(XElement svgElement)
        {
            if (CheckIfContainsIdAttribute(svgElement))
            {
                return FindXamlReferenceWhenID(svgElement);
            }
            else
            {
                return new List<XElement> { FindXamlReferenceWhenNoID(svgElement) };

            }
        }

        private static List<XElement> FindXamlReferenceWhenID(XElement svgElement)
        {
            XElement elementToConvert = FindXamlReferenceWhenNoID(svgElement);
            (XElement, XElement) resource = ConvertToResource(elementToConvert);
            return new List<XElement>() { resource.Item1, resource.Item2 };

        }

        private static (XElement, XElement) ConvertToResource(XElement elementToConvert)
        {
            XElement resourceElemenet = null;
            XElement objectElement = null;
            switch (elementToConvert.Name.LocalName)
            {
                case "Path":
                    {
                        resourceElemenet = new XElement("Style");
                        resourceElemenet.SetAttributeValue(m_xNamespace + "Key", elementToConvert.Attribute(m_xNamespace + "Name").Value);
                        resourceElemenet.SetAttributeValue("TargetType", "Path");
                        XElement element;
                        foreach (XAttribute xAttribute in elementToConvert.Attributes())
                        {
                            element = new XElement("Setter");
                            resourceElemenet.SetAttributeValue("Property", xAttribute.Name.LocalName);
                            resourceElemenet.SetAttributeValue("Value", xAttribute.Value);
                            resourceElemenet.Add(element);
                        }
                        XElement dataElement = new XElement("Setter");
                        XElement dataValueElement = new XElement("Setter.Value");
                        dataElement.SetAttributeValue("Property", "Data");
                        dataValueElement.Add(GetPathDataChild(elementToConvert));
                        dataElement.Add(dataValueElement);

                        XElement renderElement = new XElement("Setter");
                        XElement renderValueElement = new XElement("Setter.Value");
                        renderElement.SetAttributeValue("Property", "RenderTransform");
                        renderValueElement.Add(GetTransformGroupElement(elementToConvert));
                        renderElement.Add(renderValueElement);
                        resourceElemenet.Add(dataElement);
                        resourceElemenet.Add(renderElement);

                        objectElement = new XElement(m_xmlnsNamespace + "Path");
                        objectElement.SetAttributeValue("Style", "{StaticResource " + elementToConvert.Attribute(m_xNamespace + "Name").Value + "}");
                        break;
                    }
                case "Canvas":
                    {
                        resourceElemenet = new XElement(m_xmlnsNamespace + "DataTemplate");
                        resourceElemenet.SetAttributeValue(m_xNamespace + "Key", elementToConvert.Attribute(m_xNamespace + "Name").Value);
                        resourceElemenet.Add(elementToConvert);
                        objectElement = new XElement(m_xmlnsNamespace + "ContentPresenter");
                        objectElement.SetAttributeValue("ContentTemplate", "{StaticResource " + elementToConvert.Attribute(m_xNamespace + "Name").Value + "}");
                        break;
                    }
            }

            return (resourceElemenet, objectElement);
        }

        private static XElement FindXamlReferenceWhenNoID(XElement svgElement)
        {
            CreateDefaultPathElement(out XElement pathElement, out XElement pathDataElement);
            XElement baseElementForGeometry = null;
            XElement geometryElement = null;

            switch (svgElement.Name.LocalName) //set markup  reference
            {
                case "rect":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Rect");
                        baseElementForGeometry = new XElement(m_xmlnsNamespace + "RectangleGeometry",
                            new XElement(m_xmlnsNamespace + "RectangleGeometry.Rect",
                            geometryElement)
                            );
                        break;
                    }
                case "circle":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "EllipseGeometry");
                        break;
                    }
                case "ellipse":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "EllipseGeometry");
                        break;
                    }
                case "polyline":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "PolyLineSegment");
                        baseElementForGeometry = new XElement(m_xmlnsNamespace + "PathGeometry",
                            new XElement(m_xmlnsNamespace + "PathGeometry.Figures",
                                new XElement(m_xmlnsNamespace + "PathFigure", geometryElement)
                                )
                            );
                        break;
                    }
                case "polygon":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "PolyLineSegment");
                        XElement pathFigure = new XElement(m_xmlnsNamespace + "PathFigure");
                        pathFigure.SetAttributeValue("IsClosed", "True");
                        pathFigure.Add(geometryElement);
                        baseElementForGeometry = new XElement(m_xmlnsNamespace + "PathGeometry",
                            new XElement(m_xmlnsNamespace + "PathGeometry.Figures", pathFigure)
                            );
                        break;
                    }
                case "path":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "PathGeometry");
                        break;
                    }
                case "use":
                    {
                        if (CheckIfContainsSpecialHrefAttribute(svgElement, out string id))
                        {
                            XElement element = SearchForElementWithId(svgElement, id);
                            if (element.Name.LocalName != "g")
                            {
                                geometryElement = new XElement(m_xmlnsNamespace + "Path");
                                geometryElement.SetAttributeValue("Style", "{StaticResource " + id + "}");
                                SetAllAttributes(svgElement, geometryElement);
                            }
                            else
                            {
                                geometryElement = new XElement(m_xmlnsNamespace + "ContentPresenter");
                                geometryElement.SetAttributeValue("ContentTemplate", "{StaticResource " + id + "}");
                                SetAllAttributes(svgElement, geometryElement,false);
                            }
                            return geometryElement;
                        }
                        return null;
                    }
                case "g":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Canvas");
                        geometryElement.SetAttributeValue("Tag", "group");
                        SetAllAttributes(svgElement, geometryElement);
                        return geometryElement;
                    }
                case "text":
                    {
                        throw new NotImplementedException();
                        //toDo texts

                    }
                case "style":
                    {
                        //toDo display warning : style block is not supported
                        break;
                    }
                case "glyph":
                    {
                        //toDo depreceated
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

            pathElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").FirstOrDefault()?.Attributes("xmlns")?.Remove(); //remove namespace for child Path.Data

            return pathElement;
        }

        private static void CreateDefaultPathElement(out XElement pathElement, out XElement pathDataElement)
        {
            pathElement = new XElement(m_xmlnsNamespace + "Path");
            //pathElement.SetAttributeValue(m_presentationNamespace + "Freeze", "true");
            pathElement.SetAttributeValue("Fill", "Black");
            pathDataElement = new XElement(m_xmlnsNamespace + "Path.Data");
            XElement renderTransformElement = new XElement(m_xmlnsNamespace + "Path.RenderTransform");
            XElement transformGroupElement = new XElement(m_xmlnsNamespace + "TransformGroup");

            renderTransformElement.Add(transformGroupElement);
            pathElement.Add(renderTransformElement);
        }

        private static XElement SearchForElementWithId(XElement element, string id)
        {
            XElement svgMainDocument = SVG_To_XAML.svgDocument.Elements().Where(x => x.Name.LocalName == "svg").First();
            return svgMainDocument.Descendants().Where(x => x.Attribute("id")?.Value == id).FirstOrDefault();
        }

        private static void SetAllAttributes(XElement svgElement, XElement pathElement,bool isNotContentPresenter = true)
        {
            foreach (XAttribute x in svgElement.Attributes())
            {
                FindXAMLAttributeReference(x, pathElement,isNotContentPresenter);
            }
        }

        private static void FindXAMLAttributeReference(XAttribute svgAttribute, XElement pathElement, bool isNotContentPresenter = true)
        {
            XElement geometryElement = GetGeometryElement(pathElement);
            switch (svgAttribute.Name.LocalName)
            {
                case "style" when isNotContentPresenter: 
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
                case "fill" when isNotContentPresenter: 
                    {
                        if (svgAttribute.Value != "none")
                            pathElement.SetAttributeValue("Fill",
                                char.ToUpper(svgAttribute.Value[0]) + svgAttribute.Value[1..]);
                        else
                            pathElement.SetAttributeValue("Fill", "Transparent");
                        break;
                    }
                case "fill-opacity" when isNotContentPresenter: 
                    {
                        break;
                    }
                case "opacity" when isNotContentPresenter:
                    {
                        pathElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke" when isNotContentPresenter:
                    {
                        pathElement.SetAttributeValue("Stroke", svgAttribute.Value);
                        break;
                    }
                case "stroke-opacity" when isNotContentPresenter:
                    {
                        pathElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke-width" when isNotContentPresenter:
                    {
                        pathElement.SetAttributeValue("StrokeThickness", svgAttribute.Value);
                        break;
                    }
                case "x":
                    {
                        XElement renderTransformElement = GetTransformGroupElement(pathElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("X", svgAttribute.Value);
                        renderTransformElement.Add(element);
                        break;
                    }
                case "y":
                    {
                        XElement renderTransformElement = GetTransformGroupElement(pathElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("Y", svgAttribute.Value);
                        renderTransformElement.Add(element);
                        break;
                    }
                case "cx" when isNotContentPresenter:
                    {
                        XAttribute centerAttribute = geometryElement.Attributes().Where(x => x.Name.LocalName == "Center").FirstOrDefault();
                        if (centerAttribute != null)
                        {
                            Point point = ParsePoint(centerAttribute.Value);
                                geometryElement.SetAttributeValue("Center", svgAttribute.Value + "," + point.Y);
                            
                        }
                        else
                            geometryElement.SetAttributeValue("Center", svgAttribute.Value + ",0");
                        break;
                    }
                case "cy" when isNotContentPresenter:
                    {
                        XAttribute centerAttribute = geometryElement.Attributes().Where(x => x.Name.LocalName == "Center").FirstOrDefault();
                        if (centerAttribute != null)
                        {
                            Point point = ParsePoint(centerAttribute.Value);
                            geometryElement.SetAttributeValue("Center", point.X + "," + svgAttribute.Value);

                        }
                        else
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
                case "rx" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("RadiusX", svgAttribute.Value);
                        break;
                    }
                case "ry" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("RadiusY", svgAttribute.Value);
                        break;
                    }
                case "r" when isNotContentPresenter:
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
                case "href" when isNotContentPresenter:
                    {
                        if (svgAttribute.Value.Contains('#'))
                            pathElement.SetAttributeValue("Style", "{StaticResource " + svgAttribute.Value.TrimStart('#') + "}");
                        else
                        {
                            //todo hrefs nieobsługiwane linki sieciowe
                        }
                        break;
                    }
                case "transform":
                    {
                        string[] transforms = svgAttribute.Value.Split();
                        foreach (string transform in transforms)
                        {
                            string[] propertiesOfTransform = transform.Split('(');
                            SetTransformAttributes(pathElement, propertiesOfTransform);
                        }
                        break;
                    }
                case "d" when isNotContentPresenter: 
                    {
                        geometryElement.SetAttributeValue("Figures", svgAttribute.Value);
                        break;
                    }
                case "points" when isNotContentPresenter:
                    {
                        string startPoint = svgAttribute.Value.Split(' ')[0];
                        geometryElement.SetAttributeValue("Points", svgAttribute.Value);
                        geometryElement.Parent.SetAttributeValue("StartPoint", startPoint);
                        break;
                    }
                case "id":
                    {
                        pathElement.SetAttributeValue(m_xNamespace + "Name", svgAttribute.Value);
                        break;
                    }
                default: break;
            }
        }

        private static void SetTransformAttributes(XElement pathElement, string[] transforms)
        {
            transforms[1] = transforms[1].TrimEnd(')');
            char[] splitters = { ' ', ',' };
            string[] values = transforms[1].Split(splitters);
            XElement renderTransformElement = GetTransformGroupElement(pathElement);
            XElement element = null;
            switch (transforms[0])
            {
                case "matrix":
                    {
                        break;
                    }
                case "rotate":
                    {
                        element = new XElement(m_xmlnsNamespace + "RotateTransform");
                        element.SetAttributeValue("Angle", values[0]);
                        if (transforms.Length > 1)
                        {
                            element.SetAttributeValue("CenterX", values[1]);
                            element.SetAttributeValue("CenterY", values[2]);
                        }

                        break;
                    }
                case "translate":
                    {
                        element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("X", values[0]);
                        if (transforms.Length > 1)
                        {
                            element.SetAttributeValue("Y", values[1]);
                        }
                        break;
                    }
                case "scale":
                    {
                        element = new XElement(m_xmlnsNamespace + "ScaleTransform");
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
                        element = new XElement(m_xmlnsNamespace + "SkewTransform");
                        element.SetAttributeValue("AngleX", values[0]);
                        break;
                    }
                case "skewY":
                    {
                        element = new XElement(m_xmlnsNamespace + "SkewTransform");
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

        private static bool CheckIfContainsSpecialHrefAttribute(XElement element, out string id)
        {
            id = "";
            XAttribute xAttribute = element.Attributes().Where(x => x.Name.LocalName == "href").FirstOrDefault();
            //XAttribute xlinkHrefAttribute = element.Attribute("xlink:href");
            if (xAttribute != null && xAttribute.Value[0] == '#')
            {
                id = xAttribute.Value[1..];
                return true;
            }

            //if (xlinkHrefAttribute != null && xlinkHrefAttribute.Value.Contains('#'))
            //{
            //    id = xAttribute.Value[1..];
            //    return true;
            //}

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
            XElement pathDataElement = parentElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").FirstOrDefault();
            return pathDataElement?.Descendants().Last() ?? parentElement;
        }

        private static XElement GetTransformGroupElement(XElement parentElement)
        {
            XElement transformGroupElement = parentElement.Descendants().Where(x => x.Name.LocalName == "TransformGroup").FirstOrDefault();
            if (transformGroupElement == null)
            {
                XElement renderTransform = new XElement(m_xmlnsNamespace + parentElement.Name.LocalName + ".RenderTransform");
                transformGroupElement = new XElement(m_xmlnsNamespace + "TransformGroup");
                renderTransform.Add(transformGroupElement);
                parentElement.Add(renderTransform);
            }
            return transformGroupElement;
        }

        private static XElement GetPathDataChild(XElement parentElement)
        {
            XElement pathDataElement = parentElement.Descendants().Where(x => x.Name.LocalName == "Path.Data").FirstOrDefault();
            return pathDataElement.Elements().FirstOrDefault();
        }

        private static Point ParsePoint(string pointText)
        {
            string[] textValues = pointText.Split(',');
            int[] values = new int[2];
            values[0] = int.Parse(textValues[0]);
            values[1] = int.Parse(textValues[1]);
            return new Point(values[0], values[1]);
        }
    }
}
