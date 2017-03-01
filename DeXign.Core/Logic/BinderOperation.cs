namespace DeXign.Core.Logic
{
    public static class BinderOperation
    {
        public static void SetBind(IBinderProvider outputProvider, IBinderProvider inputProvider, BinderOptions options)
        {
            BaseBinder outputBinder = outputProvider.ProvideValue() as BaseBinder;
            BaseBinder inputBinder = inputProvider.ProvideValue() as BaseBinder;

            SetBind(outputBinder, inputBinder, options);
        }

        public static void SetBind(BaseBinder output, BaseBinder input, BinderOptions options)
        {
            if (!input.CanBind(output, options))
                throw new System.Exception("바인드할 수 없습니다");

            var expression = new BinderExpression(input, output, options);

            // Output -> Input
            output.OutputExpressions.Add(expression);
            output.Outputs.Add(input);

            var inputCollection = BaseBinder.GetInputBinderCollection(input, options);
            var inputExpressionCollection = BaseBinder.GetInputExpressionCollection(input, options);

            // Input -> Output
            inputCollection.Add(output);
            inputExpressionCollection.Add(expression);
        }
    }
}
