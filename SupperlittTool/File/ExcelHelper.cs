using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.Streaming;

namespace SupperlittTool
{
    /// <summary>
    /// Excel管理工具
    /// </summary>
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        /// <summary>
        /// 初始化Excel，创建和打开
        /// </summary>
        /// <param name="fileName"></param>
        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 对已有的Excel追加数据，从指定行开始
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="startIndex">指定追加行号</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten, int startIndex)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;
            using (fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
            }

            try
            {
                if (workbook != null)
                {
                    if (workbook.NumberOfSheets > 0)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    else
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }

                    count = 1 + startIndex;
                }
                else
                {
                    count = 0 + startIndex;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        if (data.Rows[i][j] == DBNull.Value || data.Rows[i][j] == null)
                        {
                            row.CreateCell(j).SetCellValue("");
                        }
                        else
                        {
                            if (data.Columns[j].DataType == typeof(double) || data.Columns[j].DataType == typeof(float))
                            {
                                row.CreateCell(j).SetCellValue(Convert.ToDouble(data.Rows[i][j]));
                            }
                            else
                            {
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                            }
                        }
                    }

                    ++count;
                }

                sheet.ForceFormulaRecalculation = true;

                //将内存数据写到文件
                using (fs = new FileStream(fileName, FileMode.Open, FileAccess.Write))
                {
                    workbook.Write(fs);
                    workbook.Close();
                }

                return count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook(fs);
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook(fs);

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }

                //workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            DataColumn column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                            data.Columns.Add(column);
                        }

                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }

                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 大文件导出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        /// <param name="isColumnWritten"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int DataTableToExcel_SXSS(DataTable data, string sheetName, bool isColumnWritten, int startIndex)
        {
            //创建SXSSFWorkbook对象
            SXSSFWorkbook wb = new SXSSFWorkbook();

            //创建sheet　并指定sheet的名字
            ISheet sheet1 = wb.CreateSheet(sheetName);

            if (startIndex == 0)
            {
                //创建第一行
                IRow row0 = sheet1.CreateRow(startIndex);
                for (int index = 0; index < data.Columns.Count; index++)
                {
                    var dc = data.Columns[index];
                    row0.CreateCell(index).SetCellValue(dc.ColumnName);
                }

                startIndex++;
            }

            // 构建大数据量
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow dr = data.Rows[i];
                IRow rowi = sheet1.CreateRow(startIndex);
                for (int index = 0; index < data.Columns.Count; index++)
                {
                    var dc = data.Columns[index];

                    if (dr[index] == null || dr[index] == DBNull.Value)
                    {
                        rowi.CreateCell(index).SetCellValue("");
                    }
                    else
                    {
                        if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                        {
                            rowi.CreateCell(index).SetCellValue(Convert.ToDouble(dr[index]));
                        }
                        else
                        {
                            rowi.CreateCell(index).SetCellValue(dr[index].ToString());
                        }
                    }
                }

                startIndex++;
            }

            try
            {
                //将内存中的数据写入磁盘
                using (FileStream filestream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    wb.Write(filestream);
                    filestream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return startIndex;
        }

        /// <summary>
        /// 大文件导出,多sheet
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="sheetName"></param>
        /// <param name="isColumnWritten"></param>
        public void DataTableToExcel_SXSS(List<DataTable> dataList, string sheetName, bool isColumnWritten)
        {
            //创建SXSSFWorkbook对象
            SXSSFWorkbook wb = new SXSSFWorkbook();

            // 构建大数据量
            for (int k = 0; k < dataList.Count; k++)
            {
                var data = dataList[k];

                // 创建sheet　并指定sheet的名字
                ISheet sheet1 = wb.CreateSheet(sheetName + k);

                //创建第一行
                IRow row0 = sheet1.CreateRow(0);
                for (int index = 0; index < data.Columns.Count; index++)
                {
                    var dc = dataList[0].Columns[index];
                    row0.CreateCell(index).SetCellValue(dc.ColumnName);
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (i % 1000 == 0)
                    {
                        Console.Write(k + "-" + i + " ");
                    }

                    DataRow dr = data.Rows[i];
                    IRow rowi = sheet1.CreateRow(i + 1);
                    for (int index = 0; index < data.Columns.Count; index++)
                    {
                        var dc = data.Columns[index];

                        if (dr[index] == null || dr[index] == DBNull.Value)
                        {
                            rowi.CreateCell(index).SetCellValue("");
                        }
                        else
                        {
                            if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                            {
                                rowi.CreateCell(index).SetCellValue(Convert.ToDouble(dr[index]));
                            }
                            else
                            {
                                rowi.CreateCell(index).SetCellValue(dr[index].ToString());
                            }
                        }
                    }
                }
            }

            try
            {
                //将内存中的数据写入磁盘
                using (FileStream filestream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    wb.Write(filestream);
                    filestream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
    }
}
