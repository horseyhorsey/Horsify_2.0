using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class SmartButton : Button
    {
        #region Fields
        bool uphandled = false;
        bool downhandled = false;
        #endregion

        #region Commands
        public ICommand ClickAndHoldCommand
        {
            get { return (ICommand)GetValue(ClickAndHoldCommandProperty); }
            set { SetValue(ClickAndHoldCommandProperty, value); }
        }
        #endregion

        #region Properties
        private DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        public int MillisecondsToWait
        {
            get { return (int)GetValue(MillisecondsToWaitProperty); }
            set { SetValue(MillisecondsToWaitProperty, value); }
        }

        public object ClickAndHoldCommandParameter
        {
            get { return (object)GetValue(ClickAndHoldCommandParameterProperty); }
            set { SetValue(ClickAndHoldCommandParameterProperty, value); }
        }

        public bool EnableClickHold
        {
            get { return (bool)GetValue(EnableClickHoldProperty); }
            set { SetValue(EnableClickHoldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableClickHold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableClickHoldProperty =
            DependencyProperty.Register("EnableClickHold", typeof(bool), typeof(SmartButton), new PropertyMetadata(false));


        // Using a DependencyProperty as the backing store for ClickAndHoldCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickAndHoldCommandParameterProperty =
            DependencyProperty.Register("ClickAndHoldCommandParameter", typeof(object), typeof(SmartButton), new PropertyMetadata(""));

        // Using a DependencyProperty as the backing store for ClickAndHoldCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickAndHoldCommandProperty =
            DependencyProperty.Register("ClickAndHoldCommand", typeof(ICommand), typeof(SmartButton), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for MillisecondsToWait.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MillisecondsToWaitProperty =
            DependencyProperty.Register("MillisecondsToWait", typeof(int), typeof(SmartButton), new PropertyMetadata(0));
        #endregion

        #region Constructors
        public SmartButton()
        {
            //this.PreviewMouseRightButtonDown += SmartButton_PreviewMouseRightButtonDown;
            ////Disable the mouse if touch screen...fires twice otherwise...TODO
            
            if (Properties.Settings.Default.IsTouchScreen)
            {
                this.PreviewTouchDown += SmartButton_PreviewTouchDown;
                this.PreviewTouchUp += SmartButton_PreviewTouchUp;
            }
            else
            {
                this.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                this.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            }

        }

        //private void SmartButton_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        #endregion

        #region Support Methods
        /// <summary>
        /// Touchscreen Up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartButton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            OnSmartButtonUp(e);
        }

        /// <summary>
        /// Touch screen Down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartButton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            OnSmartButtonDown(e);
        }

        /// <summary>
        /// Creates a timer when clicked if click and hold is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnSmartButtonDown(e);
        }

        /// <summary>
        ///  Only acts if the button is held down for a second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnSmartButtonUp(e);
        }

        /// <summary>
        /// Deals with Mouse or touch screen events for when User pushes the button
        /// </summary>
        /// <param name="e"></param>
        private void OnSmartButtonDown(InputEventArgs e)
        {
            if (EnableClickHold)
            {
                if (e.GetType() == typeof(TouchEventArgs))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }

                //Create a timer
                Timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher)
                {
                    Interval = TimeSpan.FromMilliseconds(MillisecondsToWait)
                };

                Timer.Tick += Timer_Tick;
                Timer.IsEnabled = true;
                Timer.Start();
            }
        }

        /// <summary>
        /// Deals with Mouse or touch screen events for when User releases button
        /// </summary>
        /// <param name="e"></param>
        private void OnSmartButtonUp(InputEventArgs e)
        {
            if (EnableClickHold)
            {
                if (Timer != null)
                    try
                    {
                        if (e.GetType() == typeof(TouchEventArgs))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }

                        //Enable the timer
                        bool isMouseReleaseBeforeHoldTimeout = Timer.IsEnabled;

                        ResetAndRemoveTimer();

                        // Consider it as a mouse click 
                        if (isMouseReleaseBeforeHoldTimeout)
                        {
                            //Command.Execute(CommandParameter);                            
                        }

                        //if (e.GetType() == typeof(TouchEventArgs))
                        //{
                        //    //e.Handled = true;
                        //}
                        //else                        

                    }
                    catch { }
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.ClickAndHoldCommandParameter != null && this.ClickAndHoldCommandParameter.ToString().Contains("Window"))
            {
                this.ClickAndHoldCommand.Execute(this.ClickAndHoldCommandParameter);
            }
            else if (this.ClickAndHoldCommandParameter.ToString().Contains("Window"))
            {
                this.ClickAndHoldCommand.Execute(this.ClickAndHoldCommandParameter);
            }

            ResetAndRemoveTimer();
        }

        private void ResetAndRemoveTimer()
        {
            if (Timer == null) return;
            Timer.Tick -= Timer_Tick;
            Timer.IsEnabled = false;
            Timer.Stop();
            Timer = null;
        }
        #endregion
    }
}
