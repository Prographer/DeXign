using DeXign.Core.Collections;
using DeXign.Core.Controls;
using System;
using System.Linq;

namespace DeXign.Core.Logic
{
    public class BaseBinder : PObject, IBinder
    {
        public event EventHandler<BinderBindedEventArgs> Binded;
        public event EventHandler<BinderReleasedEventArgs> Released;

        // Lazy Instance Create

        // Binder
        private BinderCollection _inputs;
        public BinderCollection Inputs
        {
            get
            {
                if (_inputs == null)
                    _inputs = new BinderCollection();

                return _inputs;
            }
        }

        private BinderCollection _outputs;
        public BinderCollection Outputs
        {
            get
            {
                if (_outputs == null)
                    _outputs = new BinderCollection();

                return _outputs;
            }
        }

        private BinderCollection _parameters;
        public BinderCollection Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new BinderCollection();

                return _parameters;
            }
        }

        // Expression
        private BinderExpressionCollection _inputExpressions;
        internal BinderExpressionCollection InputExpressions
        {
            get
            {
                if (_inputExpressions == null)
                    _inputExpressions = new BinderExpressionCollection();

                return _inputExpressions;
            }
        }

        private BinderExpressionCollection _outputExpressions;
        internal BinderExpressionCollection OutputExpressions
        {
            get
            {
                if (_outputExpressions == null)
                    _outputExpressions = new BinderExpressionCollection();

                return _outputExpressions;
            }
        }

        private BinderExpressionCollection _parameterExpressions;
        internal BinderExpressionCollection ParameterExpressions
        {
            get
            {
                if (_parameterExpressions == null)
                    _parameterExpressions = new BinderExpressionCollection();

                return _parameterExpressions;
            }
        }
        
        public BaseBinder()
        {
        }

        public void Bind(BaseBinder outputBinder, BinderOptions options)
        {
            BinderOperation.SetBind(outputBinder, this, options);
        }

        public virtual bool CanBind(BaseBinder outputBinder, BinderOptions options)
        {
            return !GetInputBinderCollection(this, options).Contains(outputBinder);
        }

        public void ReleaseAll()
        {
            // Input
            foreach (BinderExpression expression in InputExpressions.ToArray())
                expression.Release();

            // Output
            foreach (BinderExpression expression in OutputExpressions.ToArray())
                expression.Release();

            // Parameter
            foreach (BinderExpression expression in ParameterExpressions.ToArray())
                expression.Release();
        }

        public void ReleaseInput(BaseBinder outputBinder)
        {
            // 연결된 바인더 정보 가져옴
            var expression = this.InputExpressions.GetExpression(outputBinder, this);

            if (expression == null)
                expression = this.ParameterExpressions.GetExpression(outputBinder, this);

            if (expression == null)
                throw new ArgumentException();

            // 바인더 정보에 따라 컬렉션 가져옴
            var inputExpressionCollection = GetInputExpressionCollection(this, expression.BindOptions);
            var inputCollection = GetInputBinderCollection(this, expression.BindOptions);

            inputCollection.Remove(outputBinder);
            inputExpressionCollection.Remove(expression);

            outputBinder.ReleaseOutput(this);
        }

        public void ReleaseOutput(BaseBinder inputBinder)
        {
            var expression = this.OutputExpressions.GetExpression(this, inputBinder);

            this.Outputs.Remove(inputBinder);
            this.OutputExpressions.Remove(expression);
        }
        
        internal static BinderCollection GetInputBinderCollection(BaseBinder binder, BinderOptions option)
        {
            if (option == BinderOptions.Trigger)
                return binder.Inputs;

            return binder.Parameters;
        }

        internal static BinderExpressionCollection GetInputExpressionCollection(BaseBinder binder, BinderOptions option)
        {
            if (option == BinderOptions.Trigger)
                return binder.InputExpressions;

            return binder.ParameterExpressions;
        }
        
        BaseBinder IBinderProvider.ProvideValue()
        {
            return this;
        }
    }
}
