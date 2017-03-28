using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;

namespace DeXign.UI
{
    public class DeXignWindow : Window
    {
        Dictionary<string, FrameworkElement> elementBackpack;
        Dictionary<DeXignPage, List<string>> pageChildNames;

        Grid pageContainer;
        ContentPresenter pagePresenter;

        public DeXignPage SelectedPage => (DeXignPage)pagePresenter.Content;

        public DeXignWindow()
        {
            InitializeComponent();

            this.Loaded += DeXignWindow_Loaded;
        }
        
        private void InitializeComponent()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            pageContainer = new Grid();
            pagePresenter = new ContentPresenter();

            pageContainer.Children.Add(pagePresenter);

            this.Content = pageContainer;

            elementBackpack = new Dictionary<string, FrameworkElement>();
            pageChildNames = new Dictionary<DeXignPage, List<string>>();
        }

        private void DeXignWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DeXignWindow_Loaded;
            
            OnLoaded();
        }

        protected virtual void OnLoaded()
        {
        }

        public void Add(DeXignPage page)
        {
            if (!pageChildNames.ContainsKey(page))
                pageChildNames[page] = new List<string>();

            // Clear
            pageChildNames[page].Clear();
            elementBackpack[GetCleanName(page.Name)] = page;

            foreach (FrameworkElement element in page.FindVisualChildrens<FrameworkElement>())
            {
                string name = element.Name;
                
                if (name.StartsWith("__") && name.EndsWith("__"))
                {
                    name = GetCleanName(name);

                    if (elementBackpack.ContainsKey(name))
                        throw new ElementNameDuplicateException();

                    // Add On Global Backpack
                    elementBackpack[name] = element;

                    // Add On Page Child Names
                    if (page.Name != element.Name)
                        pageChildNames[page].Add(name);
                }                
            }
        }

        private string GetCleanName(string name)
        {
            if (name.StartsWith("__") && name.EndsWith("__"))
            {
                return name.Substring(2, name.Length - 4);
            }

            return name;
        }

        public void Remove(DeXignPage page)
        {
            // 페이지 제거 (Global)
            if (elementBackpack.ContainsKey(page.Name))
                elementBackpack.Remove(page.Name);

            if (pageChildNames.ContainsKey(page))
            {
                // 페이지 자식 이름 제거
                foreach (string childName in pageChildNames[page])
                    if (elementBackpack.ContainsKey(childName))
                        elementBackpack.Remove(childName);

                // 페이지 제거
                pageChildNames.Remove(page);
            }
        }

        public void SetPage(DeXignPage page)
        {
            if (elementBackpack.ContainsValue(page))
            {
                if (SelectedPage == null)
                {
                    // Initialize
                    this.Width = page.Width + SystemParameters.ResizeFrameVerticalBorderWidth * 2;
                    this.Height = page.Height + SystemParameters.ResizeFrameHorizontalBorderHeight * 2 + SystemParameters.CaptionHeight;
                    
                    page.Width = double.NaN;
                    page.Height = double.NaN;
                }

                pagePresenter.Content = page;
                
                this.Title = page.WindowTitle ?? "";
            }
            else
            {
                throw new UnknownPageException();
            }
        }

        public void SetPage(string pageName)
        {
            var page = FindElement<DeXignPage>(pageName);

            SetPage(page ?? throw new PageNotFoundException());
        }

        public new object FindName(string name)
        {
            if (elementBackpack.ContainsKey(name))
                return elementBackpack[name];

            throw new ElementNotFoundException();
        }

        public T FindElement<T>(string name)
            where T : FrameworkElement
        {
            if (elementBackpack.ContainsKey(name))
                return (T)elementBackpack[name];

            throw new ElementNotFoundException();
        }
    }
}
