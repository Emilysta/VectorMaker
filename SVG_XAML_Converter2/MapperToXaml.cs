using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
                case "Canvas":
                    {
                        resourceElemenet = new XElement(m_xmlnsNamespace + "DataTemplate");
                        resourceElemenet.SetAttributeValue(m_xNamespace + "Key", elementToConvert.Attribute(m_xNamespace + "Name").Value);
                        resourceElemenet.Add(elementToConvert);
                        objectElement = new XElement(m_xmlnsNamespace + "ContentPresenter");
                        objectElement.SetAttributeValue("ContentTemplate", "{StaticResource " + elementToConvert.Attribute(m_xNamespace + "Name").Value + "}");
                        break;
                    }
                default:
                    {
                        resourceElemenet = new XElement("Style");
                        resourceElemenet.SetAttributeValue(m_xNamespace + "Key", elementToConvert.Attribute(m_xNamespace + "Name").Value);
                        resourceElemenet.SetAttributeValue("TargetType", elementToConvert.Name.LocalName);
                        XElement element;
                        foreach (XAttribute xAttribute in elementToConvert.Attributes())
                        {
                            element = new XElement("Setter");
                            resourceElemenet.SetAttributeValue("Property", xAttribute.Name.LocalName);
                            resourceElemenet.SetAttributeValue("Value", xAttribute.Value);
                            resourceElemenet.Add(element);
                        }

                        XElement renderElement = new XElement("Setter");
                        XElement renderValueElement = new XElement("Setter.Value");
                        renderElement.SetAttributeValue("Property", "RenderTransform");
                        renderValueElement.Add(GetTransformGroupElement(elementToConvert));
                        renderElement.Add(renderValueElement);
                        resourceElemenet.Add(renderElement);

                        objectElement = new XElement(m_xmlnsNamespace + " elementToConvert.Name.LocalName");
                        objectElement.SetAttributeValue("Style", "{StaticResource " + elementToConvert.Attribute(m_xNamespace + "Name").Value + "}");
                        break;
                    }

            }

            return (resourceElemenet, objectElement);
        }

        private static XElement FindXamlReferenceWhenNoID(XElement svgElement)
        {
            XElement geometryElement = null;

            switch (svgElement.Name.LocalName) //set markup  reference
            {
                case "rect":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Rectangle");
                        break;
                    }
                case "circle":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Ellipse");
                        break;
                    }
                case "ellipse":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Ellipse");
                        break;
                    }
                case "polyline":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Polyline");
                        break;
                    }
                case "polygon":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Polygon");
                        break;
                    }
                case "path":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Path");
                        break;
                    }
                case "line":
                    {
                        geometryElement = new XElement(m_xmlnsNamespace + "Line");
                        break;
                    }
                case "use":
                    {
                        //if (CheckIfContainsSpecialHrefAttribute(svgElement, out string id))
                        //{
                        //    XElement element = SearchForElementWithId(svgElement, id);
                        //    if (element.Name.LocalName != "g")
                        //    {
                        //        geometryElement = new XElement(m_xmlnsNamespace + "Path");
                        //        geometryElement.SetAttributeValue("Style", "{StaticResource " + id + "}");
                        //        SetAllAttributes(svgElement, geometryElement);
                        //    }
                        //    else
                        //    {
                        //        geometryElement = new XElement(m_xmlnsNamespace + "ContentPresenter");
                        //        geometryElement.SetAttributeValue("ContentTemplate", "{StaticResource " + id + "}");
                        //        SetAllAttributes(svgElement, geometryElement,false);
                        //    }
                        //    return geometryElement;
                        //}
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
                case "image":
                    {
                        break;
                    }
                default:
                    return null;
            }

            SetAllAttributes(svgElement, geometryElement); //set all atributes for whole object 

            return geometryElement;
        }

        private static XElement SearchForElementWithId(XElement element, string id)
        {
            XElement svgMainDocument = SVG_To_XAML.svgDocument.Elements().Where(x => x.Name.LocalName == "svg").First();
            return svgMainDocument.Descendants().Where(x => x.Attribute("id")?.Value == id).FirstOrDefault();
        }

        private static void SetAllAttributes(XElement svgElement, XElement pathElement, bool isNotContentPresenter = true)
        {
            foreach (XAttribute x in svgElement.Attributes())
            {
                FindXAMLAttributeReference(x, pathElement, isNotContentPresenter);
            }
        }

        private static void FindXAMLAttributeReference(XAttribute svgAttribute, XElement geometryElement, bool isNotContentPresenter = true)
        {
            //toDo using with url()
            switch (svgAttribute.Name.LocalName)
            {
                case "style" when isNotContentPresenter:
                    {
                        try
                        {
                            foreach (string s in svgAttribute.Value.Split(';'))
                            {
                                string[] values = s.Split(':');
                                FindXAMLAttributeReference(new XAttribute(values[0], values[1]), geometryElement);
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

                        if (svgAttribute.Value == "none")
                        {
                            geometryElement.SetAttributeValue("Fill", "Tranparent");
                        }
                        else
                        {
                            geometryElement.SetAttributeValue("Fill", svgAttribute.Value);
                        }
                        break;
                    }
                case "fill-opacity" when isNotContentPresenter:
                    {
                        break;
                    }
                case "opacity" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("Stroke", svgAttribute.Value);
                        break;
                    }
                case "stroke-opacity" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("Opacity", svgAttribute.Value);
                        break;
                    }
                case "stroke-linecap" when isNotContentPresenter:
                    {
                        switch (svgAttribute.Value)
                        {
                            case "butt":
                                {
                                    geometryElement.SetAttributeValue("StrokeStartLineCap", "Flat");
                                    geometryElement.SetAttributeValue("StrokeEndLineCap", "Flat");
                                    geometryElement.SetAttributeValue("StrokeDashCap", "Flat");
                                    break;
                                }
                            case "square":
                                {
                                    geometryElement.SetAttributeValue("StrokeStartLineCap", "Square");
                                    geometryElement.SetAttributeValue("StrokeEndLineCap", "Square");
                                    geometryElement.SetAttributeValue("StrokeDashCap", "Square");
                                    break;
                                }
                            case "round":
                                {
                                    geometryElement.SetAttributeValue("StrokeStartLineCap", "Round");
                                    geometryElement.SetAttributeValue("StrokeEndLineCap", "Round");
                                    geometryElement.SetAttributeValue("StrokeDashCap", "Round");
                                    break;
                                }
                        }
                        break;
                    }
                case "stroke-linejoin" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("StrokeLineJoin", char.ToUpper(svgAttribute.Value[0]) + svgAttribute.Value[1..]);
                        break;
                    }
                case "stroke-dasharray" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("StrokeDashArray", svgAttribute.Value.Replace(',', ' '));
                        break;
                    }
                case "stroke-dashoffset" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("StrokeDashOffset", svgAttribute.Value);
                        break;
                    }
                case "stroke-miterlimit" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("StrokeMiterLimit", svgAttribute.Value);
                        break;
                    }
                case "stroke-width" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("StrokeThickness", svgAttribute.Value);
                        break;
                    }
                case "x":
                    {
                        XElement renderTransformElement = GetTransformGroupElement(geometryElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("X", svgAttribute.Value);
                        renderTransformElement.Add(element);
                        break;
                    }
                case "y":
                    {
                        XElement renderTransformElement = GetTransformGroupElement(geometryElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("Y", svgAttribute.Value);
                        renderTransformElement.Add(element);
                        break;
                    }
                case "x1":
                    {
                        geometryElement.SetAttributeValue("X1", svgAttribute.Value);
                        break;
                    }
                case "y1":
                    {
                        geometryElement.SetAttributeValue("Y1", svgAttribute.Value);
                        break;
                    }
                case "x2":
                    {
                        geometryElement.SetAttributeValue("X2", svgAttribute.Value);
                        break;
                    }
                case "y2":
                    {
                        geometryElement.SetAttributeValue("Y2", svgAttribute.Value);
                        break;
                    }
                case "cx" when isNotContentPresenter:
                    {
                        XElement renderTransformElement = GetTransformGroupElement(geometryElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("X", svgAttribute.Value);
                        renderTransformElement.Add(element);
                        break;
                    }
                case "cy" when isNotContentPresenter:
                    {
                        XElement renderTransformElement = GetTransformGroupElement(geometryElement);
                        XElement element = new XElement(m_xmlnsNamespace + "TranslateTransform");
                        element.SetAttributeValue("Y", svgAttribute.Value);
                        renderTransformElement.Add(element);
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
                        if (geometryElement.Name.LocalName == "Rectangle")
                        {
                            geometryElement.SetAttributeValue("RadiusX", svgAttribute.Value);
                        }
                        else
                        {
                            geometryElement.SetAttributeValue("Width", svgAttribute.Value);
                        }

                        break;
                    }
                case "ry" when isNotContentPresenter:
                    {
                        if (geometryElement.Name.LocalName == "Rectangle")
                        {
                            geometryElement.SetAttributeValue("RadiusY", svgAttribute.Value);
                        }
                        else
                        {
                            geometryElement.SetAttributeValue("Height", svgAttribute.Value);
                        }
                        break;
                    }
                case "r" when isNotContentPresenter:
                    {
                        if (geometryElement.Name.LocalName == "Ellipse")
                        {
                            geometryElement.SetAttributeValue("Height", svgAttribute.Value);
                            geometryElement.SetAttributeValue("Width", svgAttribute.Value);
                        }
                        else
                        {
                            //toDo radial Gradient
                            geometryElement.SetAttributeValue("RadiusX", svgAttribute.Value);
                            geometryElement.SetAttributeValue("RadiusY", svgAttribute.Value);
                        }
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
                            geometryElement.SetAttributeValue("Style", "{StaticResource " + svgAttribute.Value.TrimStart('#') + "}");
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
                            SetTransformAttributes(geometryElement, propertiesOfTransform);
                        }
                        break;
                    }
                case "d" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("Data", svgAttribute.Value);
                        break;
                    }
                case "points" when isNotContentPresenter:
                    {
                        geometryElement.SetAttributeValue("Points", svgAttribute.Value);
                        break;
                    }
                case "id":
                    {
                        geometryElement.SetAttributeValue(m_xNamespace + "Name", svgAttribute.Value);
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
                        if (values.Length > 1)
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
                        if (values.Length > 1)
                        {
                            element.SetAttributeValue("Y", values[1]);
                        }
                        break;
                    }
                case "scale":
                    {
                        element = new XElement(m_xmlnsNamespace + "ScaleTransform");
                        element.SetAttributeValue("ScaleX", values[0]);
                        if (values.Length > 1)
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
    }
}
