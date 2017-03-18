using System.Collections.Generic;
using System.Linq;

namespace DeXign.Core.Logic
{
    public static class BinderHelper
    {
        public static void GetPairObject<T>(
            ref T output, ref T input,
            (T Object, BindOptions Option) source,
            (T Object, BindOptions Option) target)
        {
            if (BindDirection.Output.HasFlag((BindDirection)source.Option))
            {
                output = source.Object;
                input = target.Object;
            }
            else
            {
                output = target.Object;
                input = source.Object;
            }
        }

        public static (IBinder Output, IBinder Input) GetPairBinder(this IBinder sourceBinder, IBinder targetBinder)
        {
            if (BindDirection.Output.HasFlag((BindDirection)sourceBinder.BindOption))
                return (sourceBinder, targetBinder);

            return (targetBinder, sourceBinder);
        }

        public static BindOptions GetPairOption(this IBinder binder)
        {
            return binder.BindOption.GetPairOption();
        }

        public static BindOptions GetPairOption(this BindOptions option)
        {
            switch (option)
            {
                case BindOptions.Output:
                    return BindOptions.Input;

                case BindOptions.Input:
                    return BindOptions.Output;

                case BindOptions.Parameter:
                    return BindOptions.Return;

                case BindOptions.Return:
                    return BindOptions.Parameter;
            }

            return default(BindOptions);
        }

        /// <summary>
        /// 바인더와 연결된 모든 바인더 호스트를 재귀적으로 가져옵니다.
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
