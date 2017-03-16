using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;


using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Controls;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Editor.Logic
{
    public class BindedEventArgs : EventArgs
    {
        public BindThumb Output { get; set; }
        public BindThumb Input { get; set; }
        
        public BinderExpression BinderExpression { get; set; }

        public BindedEventArgs(BindThumb output, BindThumb input)
        {
            this.Output = output;
            this.Input = input;
        }
    }

    [TemplatePart(Name = "PART_moveThumb", Type = typeof(RelativeThumb))]
    public class ComponentElement : ContentControl
    {
        const double GridSnap = 16;

        public event EventHandler<BindedEventArgs> Binded;

        public static readonly DependencyProperty HeaderProperty =
            DependencyHelper.Register(
                new PropertyMetadata("< Component >"));

        private static readonly DependencyPropertyKey InputThumbsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey OutputThumbsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey ParameterThumbsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey ReturnThumbsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty InputThumbsProperty =
            InputThumbsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty OutputThumbsProperty =
            OutputThumbsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ParameterThumbsProperty =
            ParameterThumbsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ReturnThumbsProperty =
            ReturnThumbsPropertyKey.DependencyProperty;

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ObservableCollection<BindThumb> InputThumbs
        {
            get { return (ObservableCollection<BindThumb>)GetValue(InputThumbsProperty); }
        }

        public ObservableCollection<BindThumb> OutputThumbs
        {
            get { return (ObservableCollection<BindThumb>)GetValue(OutputThumbsProperty); }
        }

        public ObservableCollection<BindThumb> ParameterThumbs
        {
            get { return (ObservableCollection<BindThumb>)GetValue(ParameterThumbsProperty); }
        }

        public ObservableCollection<BindThumb> ReturnThumbs
        {
            get { return (ObservableCollection<BindThumb>)GetValue(ReturnThumbsProperty); }
        }

        public PComponent Model => (PComponent)this.DataContext;

        #region [ Local Variable ]
        private RelativeThumb moveThumb;

        private Point beginPosition;
        #endregion

        public ComponentElement()
        {
            this.SetValue(InputThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(OutputThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(ParameterThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(ReturnThumbsPropertyKey, new ObservableCollection<BindThumb>());
        }

        public void SetComponentModel(PComponent model)
        {
            this.Content = model;
            this.DataContext = model;

            if (this.Model.HasAttribute<DesignElementAttribute>())
                this.Header = this.Model.GetAttribute<DesignElementAttribute>().DisplayName;

            InitializeSelector();
            InitializeBinders();

            OnAttachedComponentModel();
        }

        protected virtual void OnAttachedComponentModel()
        {
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);
        }

        private void InitializeBinders()
        {
            foreach (IBinder binder in this.Model.Items)
            {
                EnsureAddThumb(binder);
            }

            this.Model.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void EnsureAddThumb(IBinder binder)
        {
            switch (binder.BindOption)
            {
                case BindOptions.Input:
                case BindOptions.Output:
                    AddTriggerThumb(binder as PBinder);
                    break;

                case BindOptions.Parameter:
                    AddParameterThumb(binder as PParameterBinder);
                    break;

                case BindOptions.Return:
                    AddReturnThumb(binder as PReturnBinder);
                    break;
            }
        }

        private void EnsureRemoveThumb(IBinder binder)
        {
            if (binder is PBinder binderModel)
            {
                var thumb = binderModel.GetView<BindThumb>();

                if (thumb != null)
                {
                    switch (binder.BindOption)
                    {
                        case BindOptions.Input:
                            InputThumbs.Remove(thumb);
                            break;

                        case BindOptions.Output:
                            OutputThumbs.Remove(thumb);
                            break;

                        case BindOptions.Parameter:
                            ParameterThumbs.Remove(thumb);
                            break;

                        case BindOptions.Return:
                            ReturnThumbs.Remove(thumb);
                            break;
                    }
                }
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PBinder binder in e.OldItems)
                {
                    EnsureRemoveThumb(binder);
                }
            }

            if (e.NewItems != null)
            {
                foreach (IBinder binder in e.NewItems)
                {
                    EnsureAddThumb(binder);
                }
            }
        }

        // Trigger In / Out Thumb
        private void AddTriggerThumb(PBinder binder)
        {
            var thumb = new BindThumb(binder);

            if (binder.BindOption == BindOptions.Input)
                InputThumbs.Add(thumb);
            else
                OutputThumbs.Add(thumb);

            AddThumbCore(thumb);
        }

        // Parameter Thumb
        private void AddParameterThumb(PParameterBinder parameterBinder)
        {
            BindThumb thumb;

            ParameterThumbs.Add(
                thumb = new BindThumb(parameterBinder));

            AddThumbCore(thumb);
        }

        // Return Thumb
        private void AddReturnThumb(PReturnBinder returnBinder)
        {
            BindThumb thumb;

            ReturnThumbs.Add(
                thumb = new BindThumb(returnBinder));

            AddThumbCore(thumb);
        }

        private void AddThumbCore(BindThumb thumb)
        {
            thumb.Binder.SetView(thumb);

            thumb.Binded += Thumb_Binded;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            OnApplyContentTemplate();
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            moveThumb = GetTemplateChild<RelativeThumb>("PART_moveThumb");

            if (this.IsLoaded)
                InitializeMoveThumb();
            else
                this.Loaded += ComponentElement_Loaded;
        }

        private void ComponentElement_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ComponentElement_Loaded;

            InitializeMoveThumb();
        }

        private void InitializeMoveThumb()
        {
            if (moveThumb != null)
            {
                moveThumb.RelativeTarget = (this.GetRenderer() as StoryboardLayer).Storyboard;

                moveThumb.DragStarted += MoveThumb_DragStarted;
                moveThumb.DragDelta += MoveThumb_DragDelta;
            }
        }

        private void Thumb_Binded(object sender, ThumbBindedEventArgs e)
        {

        }
        
        private void OnUnselected(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            
        }


        public virtual void OnApplyContentTemplate()
        {
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
    }
}