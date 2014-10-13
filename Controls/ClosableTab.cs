using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CarDepot.Controls
{
    public class ClosableTab:TabItem
    {
        private CloseableHeaderControl closableTabHeaderControl;
        public delegate bool TabClosingEventHandler();
        public event TabClosingEventHandler TabClosing;

        public ClosableTab()
        {
            // Create an instance of the usercontrol
            closableTabHeaderControl = new CloseableHeaderControl();
            // Assign the usercontrol to the tab header
            this.Header = closableTabHeaderControl;

            closableTabHeaderControl.button_close.MouseEnter += new MouseEventHandler(button_close_MouseEnter);
            closableTabHeaderControl.button_close.MouseLeave += new MouseEventHandler(button_close_MouseLeave);
            closableTabHeaderControl.button_close.Click += new RoutedEventHandler(button_close_Click);
            closableTabHeaderControl.label_TabTitle.SizeChanged += new SizeChangedEventHandler(label_TabTitle_SizeChanged);
        }

        public string Title
        {
            set
            {
                ((CloseableHeaderControl)this.Header).label_TabTitle.Content = value;
            }
        }

        public Brush BackGroundColor
        {
            set
            {
                closableTabHeaderControl.Background = value;
            }
            get { return closableTabHeaderControl.Background; }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeaderControl)this.Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeaderControl)this.Header).button_close.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeaderControl)this.Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((CloseableHeaderControl)this.Header).button_close.Visibility = Visibility.Hidden;
            }
        }

        // Button MouseEnter - When the mouse is over the button - change color to Red
        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeaderControl)this.Header).button_close.Foreground = Brushes.Red;
        }
        // Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeaderControl)this.Header).button_close.Foreground = Brushes.Black;
        }
        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        public void button_close_Click(object sender, RoutedEventArgs e)
        {
            bool cancel = false;
            if (TabClosing != null)
            {
                cancel = TabClosing();
            }

            if (cancel)
            {
                return;
            }

            if (this.Parent != null && ((TabControl)this.Parent).Items.Contains(this))
            {
                ((TabControl)this.Parent).Items.Remove(this);
            }
        }
        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeaderControl)this.Header).button_close.Margin = new Thickness(
               ((CloseableHeaderControl)this.Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
    }
}
