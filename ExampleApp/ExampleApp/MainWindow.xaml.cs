using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace ExampleApp
{
    /// <summary>
    /// Resolution Independent Example App based on Stackoverflow,
    /// https://stackoverflow.com/questions/3193339/tips-on-developing-resolution-independent-application/5000120
    /// but modified for handle higher resolution and DPI scaling
    /// 
    /// This program works like zoom in or out your entire grid depends on display properties
    /// Not so perfect for every display, sometimes cut taskbar but contents.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region ScaleValue Depdency Property
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register(
            "ScaleValue",
            typeof(double),
            typeof(MainWindow),
            new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue))
            );

        // Target resolution is FHD display
        // but scaling is Grid element dependent
        public static readonly double BaseResolution_Width = 1924.0; // 1920 + 4
        public static readonly double BaseResolution_Height = 1028.0; // 1024 + 4

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }
        #endregion

        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double DPIScaleFactor = 1.0;
            double xScale = 1.0;
            double yScale = 1.0;
            double value = 1.0;

            // This is current resolution of primary screen, not affected by DPI or others
            Rectangle DisplayResolution = Screen.PrimaryScreen.Bounds;

            // This is for get current DPI setting
            PresentationSource src = PresentationSource.FromVisual(this);

            if (src != null)
            {
                DPIScaleFactor = src.CompositionTarget.TransformToDevice.M11;
            }

            // Rendered size is bigger than base resolution
            if (DisplayResolution.Width > BaseResolution_Width ||
                DisplayResolution.Height > BaseResolution_Height)
            {
                xScale = DisplayResolution.Width / BaseResolution_Width;
                yScale = DisplayResolution.Height / BaseResolution_Height;
            }
            else // Rendered size is smaller than base resolution
            {
                xScale = ActualWidth / BaseResolution_Width;
                yScale = ActualHeight / BaseResolution_Height;
            }
            
            value = Math.Min(xScale, yScale);

            // Adjust with DPI rate, for high resolution
            if (DisplayResolution.Width > BaseResolution_Width)
            {
                value /= DPIScaleFactor;
            }

            LabelTest.Content =
                "DisplayWidth : " + DisplayResolution.Width +
                "\nDisplayHeight : " + DisplayResolution.Height +
                "\nDPI Scaling : x" + DPIScaleFactor +
                "\nBaseWidth : " + BaseResolution_Width +
                "\nBaseHeight : " + BaseResolution_Height +
                "\nxScale : " + xScale +
                "\nyScale : " + yScale +
                "\nCalculated Scale : x" + value +
                "\nActualWidth (Rendered width) : " + ActualWidth +
                "\nActualHeight (Rendered height) : " + ActualHeight;

            ScaleValue = (double)OnCoerceScaleValue(myMainWindow, value);
        }
    }
}
