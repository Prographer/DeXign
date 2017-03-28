using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Controls;
using DeXign.Extension;
using DeXign.Editor.Controls;

using WPFExtension;
using DeXign.Editor.Renderer;
using DeXign.Resources;
using System.Windows.Media;
using DeXign.Editor.Layer;
using DeXign.Task;

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

        public static readonly DependencyProperty AccentBrushProperty =
            DependencyHelper.Register(
                new PropertyMetadata(ResourceManager.GetBrush("Logic.Statement")));

        public static readonly DependencyProperty AccentColorProperty =
            DependencyHelper.Register(
                new PropertyMetadata(ResourceManager.GetColor("Logic.Statement")));

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

        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        public Color AccentColor
        {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
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

        public double Left
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        public double Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }

        public TaskManager TaskManager => this.ParentStoryboard?.TaskManager;

        #region [ Local Variable ]
        internal Storyboard ParentStoryboard { get; private set; }

        private RelativeThumb moveThumb;

        private Point beginPosition;
        #endregion

        public ComponentElement()
        {
            this.Opacity = 0.3;

            this.SetValue(InputThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(OutputThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(ParameterThumbsPropertyKey, new ObservableCollection<BindThumb>());
            this.SetValue(ReturnThumbsPropertyKey, new ObservableCollection<BindThumb>());

            this.Loaded += ComponentElement_Loaded;
            this.Unloaded += ComponentElement_Unloaded;
        }

        private void ComponentElement_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= ComponentElement_Unloaded;

            OnUnloaded();
        }
        
        private void ComponentElement_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ComponentElement_Loaded;

            this.ParentStoryboard = this.FindVisualParents<Storyboard>().FirstOrDefault();

            InitializeMoveThumb();

            OnLoaded();
        }

        protected virtual void OnLoaded()
        {
        }

        protected virtual void OnUnloaded()
        {
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

        public void DragMove()
        {
            this.moveThumb.CaptureMouse();
        }

        protected virtual void OnAttachedComponentModel()
        {
        }

        private void InitializeSelector()
        {
            if (this.IsFloating())
                return;

            this.AddSelectedHandler(Selected);
            this.AddUnselectedHandler(Unselected);
        }

        private void InitializeBinders()
        {
            foreach (IBinder binder in this.Model.Items)
            {
                EnsureAddThumb(binder);
            }

            this.Model.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void InitializeMoveThumb()
        {
            if (moveThumb != null)
            {
                moveThumb.RelativeTarget = this.ParentStoryboard;
                
                moveThumb.DragStarted += MoveThumb_DragStarted;
                moveThumb.DragDelta += MoveThumb_DragDelta;
                moveThumb.DragCompleted += MoveThumb_DragCompleted;
            }
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

            if (this.IsFloating())
                return;

            moveThumb = GetTemplateChild<RelativeThumb>("PART_moveThumb");
        }

        private void Thumb_Binded(object sender, ThumbBindedEventArgs e)
        {

        }
        
        private void Unselected(object sender, SelectionChangedEventArgs e)
        {
            SetNodeOpacity(BindOptions.Output | BindOptions.Return, 0.3);
            SetNodeOpacity(BindOptions.Input | BindOptions.Parameter, 0.3);

            OnUnSelected();
        }

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            if (this.ParentStoryboard != null)
                Keyboard.Focus(this.ParentStoryboard);

            SetNodeOpacity(BindOptions.Output | BindOptions.Return, 1);
            SetNodeOpacity(BindOptions.Input | BindOptions.Parameter, 1);

            OnSelected();
        }
        
        protected virtual void OnSelected()
        {
        }

        protected virtual void OnUnSelected()
        {
        }

        public virtual void OnApplyContentTemplate()
        {
        }

        private void SetNodeOpacity(BindOptions option, double opacity)
        {
            this.Opacity = opacity;

            foreach (var node in BinderHelper.FindHostNodes(this.Model, option))
            {
                var element = node.GetView<FrameworkElement>();

                if (element != null)
                    element.Opacity = opacity;
            }
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

        #region [ Focusing ]
        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDoubleClick(e);

            if (this.ParentStoryboard != null)
            {
                e.Handled = true;
                
                this.ParentStoryboard.ZoomFocusTo(this.GetRenderer());
            }
        }
        #endregion

        #region [ Drag ] 
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double x = beginPosition.X + e.HorizontalChange;
            double y = beginPosition.Y + e.VerticalChange;

            this.Left = SnapToGrid(x);
            this.Top = SnapToGrid(y);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            beginPosition = new Point(this.Left, this.Top);
            
            if (this.Parent is Canvas canvas)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }

            this.SetIsSelected(true);
        }

        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Point movedPosition = new Point(this.Left, this.Top);
            Point prevPosition = beginPosition;

            var layer = this.GetRenderer() as StoryboardLayer;

            this.TaskManager.Push(
                new TaskData(this,
                () =>
                {
                    this.Left = movedPosition.X;
                    this.Top = movedPosition.Y;
                },
                () =>
                {
                    this.Left = prevPosition.X;
                    this.Top = prevPosition.Y;
                }));
        }

        public double SnapToGrid(double value)
        {
            return Math.Floor(value / GridSnap) * GridSnap;
        }
        #endregion

        #region [ Binder Method ]
        protected IEnumerable<BindThumb> GetBindThumbs(BindOptions option)
        {
            switch (option)
            {
                case BindOptions.Input:
                    return InputThumbs;

                case BindOptions.Output:
                    return OutputThumbs;

                case BindOptions.Parameter:
                    return ParameterThumbs;

                case BindOptions.Return:
                    return ReturnThumbs;

                case BindOptions.Output | BindOptions.Return:
                    return OutputThumbs.Concat(ReturnThumbs);

                case BindOptions.Input | BindOptions.Parameter:
                    return InputThumbs.Concat(ParameterThumbs);
            }

            return Enumerable.Empty<BindThumb>();
        }

        protected T GetBinderModel<T>(BindOptions option, int index)
            where T : IBinder
        {
            if (this.Model != null)
            {
                IBinder binder = this.Model[option].Skip(index).FirstOrDefault();

                if (binder is T tBinder)
                    return tBinder;
            }

            return default(T);
        }

        protected BindThumb GetBindThumb(BindOptions option, int index)
        {
            return GetBinderModel<PBinder>(option, index).GetView<BindThumb>();
        }
        #endregion
    }
}