using DeXign.Core;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DeXign.Rules
{
    public class NameScopeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult((bool)value, "");
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            string name = (string)value;
            var expression = owner as BindingExpression;
            
            if (expression.ResolvedSource is PObject pObj)
            {
                var renderer = pObj.GetRenderer();

                if (renderer != null)
                {
                    var scope = LogicalTreeHelperEx.FindLogicalParents<INameScope>(renderer.Element).FirstOrDefault();

                    if (pObj.Equals(scope.GetOwner(name)))
                    {
                        return base.Validate(true, cultureInfo, owner);
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        scope.Unregister(pObj);

                        return base.Validate(true, cultureInfo, owner);
                    }

                    if (StringRule.IsValidName(name ?? "", true))
                    {
                        if (scope.HasName(name))
                        {
                            MessageBox.Show($"'{name}'는 이미 정의된 이름입니다.", "DeXign", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            expression.UpdateTarget();
                        }
                        else
                        {
                            scope.Register(pObj, name);

                            return base.Validate(true, cultureInfo, owner);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"네임 스코프를 찾을 수 없습니다.", "DeXign");
                }
            }

            return base.Validate(false, cultureInfo, owner);
        }
    }
}
