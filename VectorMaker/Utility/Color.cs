using System;
namespace VectorMaker
{
    /// <summary>
    /// This clas defines color in HSL model.
    /// Currently not used in app build.
    /// </summary>
    public class HSLColorDefinition
    {
        public ColorPropertyPair H = new ColorPropertyPair(0, 360);
        public ColorPropertyPair S = new ColorPropertyPair(0, 100);
        public ColorPropertyPair L = new ColorPropertyPair(0, 100);

        public void SetColorProperties(int h, int s, int l)
        {
            H.SetIntProperty(h);
            S.SetIntProperty(s);
            L.SetIntProperty(l);
        }

        public void SetColorProperties(float h, float s, float l)
        {
            H.SetFloatProperty(h);
            S.SetFloatProperty(s);
            L.SetFloatProperty(l);
        }

        public HSLColorDefinition() { }
    }

    /// <summary>
    /// This clas defines color in RGB model.
    /// Currently not used in app build.
    /// </summary>
    public class RGBColorDefinition
    {
        public ColorPropertyPair R = new ColorPropertyPair(0, 255);
        public ColorPropertyPair G = new ColorPropertyPair(0, 255);
        public ColorPropertyPair B = new ColorPropertyPair(0, 255);

        public void SetColorProperties(int r, int g, int b)
        {
            R.SetIntProperty(r);
            G.SetIntProperty(g);
            B.SetIntProperty(b);
        }

        public void SetColorProperties(float r, float g, float b)
        {
            R.SetFloatProperty(r);
            G.SetFloatProperty(g);
            B.SetFloatProperty(b);
        }

        public RGBColorDefinition() { }
    }

    /// <summary>
    /// This clas defines color as RGB and HSL at once. Used in <see cref="ControlsResources.ColorsPicker"/>
    /// Currently not used in app build.
    /// </summary>
    public class Color
    {
        private HSLColorDefinition m_hslColor;
        private RGBColorDefinition m_rgbColor;
        private ColorPropertyPair m_a = new ColorPropertyPair(255,255);
        private System.Windows.Media.Color m_colorInWindowsFormat;
        private string m_hexColor;

        public HSLColorDefinition HSLColor
        {
            get { return m_hslColor; }
            set { m_hslColor = value; }
        }

        public RGBColorDefinition RGBColor
        {
            get { return m_rgbColor; }
            set { m_rgbColor = value; }
        }

        public ColorPropertyPair A
        {
            get { return m_a; }
            set { m_a = value; }
        }

        public string HexColor
        {
            get { return m_hexColor; }
            set { m_hexColor = value; }
        }

        public System.Windows.Media.Color ColorInWindowsFormat
        {
            get { return m_colorInWindowsFormat; }
            set { m_colorInWindowsFormat = value;
                CalculateColorParametersFromWindowsColor();
            }
        }

        public Color(int rh, int gs, int bl, int a = 255, bool isThisHSLColor = false)
        {
            RGBColor = new RGBColorDefinition();
            HSLColor = new HSLColorDefinition();
            A.SetIntProperty(a);
            SetColors(rh, gs, bl, isThisHSLColor);
            SetColorInWindowsFormat();
        }

        public Color() 
        {
            RGBColor = new RGBColorDefinition();
            HSLColor = new HSLColorDefinition();
        }

        public void SetColors(int rh, int gs, int bl, bool isThisHSLColor)
        {
            if (isThisHSLColor)
            {
                HSLColor.SetColorProperties(rh, gs, bl);
                CalculateRGBFromHSL();
            }
            else
            {
                RGBColor.SetColorProperties(rh, gs, bl);
                CalculateHSLFromRGB();
            }
            
        }

        public void SetColors(float rh, float gs, float bl, bool isThisHSLColor)
        {
            if (isThisHSLColor)
            {
                HSLColor.SetColorProperties(rh, gs, bl);
                CalculateRGBFromHSL();
            }
            else
            {
                RGBColor.SetColorProperties(rh, gs, bl);
                CalculateHSLFromRGB();
            }
            SetColorInWindowsFormat();
        }

        private void CalculateHSLFromRGB()
        {
            float r = RGBColor.R.FloatProperty;
            float g = RGBColor.G.FloatProperty;
            float b = RGBColor.B.FloatProperty;

            float maximum = Math.Max(r, Math.Max(g, b));
            float minimum = Math.Min(r, Math.Min(g, b));

            float maxMinDiff = maximum - minimum;
            float maxMinSum = maximum + minimum;
            HSLColor.L.SetFloatProperty( maxMinSum / 2.0f);

            if (maxMinDiff <= 0)
            {
                HSLColor.S.SetFloatProperty(0.0f);
                HSLColor.H.SetFloatProperty(0.0f);
            }
            else
            {
                if (HSLColor.L.FloatProperty <= 0.5f)
                    HSLColor.S.SetFloatProperty((maxMinDiff) / (maxMinSum));
                else
                    HSLColor.S.SetFloatProperty((maxMinDiff) / (2 - maxMinSum));

                float tempH;

                if (r == maximum)
                    tempH = (g - b) / maxMinDiff;
                else if (g == maximum)
                    tempH = 2 + (b - r) / maxMinDiff;
                else
                    tempH = 4 + (r - g) / maxMinDiff;

                tempH = tempH * 60;

                if (tempH < 0)
                    tempH += 360;

                HSLColor.H.SetIntProperty((int)tempH);
            }
        }

        private void CalculateRGBFromHSL()
        {
            if (HSLColor.S.FloatProperty == 0)
            {
                RGBColor.R.SetIntProperty((int)(HSLColor.L.FloatProperty * 255));
                RGBColor.G.SetIntProperty((int)(HSLColor.L.FloatProperty * 255));
                RGBColor.B.SetIntProperty((int)(HSLColor.L.FloatProperty * 255));
            }
            else
            {
                float tempValue;
                float tempLFloatProperty = HSLColor.L.FloatProperty;
                if (tempLFloatProperty < 0.5)
                    tempValue = tempLFloatProperty * (1 + HSLColor.S.FloatProperty);
                else
                    tempValue = tempLFloatProperty + HSLColor.S.FloatProperty - tempLFloatProperty * HSLColor.S.FloatProperty;

                float tempValue2 = 2 * tempLFloatProperty - tempValue;

                float r = HSLColor.H.FloatProperty + 0.333f;
                float g = HSLColor.H.FloatProperty;
                float b = HSLColor.H.FloatProperty - 0.333f;

                ClampValues(ref r);
                ClampValues(ref g);
                ClampValues(ref b);

                TestColor(ref r, tempValue, tempValue2);
                TestColor(ref g, tempValue, tempValue2);
                TestColor(ref b, tempValue, tempValue2);

                RGBColor.R.SetFloatProperty(r);
                RGBColor.G.SetFloatProperty(g);
                RGBColor.B.SetFloatProperty(b);
            }
        }

        private void ClampValues(ref float value)
        {
            if (value < 0)
                value += 1;
            else if (value > 1)
                value -= 1;
        }

        private void TestColor(ref float valueToTest, float tempValue, float tempValue2)
        {
            if (6 * valueToTest < 1)
            {
                valueToTest = tempValue2 + (tempValue - tempValue2) * 6 * valueToTest;
            }
            else if (2 * valueToTest < 1)
            {
                valueToTest = tempValue;
            }
            else if (3 * valueToTest < 2)
            {
                valueToTest = tempValue2 + (tempValue - tempValue2) * (0.666f - valueToTest) * 6;
            }
            else
                valueToTest = tempValue2;
        }

        private void SetColorInWindowsFormat()
        {
            ColorInWindowsFormat = System.Windows.Media.Color.FromArgb((byte)A.IntProperty,
                (byte)RGBColor.R.IntProperty, (byte)RGBColor.G.IntProperty, (byte)RGBColor.B.IntProperty);
        }

        private void CalculateColorParametersFromWindowsColor()
        {
            A.SetIntProperty(ColorInWindowsFormat.A);
            SetColors((int)ColorInWindowsFormat.R, (int)ColorInWindowsFormat.G, (int)ColorInWindowsFormat.B, false);
            HexColor = ColorInWindowsFormat.ToString();
        }
    }
}
