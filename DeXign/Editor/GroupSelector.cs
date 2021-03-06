using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFExtension;

namespace DeXign.Editor
{
    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        public SelectionChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }
    }

    public static class GroupSelector
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyHelper.RegisterAttached<bool>();
        #endregion

        #region [ Global Event ]
        public static event EventHandler SelectedItemChanged;
        #endregion

        #region [ Routed Event ]
        public delegate void SelectorEventHandler(object sender, SelectionChangedEventArgs e);

        public static readonly RoutedEvent SelectedEvent =
            EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Direct, typeof(SelectorEventHandler), typeof(GroupSelector));

        public static readonly RoutedEvent UnselectedEvent =
            EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Direct, typeof(SelectorEventHandler), typeof(GroupSelector));
        #endregion

        static Dictionary<string, List<FrameworkElement>> groups =
            new Dictionary<string, List<FrameworkElement>>();
        
        public static void Select(FrameworkElement obj, bool select, string group = "default", bool multiSelect = false)
        {
            if (!groups.ContainsKey(group))
                groups[group] = new List<FrameworkElement>();

            obj.SetValue(IsSelectedProperty, select);

            if (!select)
            {
                if (!groups[group].Contains(obj))
                    return;

                groups[group].Remove(obj);

                obj.RaiseEvent(new SelectionChangedEventArgs(UnselectedEvent));
            }
            else
            {
                if (groups[group].Contains(obj))
                    return;

                if (!multiSelect)
                    UnselectAll(group);

                groups[group].Add(obj);

                obj.RaiseEvent(new SelectionChangedEventArgs(SelectedEvent));
            }

            SelectedItemChanged?.Invoke(null, null);
        }

        public static void UnselectAll(string group = "default", params FrameworkElement[] ignorElements)
        {
            if (!groups.ContainsKey(group) || groups[group].Count == 0)
                return;

            foreach (var item in GetSelectedItems(group).Except(ignorElements).ToArray())
            {
                item.SetValue(IsSelectedProperty, false);

                groups[group].Remove(item);
                item.RaiseEvent(new SelectionChangedEventArgs(UnselectedEvent));
            }

            SelectedItemChanged?.Invoke(null, null);
        }

        public static bool IsSelected(FrameworkElement obj, string group = "default")
        {
            if (groups.ContainsKey(group))
                return groups[group].Contains(obj);

            return false;
        }

        public static int GetSelectedItemCount(string group = "default")
        {
            return GetSelectedItems(group).Count();
        }

        public static IEnumerable<FrameworkElement> GetSelectedItems(string group = "default")
        {
            if (groups.ContainsKey("default"))
                return groups["default"];

            return Enumerable.Empty<FrameworkElement>();
        }

        #region [ Routed Event Extension ]
        public static void AddSelectedHandler(this UIElement element, SelectorEventHandler handler)
        {
            element.AddHandler(SelectedEvent, handler);
        }

        public static void AddUnselectedHandler(this UIElement element, SelectorEventHandler handler)
        {
            element.AddHandler(UnselectedEvent, handler);
        }

        public static void RemoveSelectedHandler(this UIElement element, SelectorEventHandler handler)
        {
            element.RemoveHandler(SelectedEvent, handler);
        }

        public static void RemoveUnselectedHandler(this UIElement element, SelectorEventHandler handler)
        {
            element.RemoveHandler(UnselectedEvent, handler);
        }
        #endregion

        #region [ Dependency Extension ]
        public static bool GetIsSelected(this FrameworkElement obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(this FrameworkElement obj, bool value)
        {
            if (value)
                Select(obj, value, multiSelect: Keyboard.IsKeyDown(Key.LeftShift));

            obj.SetValue(IsSelectedProperty, value);
        }
        #endregion
    }
}