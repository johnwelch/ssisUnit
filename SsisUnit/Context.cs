using System;
using System.Collections;
using System.Collections.Generic;
#if !SQL2005
using System.Linq;
#endif

namespace SsisUnit
{
    public class Context
    {
        internal Context()
        {
            Log = new TreeItem<Log>(new Log() { ItemName = "TestSuite" });
        }

        internal Context(SsisTestSuite testSuite)
            : this()
        {
            TestSuite = testSuite;
        }

        internal Context(SsisTestSuite testSuite, Test test)
            : this(testSuite)
        {
            Test = test;
        }

        public SsisTestSuite TestSuite { get; private set; }
        public Test Test { get; set; }

        public TreeItem<Log> Log { get; set; }
    }

    public class TreeItem<T> : IEnumerable<TreeItem<T>>
    {
        private List<TreeItem<T>> _children;

        public TreeItem<T> Parent { get; protected set; }

        public T Value { get; set; }

        public TreeItem(T value)
        {
            Value = value;
        }

        public TreeItem<T> this[int index]
        {
            get
            {
                if (_children != null)
                {
                    return _children[index];
                }

                throw new ArgumentException("Collection has no values.", "index");
            }
        }

        public void ApplyTo(Action<T> action)
        {
            action(Value);
            if (_children == null)
            {
                return;
            }

            foreach (var child in _children)
            {
                child.ApplyTo(action);
            }
        }

        public TreeItem<T> Find(T value)
        {
            if (Value.Equals(value))
            {
                return this;
            }

            if (_children == null)
            {
                return null;
            }

            foreach (var child in _children)
            {
                var foundValue = child.Find(value);
                if (foundValue != null)
                {
                    return foundValue;
                }
            }

            return null;
        }

        public IEnumerator<TreeItem<T>> GetEnumerator()
        {
#if SQL2005
            return _children != null
                ? _children.GetEnumerator()
                : new List<TreeItem<T>>.Enumerator();
#else
            return _children != null
                ? _children.GetEnumerator()
                : Enumerable.Empty<TreeItem<T>>().GetEnumerator();
#endif

        }

        public virtual TreeItem<T> Add(T item)
        {
            if (_children == null)
            {
                _children = new List<TreeItem<T>>();
            }

            var newNode = new TreeItem<T>(item) { Parent = this };
            _children.Add(newNode);
            return newNode;
        }

        public void Clear()
        {
            if (_children != null)
            {
                _children.Clear();
            }
        }

        public bool Contains(TreeItem<T> item)
        {
            return _children != null && _children.Contains(item);
        }

        public void CopyTo(TreeItem<T>[] array, int arrayIndex)
        {
            if (_children != null)
            {
                _children.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(TreeItem<T> item)
        {
            return _children != null && _children.Remove(item);
        }

        public int Count
        {
            get
            {
                return _children != null ? _children.Count : 0;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Log
    {
        public Log()
        {
            Messages = new List<string>();
        }

        public string ItemName { get; set; }

        public IList<string> Messages { get; private set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var rhs = (Log)obj;
            return ItemName.Equals(rhs.ItemName, StringComparison.OrdinalIgnoreCase);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return ItemName.GetHashCode();
        }

        public static bool operator ==(Log lhs, Log rhs)
        {
            if (lhs == null || rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Log lhs, Log rhs)
        {
            if (lhs == null || rhs == null)
            {
                return false;
            }

            return !(lhs == rhs);
        }
    }
}
