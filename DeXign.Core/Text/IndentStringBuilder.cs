using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DeXign.Core.Text
{
    public class IndentStringBuilder
    {
        public int Depth { get; }

        private StringBuilder builder;

        #region [ Constructor ]
        public IndentStringBuilder()
        {
            builder = new StringBuilder();
        }

        public IndentStringBuilder(int defaultDepth) : this()
        {
            this.Depth = defaultDepth;
        }

        public IndentStringBuilder(string value, int defaultDepth)
        {
            this.Depth = defaultDepth;
            builder = new StringBuilder(value);
        }

        public IndentStringBuilder(string value) : this(value, 0)
        {
        }
        #endregion

        #region [ Methods ]
        public IndentStringBuilder AppendIndent(int depth = 1)
        {
            depth += this.Depth;

            if (depth > 0)
                builder.Append(CreateIndent(depth));

            return this;
        }

        public IndentStringBuilder Append(string value, int depth = 0)
        {
            this.AppendIndent(depth);
            builder.Append(value);

            return this;
        }

        public IndentStringBuilder AppendLine(int depth = 0)
        {
            this.AppendIndent(depth);
            builder.AppendLine();

            return this;
        }

        public IndentStringBuilder AppendLine(string value, int depth = 0)
        {
            this.AppendIndent(depth);
            builder.AppendLine(value);

            return this;
        }

        public IndentStringBuilder AppendBlock(string value, int depth = 0)
        {
            string[] lines = Regex.Split(value.Trim('\r', '\n'), "\r\n");

            for (int i = 0; i < lines.Length; i++)
            {
                this.Append(lines[i], depth);

                if (i < lines.Length - 1)
                    this.AppendLine();
            }

            return this;
        }

        public IndentStringBuilder Replace(string oldValue, string newValue)
        {
            builder.Replace(oldValue, newValue);

            return this;
        }
        
        public IndentStringBuilder Insert(int index, string value)
        {
            builder.Insert(index, value);

            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public bool Contains(string value)
        {
            return this.IndexOf(value) != -1;
        }

        /// <summary>
        /// KMP 알고리즘을 사용하여 문자열의 인덱스를 가져옵니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(string value)
        {
            if (value == null)
                throw new ArgumentNullException();

            if (value.Length == 0)
                return 0;

            if (value.Length == 1)
            {
                char c = value[0];

                for (int idx = 0; idx != builder.Length; ++idx)
                    if (builder[idx] == c)
                        return idx;

                return -1;
            }

            int m = 0;
            int i = 0;
            int[] T = CreateKMPTable(value);

            while (m + i < builder.Length)
            {
                if (value[i] == builder[m + i])
                {
                    if (i == value.Length - 1)
                        return m == value.Length ? -1 : m;

                    ++i;
                }
                else
                {
                    m = m + i - T[i];
                    i = T[i] > -1 ? T[i] : 0;
                }
            }

            return -1;
        }

        private int[] CreateKMPTable(string sought)
        {
            var table = new int[sought.Length];

            int position = 2;
            int cnd = 0;

            table[0] = -1;
            table[1] = 0;

            while (position < table.Length)
            {
                if (sought[position - 1] == sought[cnd])
                {
                    table[position++] = ++cnd;
                }
                else if (cnd > 0)
                {
                    cnd = table[cnd];
                }
                else
                {
                    table[position++] = 0;
                }
            }

            return table;
        }
        #endregion

        #region [ Inner ]
        private string CreateIndent(int depth)
        {
            return new string(' ', depth * 4);
        }
        #endregion
    }
}
