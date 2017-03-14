using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DeXign.Extension;
using DeXign.Core.Logic;

using WPFExtension;
using System;
using DeXign.Controls;
using DeXign.Editor.Renderer;
using DeXign.Editor.Layer;
using DeXign.OS;
using System.Windows.Controls.Primitives;

namespace DeXign.Editor.Logic
{
    public class BindExpression
    {
        public BindThumb Output { get; set; }
        public BindThumb Input { get; set; }

        public BindExpression(BindThumb output, BindThumb input)
        {
            this.Output = output;
            this.Input = input;
        }
    }

    [TemplatePart(Name = "PART_input", Type = typeof(BindThumb))]
    [TemplatePart(Name = "PART_output", Type = typeof(BindThumb))]
    [TemplatePart(Name = "PART_moveThumb", Type = typeof(RelativeThumb))]
    public class ComponentElement : ContentControl
    {
        const double GridSnap = 16;

        public event EventHandler<BindExpression> Binded;

        public static readonly DependencyProperty HeaderProperty =
            DependencyHelper.Register();

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public PComponent Model => (PComponent)this.DataContext;

        #region [ Local Variable ]
        private BindThumb inputThumb;
        private BindThumb outputThumb;
        private RelativeThumb moveThumb;

        private Point beginPosition;
        #endregion

        public void SetComponentModel(PComponent model)
        {
            InitializeSelector();
            this.Content = model;
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);
        }
        
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            OnApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            inputThumb = GetTemplateChild<BindThumb>("PART_input");
            outputThumb = GetTemplateChild<BindThumb>("PART_output");
            moveThumb = GetTemplateChild<RelativeThumb>("PART_moveThumb");

            if (inputThumb != null)
                inputThumb.Binded += InputThumb_Binded;

            if (outputThumb != null)
                outputThumb.Binded += OutputThumb_Binded;

            if (moveThumb != null)
                Model.GetRenderer().ElementAttached += ComponentElement_ElementAttached;
        }

        private void ComponentElement_ElementAttached(object sender, EventArgs e)
        {
            Model.GetRenderer().ElementAttached -= ComponentElement_ElementAttached;

            if (moveThumb != null)
            {
                moveThumb.RelativeTarget = (this.GetRenderer() as StoryboardLayer).RootParent;

                moveThumb.DragStarted += MoveThumb_DragStarted;
                moveThumb.DragDelta += MoveThumb_DragDelta;
            }
        }

        private void OutputThumb_Binded(object sender, BindEventArgs e)
        {
            Binded?.Invoke(this, new BindExpression(outputThumb, e.Target));
        }

        private void InputThumb_Binded(object sender, BindEventArgs e)
        {
            Binded?.Invoke(this, new BindExpression(e.Target, inputThumb));
        }

        private void OnUnselected(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            
        }

        #region [ Drag ] 
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double x = beginPosition.X + e.HorizontalChange;
            double y = beginPosition.Y + e.VerticalChange;

            x = SnapToGrid(x);
            y = SnapToGrid(y);
            
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            beginPosition = new Point(
                Canvas.GetLeft(this),
                Canvas.GetTop(this));
            
            if (this.Parent is Canvas canvas)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }

            this.SetIsSelected(true);
        }

        public double SnapToGrid(double value)
        {
            return Math.Floor(value / GridSnap) * GridSnap;
        }
        #endregion

        public virtual void OnApplyContentTemplate()
        {
        }

        internal BindThumb GetThumb(BindType type)
        {
            if (type == BindType.Input)
                return inputThumb;

            return outputThumb;
        }

        #region [ Template Method ]
        protected internal T GetTemplateChild<T>(string name)
            where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }

        protected internal T GetContentTemplateChild<T>(string name)
            where T : FrameworkElement
        {
            return this.FindVisualChildrens<T>()
                .Where(tb => tb.Name == name)
                .FirstOrDefault();
        }
        #endregion
    }
}