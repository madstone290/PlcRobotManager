using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace PlcRobotManager.Util.ExcelUtils
{
    public class ExcelService : IExcelService
    {
        public List<T> Read<T>(string filePath, ExcelOptions options = null)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();

                    // The result of each spreadsheet is in result.Tables
                    if(result.Tables.Count == 0)
                        return Enumerable.Empty<T>().ToList();

                    return DataTableToList<T>(result.Tables[0]);
                }
            }
        }

        private List<T> DataTableToList<T>(DataTable dataTable, ExcelOptions options = null)
        {
            var list = new List<T>();

            if (dataTable.Rows.Count == 0)
                return list;
            dataTable.CaseSensitive = false;

            var properties = typeof(T).GetProperties().Where(x => x.CanWrite);
            IEnumerable<ExcelOptions.Header> headers = options?.Headers
              ?? properties.Select(prop => new ExcelOptions.Header(prop.Name, prop.Name)).ToList();

            // Key: 헤더, Value: DataColumn Index
            Dictionary<ExcelOptions.Header, int> columnIndexes = new Dictionary<ExcelOptions.Header, int>();

            var dataRows = dataTable.Rows.Cast<DataRow>();
            var dataColumns = dataTable.Columns.Cast<DataColumn>();

            DataRow headerRow = dataRows.First();
            foreach (DataColumn column in dataColumns)
            {
                var caption = headerRow[column].ToString();
                var header = headers.FirstOrDefault(x=> x.Caption == caption);
                if (header != null)
                    columnIndexes[header] = column.Ordinal;
            }

            foreach (DataRow row in dataRows.Skip(1))
            {
                var instance = (T)Activator.CreateInstance(typeof(T));
                foreach (var header in headers)
                {
                    // 캡션으로 컬럼 접근
                    if (!columnIndexes.TryGetValue(header, out int columnIndex))
                        continue;

                    var property = properties.FirstOrDefault(x => string.Equals(x.Name, header.Name, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        try
                        {
                            var properValue = Convert.ChangeType(row[columnIndex], property.PropertyType);
                            property.SetValue(instance, properValue);
                        }
                        catch { } // Failed to change value type 
                    }
                }

                list.Add(instance);
            }
            return list;
        }


    }
}
