﻿using System.Collections.Generic;

namespace SearchFile.Module.Messaging.FileFilters
{
    public class CsvFileFilter : IFilter
    {
        public string Name { get; } = "CSVファイル";
        public IEnumerable<string> Patterns { get; } = new[] { "*.csv" };
    }
}
