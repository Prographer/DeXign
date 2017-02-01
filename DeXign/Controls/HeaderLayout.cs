﻿using DeXign.Extension;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Controls
{
    public class HeaderLayout : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyHelper.Register();

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}