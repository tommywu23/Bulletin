using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ClientContract.Helper;

namespace ClientContract.View {
    public class Charm : ContentControl {
        public event EventHandler CollapsedCharm;
        public event EventHandler ExpandCharm;

        public Charm() {
            this.RenderTransform = translate;
            this.BuildStory();
            this.Opacity = 1;
            // this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            // this.PreviewMouseDown += OnPreviewMouseDown;
            this.MouseDown += OnMouseDown;

            autohideTimer.Interval = TimeSpan.FromSeconds(10);
            autohideTimer.Tick += (s1, e1) => {
                try {
                    Collapsed();
                    autohideTimer.Stop();
                } catch (Exception ex) {
                    Logger.Warn(this, ex.Message);
                }
            };

            if (!VSDesigner.IsDesignMode) {
                autohideTimer.Start();
            }
        }

        void OnMouseDown(object sender, MouseButtonEventArgs e) {
            CancelAutoHide();

            this.CaptureMouse();
            begin = e.GetPosition(null);
            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            //if (Mouse.LeftButton == MouseButtonState.Pressed) {
            //	if (!this.IsMouseCaptured) {
            //		this.CaptureMouse();
            //		begin = e.GetPosition(null);
            //	}
            //}
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e) {
            if (this.IsMouseCaptured) {
                Vector v = e.GetPosition(null) - begin;
                if (v.Y < -SystemParameters.MinimumVerticalDragDistance) {
                    // 向上滑动
                    if (this.Opacity < .5) {
                        Expand();
                    }
                } else if (v.Y > SystemParameters.MinimumVerticalDragDistance) {
                    // 向下滑动
                    if (this.Opacity > .5) {
                        Collapsed();
                    }
                }
                this.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        public void Expand() {
            if (this.Content is UIElement) {
                (this.Content as UIElement).IsEnabled = true;
            }

            if (ExpandCharm != null)
                ExpandCharm(this, EventArgs.Empty);

            expand.Begin();
        }

        public void Collapsed() {
            if (this.Content is UIElement) {
                (this.Content as UIElement).IsEnabled = false;
            }

            collapsedA2.To = this.ActualHeight - 50;
            collapsed.Begin();

        }

        private void BuildStory() {
            expand = new Storyboard();

            var expandA1 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)) { DecelerationRatio = .3 };
            Storyboard.SetTarget(expandA1, this);
            Storyboard.SetTargetProperty(expandA1, new PropertyPath("Opacity"));
            expand.Children.Add(expandA1);

            var expandA2 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(expandA2, this);
            Storyboard.SetTargetProperty(expandA2, new PropertyPath("RenderTransform.Y"));
            expand.Children.Add(expandA2);

            collapsed = new Storyboard();

            var collapsedA1 = new DoubleAnimation(.01, TimeSpan.FromMilliseconds(300)) { DecelerationRatio = .3 };
            Storyboard.SetTarget(collapsedA1, this);
            Storyboard.SetTargetProperty(collapsedA1, new PropertyPath("Opacity"));
            collapsed.Children.Add(collapsedA1);

            collapsedA2 = new DoubleAnimation(50, TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(collapsedA2, this);
            Storyboard.SetTargetProperty(collapsed, new PropertyPath("RenderTransform.Y"));
            collapsed.Children.Add(collapsedA2);
            collapsed.Completed += (o1, e1) => {
                if (CollapsedCharm != null)
                    CollapsedCharm(o1, EventArgs.Empty);
            };
        }

        private void CancelAutoHide() {
            autohideTimer.Stop();
        }

        public void StartAutoHide() {
            autohideTimer.Start();
        }

        public void StopAutoHide() {
            autohideTimer.Stop();
        }

        static Charm() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Charm), new FrameworkPropertyMetadata(typeof(Charm)));
        }

        private Storyboard expand;
        private Storyboard collapsed;
        private Point begin;
        private TranslateTransform translate = new TranslateTransform();
        private DispatcherTimer autohideTimer = new DispatcherTimer();
        private DoubleAnimation collapsedA2;
    }
}
