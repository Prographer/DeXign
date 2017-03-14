using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DeXign.Core.Controls;
using DeXign.Core.Collections;

namespace DeXign.Core.Logic
{
    /// <summary>
    /// BaseBinder 사이의 논리적 연결을 지원합니다.
    /// </summary>
    public class BaseBinder : PObject, IBinder
    {
        /// <summary>
        /// Input, Output, Parameter중 한곳에 연결될 경우 Binded 이벤트가 발생합니다.
        /// </summary>
        public event EventHandler<BinderBindedEventArgs> Binded;

        /// <summary>
        /// Input, Output, Parameter중 한곳에서 연결이 끊어진경우 Released 이벤트가 발생합니다.
        /// </summary>
        public event EventHandler<BinderReleasedEventArgs> Released;

        // Lazy Instance Create

        // Binder
        private BinderCollection _inputs;

        /// <summary>
        /// 들어오는 연결입니다.
        /// </summary>
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

        /// <summary>
        /// 나가는 연결입니다.
        /// </summary>
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

        /// <summary>
        /// 들어오는 파라미터 연결입니다.
        /// </summary>
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
        
        /// <summary>
        /// 바인더를 생성합니다.
        /// </summary>
        public BaseBinder()
        {
        }

        /// <summary>
        /// <paramref name="outputBinder"/>와 연결합니다.
        /// </summary>
        /// <param name="outputBinder"></param>
        /// <param name="options"></param>
        public void Bind(BaseBinder outputBinder, BinderOptions options)
        {
            BinderOperation.SetBind(outputBinder, this, options);

            Binded?.Invoke(this, new BinderBindedEventArgs(outputBinder, options));
        }

        /// <summary>
        /// <paramref name="outputBinder"/>의 연결을 수립할 수 있는지의 여부를 확인합니다.
        /// </summary>
        /// <param name="outputBinder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual bool CanBind(BaseBinder outputBinder, BinderOptions options)
        {
            return !GetInputBinderCollection(this, options).Contains(outputBinder);
        }

        /// <summary>
        /// Input, Output, Parameter에 연결되있는 모든 연결을 해제합니다.
        /// </summary>
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

        /// <summary>
        /// Input에 연결되있는 <paramref name="outputBinder"/>를 해제합니다.
        /// </summary>
        /// <param name="outputBinder"></param>
        public void ReleaseInput(BaseBinder outputBinder)
        {
            // 연결된 바인더 정보 가져옴
            var expression = this.InputExpressions.GetExpression(outputBinder, this);

            if (expression == null)
                expression = this.ParameterExpressions.GetExpression(outputBinder, this);

            if (expression == null)
                return;

            // 바인더 정보에 따라 컬렉션 가져옴
            var inputExpressionCollection = GetInputExpressionCollection(this, expression.BindOptions);
            var inputCollection = GetInputBinderCollection(this, expression.BindOptions);

            inputCollection.Remove(outputBinder);
            inputExpressionCollection.Remove(expression);

            outputBinder.ReleaseOutput(this);

            Released?.Invoke(this, new BinderReleasedEventArgs(expression));
        }

        /// <summary>
        /// Output에 연결되있는 <paramref name="inputBinder"/>를 해제합니다.
        /// </summary>
        /// <param name="inputBinder"></param>
        public void ReleaseOutput(BaseBinder inputBinder)
        {
            var expression = this.OutputExpressions.GetExpression(this, inputBinder);

            this.Outputs.Remove(inputBinder);
            this.OutputExpressions.Remove(expression);

            inputBinder.ReleaseInput(this);

            Released?.Invoke(this, new BinderReleasedEventArgs(expression));
        }

        /// <summary>
        /// Input 연결의 표현 목록을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BinderExpression> GetInputExpression()
        {
            return InputExpressions;
        }

        /// <summary>
        /// Output 연결의 표현 목록을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BinderExpression> GetOutputExpression()
        {
            return OutputExpressions;
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
