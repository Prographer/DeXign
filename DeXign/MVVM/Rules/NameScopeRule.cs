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
            bool result = false;
            string name = (string)value;
            var expression = owner as BindingExpression;
            
            if (expression.ResolvedSource is PObject)
            {
                var obj = expression.ResolvedSource as PObject;
                var renderer = obj.GetRenderer();

                if (renderer != null)
                {
                    var scope = LogicalTreeHelperEx.FindLogicalParents<INameScope>(renderer.Element).FirstOrDefault();

                    if (string.IsNullOrEmpty(name))
                    {
                        scope.Unregister(obj);
                    }
                    else if (StringRule.CheckNamingRule(name ?? "", true))
                    {
                        if (scope.HasName(name))
                        {
                            MessageBox.Show($"'{name}'는 이미 정의된 이름입니다.", "DeXign");
                            expression.UpdateTarget();
                        }
                        else
                        {
                            scope.Register(obj, name);
                            result = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"네임 스코프를 찾을 수 없습니다.", "DeXign");
                }
            }

            return base.Validate(result, cultureInfo, owner);
        }
    }
}
