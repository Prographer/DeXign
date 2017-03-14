using DeXign.Core.Logic;
using System.Collections.Generic;

namespace DeXign.Editor.Logic
{
    public static class BinderTreeHelper
    {
        /// <summary>
        /// 바인더와 연결된 모든 바인더 표현을 재귀적으로 가져옵니다.
        /// </summary>
        /// <param name="sourceBinder"></param>
        /// <returns></returns>
        public static IEnumerable<(BaseBinder Output, BaseBinder Input)> FindAllNodeExpressions(this BaseBinder sourceBinder, int depth = -1)
        {
            var queue = new Queue<(BaseBinder Binder, int Level)>(
                new[]
                {
                    (sourceBinder, 0)
                });

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.Level > depth && depth != -1)
                    continue;

                int childLevel = (depth == -1 ? -1 : node.Level + 1);

                if (node.Binder != null)
                {
                    // input
                    foreach (BaseBinder input in node.Binder.Inputs)
                    {
                        queue.Enqueue((input, childLevel));

                        yield return (input, node.Binder);
                    }

                    // parameter
                    foreach (BaseBinder param in node.Binder.Parameters)
                    {
                        queue.Enqueue((param, childLevel));

                        yield return (param, node.Binder);
                    }

                    // outputs
                    foreach (BaseBinder output in node.Binder.Outputs)
                    {
                        queue.Enqueue((output, childLevel));

                        yield return (node.Binder, output);
                    }
                }
            }
        }

        /// <summary>
        /// 바인더와 연결된 모든 바인더를 재귀적으로 가져옵니다.
        /// </summary>
        /// <param name="sourceBinder"></param>
        /// <returns></returns>
        public static IEnumerable<BaseBinder> FindAllNodes(this BaseBinder sourceBinder, int depth = -1)
        {
            var queue = new Queue<(BaseBinder Binder, int Level)>(
                new[] 
                {
                    (sourceBinder, 0)
                });

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.Level > depth && depth != -1)
                    continue;

                int childLevel = (depth == -1 ? -1 : node.Level + 1);

                if (node.Binder != null)
                {
                    // input
                    foreach (BaseBinder input in node.Binder.Inputs)
                    {
                        queue.Enqueue((input, childLevel));

                        yield return input;
                    }

                    // parameter
                    foreach (BaseBinder param in node.Binder.Parameters)
                    {
                        queue.Enqueue((param, childLevel));

                        yield return param;
                    }

                    // outputs
                    foreach (BaseBinder output in node.Binder.Outputs)
                    {
                        queue.Enqueue((output, childLevel));

                        yield return output;
                    }
                }
            }
        }
    }
}
