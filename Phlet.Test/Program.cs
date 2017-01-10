using System.Windows;

using Phlet.Core;
using Phlet.Core.Controls;

namespace Phlet.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var content = new PContentPage
            {
                Title = "Phlet",
                Content = new PStackLayout
                {
                    VerticalAlignment = LayoutAlignment.Center,
                    HorizontalAlignment = LayoutAlignment.End,
                    Children =
                    {
                        new PLabel
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Phlet!"
                        }
                    }
                }
            };
        }
    }
}