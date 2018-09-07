using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public partial class RatingControl : Control
    {
        public RatingControl()
        {
        }

        static RatingControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), new FrameworkPropertyMetadata(typeof(RatingControl)));
        }

        public int RatingValue
        {
            get { return (int)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int), typeof(RatingControl), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true
            });

        public double StarWidth
        {
            get { return (double)GetValue(StarWidthProperty); }
            set { SetValue(StarWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StarWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StarWidthProperty =
            DependencyProperty.Register("StarWidth", typeof(double), typeof(RatingControl), new PropertyMetadata(90.0));

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />
        /// </summary>
        /// <remarks>
        /// Finds the buttons inside the template and assigns handlers to set the rating
        /// </remarks>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Return if this rating is null
            //TODO: Create PArt_Rating1 etc in template.
            ToggleButton rating1 = GetTemplateChild("rating1") as ToggleButton;
            Button clearrating = GetTemplateChild("clearrating") as Button;
            if (rating1 == null || clearrating == null) return;

            ToggleButton rating2 = GetTemplateChild("rating2") as ToggleButton;
            ToggleButton rating3 = GetTemplateChild("rating3") as ToggleButton;
            ToggleButton rating4 = GetTemplateChild("rating4") as ToggleButton;
            ToggleButton rating5 = GetTemplateChild("rating5") as ToggleButton;            

            clearrating.Click += (s, e) => { RatingIconClicked(0, e); };
            rating1.Click += (s, e) => { RatingIconClicked(1, e); };
            rating2.Click += (s, e) => { RatingIconClicked(64, e); };
            rating3.Click += (s, e) => { RatingIconClicked(128, e); };
            rating4.Click += (s, e) => { RatingIconClicked(196, e); };
            rating5.Click += (s, e) => { RatingIconClicked(255, e); };
        }

        private void RatingIconClicked(byte btnIndex, RoutedEventArgs e)
        {
            if (RatingValue != btnIndex)
                SetValue(RatingValueProperty, (int)btnIndex);
            else
                SetValue(RatingValueProperty, (int)RatingValue);
        }
    }

    public class RatingRangeControl : RatingControl
    {
        public RatingRangeControl()
        {
            
        }

        static RatingRangeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingRangeControl), new FrameworkPropertyMetadata(typeof(RatingRangeControl)));
        }

        public int RangeLower
        {
            get { return (int)GetValue(RangeLowerProperty); }
            set { SetValue(RangeLowerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeLower.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeLowerProperty =
            DependencyProperty.Register("RangeLower", typeof(int), typeof(RatingRangeControl), new FrameworkPropertyMetadata() {
                DefaultValue = 1,
                BindsTwoWayByDefault = true
            });


        public int RangeUpper
        {
            get { return (int)GetValue(RangeUpperProperty); }
            set { SetValue(RangeUpperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeUpper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeUpperProperty =
            DependencyProperty.Register("RangeUpper", typeof(int), typeof(RatingRangeControl), new FrameworkPropertyMetadata()
            {
                DefaultValue = 1,
                BindsTwoWayByDefault = true
            });    

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Enabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(RatingRangeControl), new PropertyMetadata(true));
    }
}
