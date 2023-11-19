using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace HelpfulHive
{


    public class XmlProcessor
    {
        private Dictionary<string, string> columnNamesDict;
        private Dictionary<string, string> infoEnumDict;

        public XmlProcessor()
        {
            columnNamesDict = new Dictionary<string, string>
        {
            // Пример инициализации. Замените на реальные данные.
            { "ExampleKey1", "ExampleColumnName1" },
            { "ExampleKey2", "ExampleColumnName2" }
            // Добавьте здесь другие пары ключ-значение
        };

            infoEnumDict = new Dictionary<string, string>
        {
            // Пример инициализации. Замените на реальные данные.
            { "EnumKey1", "EnumValue1" },
            { "EnumKey2", "EnumValue2" }
            // Добавьте здесь другие пары ключ-значение
        };
        }

        public DataTable ConvertXmlToDataTable(string xmlContent)
        {
            try
            {
                XDocument xDocument = XDocument.Parse(xmlContent);
                XElement requestData = xDocument.Descendants().FirstOrDefault(x => x.Name.LocalName == "RequestData");

                if (requestData == null)
                {
                    throw new InvalidOperationException("XML не содержит элемента RequestData");
                }

                // Использование FillDataGrid вместо ConvertListToDataTable
                return FillDataGrid(requestData);
            }
            catch (Exception ex) {
                return null ;
            }
            // Парсинг XML
        
        }

        private List<Dictionary<string, string>> GetRequestData(XElement requestDataElement)
        {
            List<Dictionary<string, string>> requestData = new List<Dictionary<string, string>>();

            foreach (XElement batch in requestDataElement.Elements())
            {
                Dictionary<string, string> batchData = new Dictionary<string, string>();
                FillBatchData(batchData, batch, "");
                requestData.Add(batchData);
            }

            return requestData;
        }

      



        private DataTable FillDataGrid(XElement requestData)
        {
            DataTable dataTable = new DataTable();
            var batches = requestData.Elements();

            HashSet<string> columnsToAdd = new HashSet<string>();
            foreach (var batch in batches)
            {
                Dictionary<string, string> batchData = new Dictionary<string, string>();
                FillBatchData(batchData, batch, "");
                foreach (var key in batchData.Keys)
                {
                    columnsToAdd.Add(key);
                }
            }

            var sortedColumns = columnsToAdd.OrderBy(col => !(col.ToLower().Contains("weight") || col.ToLower().Contains("quantity"))).ToList();
            foreach (var key in sortedColumns)
            {
                string columnName = columnNamesDict.ContainsKey(key) ? columnNamesDict[key] : key;
                if (!dataTable.Columns.Contains(columnName))
                {
                    dataTable.Columns.Add(columnName);
                }
            }

            foreach (var batch in batches)
            {
                Dictionary<string, string> batchData = new Dictionary<string, string>();
                FillBatchData(batchData, batch, "");

                DataRow newRow = dataTable.NewRow();
                foreach (var item in batchData)
                {
                    newRow[columnNamesDict.ContainsKey(item.Key) ? columnNamesDict[item.Key] : item.Key] = item.Value;
                }
                dataTable.Rows.Add(newRow);
            }

            DataRow summaryRow = CreateSummaryRow(dataTable);
            dataTable.Rows.InsertAt(summaryRow, 0);

            return dataTable;
        }

        private void FillBatchData(Dictionary<string, string> data, XElement element, string prefix)
        {
            foreach (var child in element.Elements())
            {
                string newPrefix = prefix == "" ? child.Name.LocalName : prefix + "_" + child.Name.LocalName;
                if (child.HasElements)
                {
                    FillBatchData(data, child, newPrefix);
                }
                else
                {
                    data[newPrefix] = child.Value;
                }
            }
        }

        private DataRow CreateSummaryRow(DataTable dataTable)
        {
            DataRow summaryRow = dataTable.NewRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName.ToLower().Contains("weight") || column.ColumnName.ToLower().Contains("quantity"))
                {
                    decimal sum = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (decimal.TryParse(row[column].ToString(), out decimal value))
                        {
                            sum += value;
                        }
                    }
                    summaryRow[column] = sum;
                }
            }
            return summaryRow;
        }





    }

}
