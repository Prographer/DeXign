using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeXign.Extension
{
    public class ObjectNode<TParent, TChild>
    {
        public TParent Parent { get; set; }
        public TChild Child { get; set; }

        public ObjectNode(TParent parent, TChild child)
        {
            this.Parent = parent;
            this.Child = child;
        }
    }

    public class ObjectNode : ObjectNode<object, object>
    {
        public ObjectNode(object parent, object child) : base(parent, child)
        {
        }
    }

    public static class ObjectContentTreeHelper
    {
        public static IEnumerable<object> GetChildren(this object obj)
        {
            object content = obj.GetContent();

            // Object
            if (content is PropertyInfo)
                yield return (content as PropertyInfo).GetValue(obj);

            // List
            if (content is IList)
                foreach (object item in content as IList)
                    yield return item;
        }

        public static IEnumerable<ObjectNode<TParent, TChild>> FindContentChildrens<TParent, TChild>(this object obj, bool findAll = true)
        {
            return Finds<TParent, TChild>(obj, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(object parent, Queue<ObjectNode> visualQueue)
        {
            foreach (object child in ObjectContentTreeHelper.GetChildren(parent))
            {
                if (child == null)
                    continue;

                visualQueue.Enqueue(new ObjectNode(parent, child));
            }
        }

        private static IEnumerable<ObjectNode<TParent, TChild>> Finds<TParent, TChild>(
            this object element,
            Action<object, Queue<ObjectNode>> elementSetter,
            bool findAll = true)
        {
            var objectQueue = new Queue<ObjectNode>();
            objectQueue.Enqueue(new ObjectNode(element, element));

            while (objectQueue.Count > 0)
            {
                var node = objectQueue.Dequeue();
                
                if (node.Parent is TParent &&
                    node.Child is TChild &&
                    !ReferenceEquals(element, node.Child))
                {
                    yield return new ObjectNode<TParent, TChild>((TParent)node.Parent, (TChild)node.Child);

                    if (!findAll)
                        break;
                }

                elementSetter(node.Child, objectQueue);
            }
        }
    }
}
