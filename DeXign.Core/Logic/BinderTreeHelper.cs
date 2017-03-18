using System.Collections.Generic;
using System.Linq;

namespace DeXign.Core.Logic
{
    public static class BinderTreeHelper
    {
        /// <summary>
        /// 바인더와 연결된 모든 바인더를 재귀적으로 가져옵니다.
        /// </summary>
        /// <param name="sourceBinder"></param>
        /// <returns></returns>
        public static IEnumerable<PBinderHost> FindHostNodes(
            this IBinderHost source, 
            BindOptions option = BindOptions.Input | BindOptions.Output | BindOptions.Parameter | BindOptions.Return,
            int depth = -1)
        {
            var visited = new List<IBinderHost>(
                new[]
                {
                    source
                });

            var queue = new Queue<(IBinderHost Host, int Level)>(
                new[]
                {
                    (source, 0)
                });

            while (queue.Count > 0)
            {
                var data = queue.Dequeue();

                if (data.Level > depth && depth != -1)
                    continue;

                int nextLevel = (depth == -1 ? -1 : data.Level + 1);

                if (data.Host != null)
                {
                    foreach (PBinderHost host in data.Host[option]
                        .SelectMany(sourceBinder => sourceBinder.Items)
                        .Select(targetBinder => targetBinder.Host)
                        .Distinct()
                        .Except(visited))
                    {
                        queue.Enqueue((host, nextLevel));
                        visited.Add(host);

                        yield return host;
                    }
                    
                }
            }
        }
    }
}
