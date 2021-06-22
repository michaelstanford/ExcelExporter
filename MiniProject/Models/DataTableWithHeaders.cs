using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace MiniProject.Models
{
    [ActivatorUtilitiesConstructor]
    public class DataTableWithHeaders
    {
        private DataTable _dataTable;

        public DataTableWithHeaders(string fileName)
        {
            _dataTable = FromFile(fileName);
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(_dataTable);
        }

        public string AsXml()
        {
            _dataTable.TableName = "Results";
            StringWriter stringWriter = new StringWriter();
            _dataTable.WriteXml(stringWriter, XmlWriteMode.IgnoreSchema);
            string xmlString = stringWriter.ToString();
            return xmlString;
        }


        public string AsDelimited(string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames =
                _dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(delimiter, columnNames));

            foreach (DataRow row in _dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(delimiter, fields));
            }

            var stringVal = sb.ToString();
            return stringVal;
        }

        public void SortTable(string sortedBy)
        {
            _dataTable.DefaultView.Sort = sortedBy;
            _dataTable = _dataTable.DefaultView.ToTable();
        }

        private static DataTable FromFile(string fileName)
        {
            using (var excelPack = new ExcelPackage(new FileInfo(fileName)))
            {
                var ws = excelPack.Workbook.Worksheets[0];

                DataTable dataTable = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    if (!string.IsNullOrEmpty(firstRowCell.Text))
                    {
                        dataTable.Columns.Add(firstRowCell.Text);
                    }
                }
                var startRow = 2;

                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, dataTable.Columns.Count];
                    DataRow row = dataTable.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                return dataTable;

            }
        }
    }
}
