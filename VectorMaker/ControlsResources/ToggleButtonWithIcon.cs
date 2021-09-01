using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using MahApps.Metro.IconPacks;


namespace VectorMaker.ControlsResources
{
    public class ToggleButtonWithIcon : ToggleButton
    {
        private static DependencyProperty m_buttonCornerRadius = DependencyProperty.Register("ButtonCornerRadius", typeof(CornerRadius), typeof(ToggleButtonWithIcon), new PropertyMetadata(new CornerRadius(0,0,0,0)));

        private static DependencyProperty m_imageWidth = DependencyProperty.Register("IconWidth", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(15));

        private static DependencyProperty m_imageHeight = DependencyProperty.Register("IconHeight", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(15));

        private static DependencyProperty m_iconRow = DependencyProperty.Register("IconRow", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(0));

        private static DependencyProperty m_iconColumn = DependencyProperty.Register("IconColumn", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(0));

        private static DependencyProperty m_iconRowSpan = DependencyProperty.Register("IconRowSpan", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(1));

        private static DependencyProperty m_iconColumnSpan = DependencyProperty.Register("IconColumnSpan", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(1));

        private static DependencyProperty m_contentRow = DependencyProperty.Register("ContentRow", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(0));

        private static DependencyProperty m_contentColumn = DependencyProperty.Register("ContentColumn", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(0));

        private static DependencyProperty m_contentRowSpan = DependencyProperty.Register("ContentRowSpan", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(1));

        private static DependencyProperty m_contentColumnSpan = DependencyProperty.Register("ContentColumnSpan", typeof(int), typeof(ToggleButtonWithIcon), new PropertyMetadata(1));

        private static DependencyProperty m_content = DependencyProperty.Register("ButtonContent", typeof(string), typeof(ToggleButtonWithIcon), new PropertyMetadata(""));

        private static DependencyProperty m_iconKind = DependencyProperty.Register("IconKind", typeof(PackIconBootstrapIconsKind), typeof(ToggleButtonWithIcon), new PropertyMetadata(PackIconBootstrapIconsKind.None));

        private static DependencyProperty m_isChecked = ToggleButton.IsCheckedProperty.AddOwner(typeof(ToggleButtonWithIcon));

        static ToggleButtonWithIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonWithIcon), new FrameworkPropertyMetadata(typeof(ToggleButtonWithIcon)));    
        }

        protected override void OnInitialized(EventArgs e) //override On Initialized Event Rised after initialization of Parent
        {
            base.OnInitialized(e);
            string iconKind = this.IconKind.ToString(); //getting value of own property
            string buttonContent = this.ButtonContent.ToString();
            SetColumnsVisualPart(buttonContent, iconKind);
        }

        public CornerRadius ButtonCornerRadius //implement Wrapper
        {
            get { return (CornerRadius)GetValue(m_buttonCornerRadius); }
            set { SetValue(m_buttonCornerRadius, value); }
        }

        public int IconWidth
        {
            get { return (int)GetValue(m_imageWidth); }
            set { SetValue(m_imageWidth, value); }
        }

        public int IconHeight
        {
            get { return (int)GetValue(m_imageHeight); }
            set { SetValue(m_imageHeight, value); }
        }

        public int IconRow
        {
            get { return (int)GetValue(m_iconRow); }
            set { SetValue(m_iconRow, value); }
        }

        public int IconColumn
        {
            get { return (int)GetValue(m_iconColumn); }
            set { SetValue(m_iconColumn, value); }
        }

        public int IconRowSpan
        {
            get { return (int)GetValue(m_iconRowSpan); }
            set { SetValue(m_iconRowSpan, value); }
        }

        public int IconColumnSpan
        {
            get { return (int)GetValue(m_iconColumnSpan); }
            set { SetValue(m_iconColumnSpan, value); }
        }

        public int ContentRow
        {
            get { return (int)GetValue(m_contentRow); }
            set { SetValue(m_contentRow, value); }
        }

        public int ContentColumn
        {
            get { return (int)GetValue(m_contentColumn); }
            set { SetValue(m_contentColumn, value); }
        }

        public int ContentRowSpan
        {
            get { return (int)GetValue(m_contentRowSpan); }
            set { SetValue(m_contentRowSpan, value); }
        }

        public int ContentColumnSpan
        {
            get { return (int)GetValue(m_contentColumnSpan); }
            set { SetValue(m_contentColumnSpan, value); }
        }

        public string ButtonContent
        {
            get { return (string)GetValue(m_content); }
            set { SetValue(m_content, value); }
        }

        public PackIconBootstrapIconsKind IconKind
        {
            get { return (PackIconBootstrapIconsKind)GetValue(m_iconKind); }
            set { SetValue(m_iconKind, value); }
        }

        public new bool IsChecked
        {
            get { return (bool)GetValue(m_isChecked); }
            set { SetValue(m_contentColumn, value); }
        }

        private void SetColumnsVisualPart(string buttonContent, string iconKind)
        {
            if (iconKind != PackIconBootstrapIconsKind.None.ToString()
                && buttonContent == "")
            {
                IconColumnSpan = 2;
            }
            else if (iconKind == PackIconBootstrapIconsKind.None.ToString()
                && buttonContent != "")
            {
                ContentColumnSpan = 2;
            }
            else if (iconKind != PackIconBootstrapIconsKind.None.ToString()
                 && buttonContent != "")
            {
                ContentColumn = 1;
                IconHeight = (int)(IconHeight * 0.8f);
                IconWidth = (int)(IconWidth * 0.8f);
                FontSize = FontSize * 0.8;
            }
        }
    }
}
