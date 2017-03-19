﻿using System;
using System.Linq;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PTargetable : PComponent
    {
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(PGetter));

        public static readonly DependencyProperty TargetTypeProperty =
            DependencyHelper.Register();

        public DependencyProperty Property
        {
            get { return GetValue<DependencyProperty>(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        [ComponentParameter("대상", typeof(PObject), DisplayIndex = 0, IsSingle = true)]
        public Type TargetType
        {
            get { return GetValue<Type>(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        public PParameterBinder TargetBinder { get; }

        public PTargetable() : base()
        {
            this.AddNewBinder(BindOptions.Input);
            this.AddNewBinder(BindOptions.Output);

            this.AddReturnBinder("값", typeof(object));

            this.TargetBinder = this[BindOptions.Parameter].First() as PParameterBinder;
        }
    }
}
