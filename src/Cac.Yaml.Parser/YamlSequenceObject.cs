using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cac.Yaml
{
    public class YamlSequenceObject : YamlObject, ICollection<IYamlObject>
    {
        private readonly List<IYamlObject> _list;

        public YamlSequenceObject() : base(YamlObjectType.Sequence)
        {
            _list = new List<IYamlObject>();
        }

        public IYamlObject Value { get; set; }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public override bool IsEmpty => _list.Count == 0;

        public IYamlObject this[int position]
        {
            get => _list[position];
            set => _list[position] = value;
        }

        public void Add(IYamlObject item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IYamlObject item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IYamlObject[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IYamlObject> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(IYamlObject item)
        {
            return _list.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public override string ToString(int spaces, bool isSequence, bool isMapping)
        {
            spaces = Math.Max(0, spaces - 2);
            var space = new string(' ', spaces);
            var sb = new StringBuilder();
            if (isMapping)
            {
                sb.Append("\n");
            }

            foreach (var item in _list)
            {
                sb.Append(space);
                sb.Append("- ");
                sb.Append(item.ToString(spaces + 2, true, false));
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public override object Clone()
        {
            var result = new YamlSequenceObject
            {
                Line = Line,
                Column = Column
            };

            foreach (var item in _list)
            {
                result.Add((IYamlObject)item.Clone());
            }

            return result;
        }
    }
}
