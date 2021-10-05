namespace VectorMaker
{
    public class ColorPropertyPair
    {
        private int m_maximum;
        private float m_floatProperty;
        private int m_intProperty;

        public float FloatProperty => m_floatProperty;
        public int IntProperty => m_intProperty;

        public ColorPropertyPair(int intProperty, int maximum)
        {
            m_maximum = maximum;
            SetIntProperty(intProperty);
        }

        public ColorPropertyPair(float floatProperty, int maximum)
        {
            m_maximum = maximum;
            SetFloatProperty(floatProperty);
        }

        public void SetIntProperty(int value)
        {
            m_intProperty = value;
            m_floatProperty = (float)value / m_maximum;
        }

        public void SetFloatProperty(float value)
        {
            m_floatProperty = value;
            m_intProperty = (int)(value * m_maximum);
        }
    }
}
