using DeXign.UI.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.UI
{
    public enum MessageLength
    {
        Short = 2000,
        Long = 3500
    }

    public enum MessageDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class MessagePanel : ContentControl
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyHelper.Register(
                new PropertyMetadata(MessageDirection.Bottom));

        public MessageDirection Direction
        {
            get { return (MessageDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        TranslateTransform transform;
        DateTime pendingHideTime;

        Border textContainer;
        FrameworkElement contentElement;

        public MessagePanel()
        {
            this.ClipToBounds = true;

            transform = new TranslateTransform();

            textContainer = new Border()
            {
                Background = "#222222".ToBrush(),
                Child = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Foreground = Brushes.White,
                    Margin = new Thickness(10, 0, 0, 0)
                }
            };

            this.Loaded += MessagePanel_Loaded;
        }

        private void MessagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MessagePanel_Loaded;

            this.SetOffset(this.GetBeginOffset());
        }

        public void Show()
        {
            int duration = 300;

            this.Content = contentElement;

            pendingHideTime = DateTime.MaxValue;

            Animator.BeginDoubleAnimation(
                transform,
                GetDirectionProperty(), 0,
                duration,
                EasingFactory.CircleOut);
        }


        public void Show(MessageLength length, string text)
        {
            (textContainer.Child as TextBlock).Text = text;

            this.Content = textContainer;

            this.Show(length);
        }

        public void Show(MessageLength length)
        {
            int messageLength = (int)length;
            int duration = 300;

            this.Content = contentElement;

            pendingHideTime = DateTime.Now.AddMilliseconds(messageLength - 50);

            Animator.BeginDoubleAnimation(
                transform,
                GetDirectionProperty(), 0,
                duration,
                EasingFactory.CircleOut,
                async (s, e) =>
                {
                    await Task.Delay(messageLength - duration);
                    
                    if (pendingHideTime <= DateTime.Now)
                        this.Hide();
                });
        }

        public void Hide()
        {
            Animator.BeginDoubleAnimation(
                transform,
                GetDirectionProperty(), GetBeginOffset(),
                easing: EasingFactory.CircleOut);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            
            if (oldContent is FrameworkElement oldElement)
                oldElement.RenderTransform = null;

            if (newContent is FrameworkElement newElement)
            {
                contentElement = newElement;
                newElement.RenderTransform = transform;
            }
        }

        private void SetOffset(double offset)
        {
            DependencyProperty property = GetDirectionProperty();

            Animator.StopAnimation(transform, TranslateTransform.XProperty);
            Animator.StopAnimation(transform, TranslateTransform.YProperty);
            
            transform.SetValue(property, offset);
        }

        private DependencyProperty GetDirectionProperty()
        {
            if (this.Direction == MessageDirection.Left || this.Direction == MessageDirection.Right)
            {
                return TranslateTransform.XProperty;
            }
            else
            {
                return TranslateTransform.YProperty;
            }
        }

        private double GetBeginOffset()
        {
            switch (this.Direction)
            {
                case MessageDirection.Left:
                    return -this.RenderSize.Width;

                case MessageDirection.Right:
                    return this.RenderSize.Width;

                case MessageDirection.Top:
                    return -this.RenderSize.Height;

                case MessageDirection.Bottom:
                    return this.RenderSize.Height;
            }

            return -1;
        }
    }
}
