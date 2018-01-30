using System;
using System.Collections.Generic;

namespace Fireasy.Zero.Infrastructure
{
    public class DataRepeatException : Exception
    {
        public DataRepeatException(string title, List<int> rows)
    : base(string.Empty)
        {
            Title = title;
            Rows = rows;
        }

        public string Title { get; set; }

        public List<int> Rows { get; set; }
    }
}
