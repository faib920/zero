using Fireasy.Common.ComponentModel;
using Fireasy.Common.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// Excel辅助类（使用NPOI组件）。
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 导出Excel。
        /// </summary>
        /// <param name="templateFileName">模版文件。</param>
        /// <param name="data">列表数据</param>
        /// <param name="extendKeyValues">扩展绑定的数据。</param>
        /// <param name="staticInfo">静态信息</param>
        /// <param name="password">保护的密码。</param>
        public static byte[] Export(string templateFileName, IList data, object staticInfo, Dictionary<string, string> extendKeyValues = null, string password = null, Action<ISheet, ExportingDocument> initializer = null)
        {
            int count = 0;
            if (data != null)
            {
                count = data.Count;
            }

            var fileName = templateFileName.Substring(templateFileName.LastIndexOf("\\") + 1);

            if (templateFileName.IndexOf(":\\") == -1)
            {
                templateFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateFileName);
            }

            if (!File.Exists(templateFileName))
            {
                throw new Exception("没有找到对应的 Excel 模板文件。");
            }

            using (var stream = new FileStream(templateFileName, FileMode.Open, FileAccess.Read))
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);
                sheet.ForceFormulaRecalculation = true;
                
                // 加密文档
                if (!string.IsNullOrEmpty(password))
                {
                    sheet.ProtectSheet(password);
                }

                var document = ParseDocument(sheet);

                var startRowIndex = document.DataSets[0].StartRowIndex;
                var source = sheet.GetRow(startRowIndex);

                if (document.Extend != null && extendKeyValues != null)
                {
                    var r = document.Extend.RowIndex;
                    var c = document.Extend.ColumnIndex;
                    var index = 0;

                    foreach (var kvp in extendKeyValues)
                    {
                        var cell = document.Extend.Row.GetCell(c + index) ??
                            document.Extend.Row.CreateCell(c + index);

                        cell.SetCellValue(kvp.Value);
                        cell.CellStyle = document.Extend.CellStyle;

                        cell = source.GetCell(c + index) ??
                            source.CreateCell(c + index);

                        cell.CellStyle = source.GetCell(c).CellStyle;

                        index++;

                        document.DataSets[0].Properties.Add(cell, new CellDataMap(kvp.Key, $"[{kvp.Key}]"));
                    }
                }

                TypeDescriptorUtility.AddDefaultDynamicProvider();

                initializer?.Invoke(sheet, document);

                if (count == 0)
                {
                    //删除定义字段标签的行
                    sheet.RemoveRow(source);

                    //上移一行
                    //sheet.ShiftRows(startRowIndex + 1, sheet.LastRowNum, -1, true, true);
                }
                else if (data != null)
                {
                    //sheet.ShiftRows(startRowIndex + 1, sheet.LastRowNum, count);
                    FillSheet(sheet, data, document, source);
                }

                if (staticInfo != null)
                {
                    foreach (var formula in document.Formulas)
                    {
                        var array = new ArrayList();

                        if (formula.Parameters != null && formula.Parameters.Count > 0)
                        {
                            var properties = TypeDescriptor.GetProperties(staticInfo);
                            foreach (var par in formula.Parameters)
                            {
                                var property = properties[par];
                                if (property != null)
                                {
                                    var value = property.GetValue(staticInfo);
                                    array.Add(value);
                                }
                                else
                                {
                                    array.Add(string.Empty);
                                }
                            }
                        }

                        if (array.Count != 0)
                        {
                            SetCellString(formula.Cell, string.Format(formula.Formula, array.ToArray()));
                        }
                    }
                }

                using (var mstram = new MemoryStream())
                {
                    workbook.Write(mstram);
                    return mstram.ToArray();
                }
            }
        }

        /// <summary>
        /// 将数据填充到工作表中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="data">列表数据。</param>
        /// <param name="document">解析的文档对象。</param>
        /// <param name="source">源行，即定义字段标签的行。</param>
        private static void FillSheet(ISheet sheet, IList data, ExportingDocument document, IRow source)
        {
            var index = 0;
            var mergedCellIndex = -1;

            foreach (var item in data)
            {
                var row = index == 0 ? sheet.GetRow(source.RowNum) : sheet.CopyRow(source.RowNum, source.RowNum + index);
                foreach (var ckv in document.DataSets[0].Properties)
                {
                    ICell cell = null;
                    if ((cell = row.GetCell(ckv.Key.ColumnIndex)) == null)
                    {
                        cell = row.CreateCell(ckv.Key.ColumnIndex);
                    }

                    if (index != 0)
                    {
                        var scell = source.GetCell(ckv.Key.ColumnIndex);
                        row.Height = source.Height;
                        cell.CellStyle = scell.CellStyle;
                    }
                    else
                    {
                        if (cell.IsMergedCell)
                        {
                            mergedCellIndex = ckv.Key.ColumnIndex;
                        }
                    }

                    var properties = TypeDescriptor.GetProperties(item);
                    var property = properties[ckv.Value.Key];
                    if (property == null)
                    {
                        continue;
                    }

                    var value = property == null ?
                        string.Empty : property.GetValue(item);

                    SetCellValue(cell, ckv.Value, value);
                }
                index++;
            }
        }

        /// <summary>
        /// 设置单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <param name="map">单元格映射数据。</param>
        /// <param name="value">设置的值。</param>
        private static void SetCellValue(ICell cell, CellDataMap map, object value)
        {
            if (value is int || value is short || value is double || value is decimal)
            {
                if (!TryFormatCellValue(cell, map, Convert.ToDouble(value)))
                {
                    cell.SetCellValue(Convert.ToDouble(value));
                }
            }
            else if (value is DateTime)
            {
                var date = Convert.ToDateTime(value);
                if (date.Year <= 1)
                {
                    cell.SetCellValue(string.Empty);
                }
                else if (!TryFormatCellValue(cell, map, value))
                {
                    cell.SetCellValue(date);
                }
            }
            else
            {
                if (value == null)
                {
                    cell.SetCellValue(string.Empty);
                }
                else if (!TryFormatCellValue(cell, map, value))
                {
                    SetCellString(cell, value.ToString());
                }
            }
        }

        /// <summary>
        /// 设置单元格的字符串。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <param name="str">字符串。</param>
        private static void SetCellString(ICell cell, string str)
        {
            //判断是否有换行
            if (!string.IsNullOrEmpty(str) && str.IndexOf('\n') != -1)
            {
                var rich = new HSSFRichTextString(str);
                cell.CellStyle.WrapText = true;
                cell.SetCellValue(rich);
            }
            else
            {
                cell.SetCellValue(str);
            }
        }

        /// <summary>
        /// 尝试通过格式化后设置单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <param name="map">单元格映射数据。</param>
        /// <param name="value">设置的值。</param>
        /// <returns></returns>
        private static bool TryFormatCellValue(ICell cell, CellDataMap item, object value)
        {
            if (!string.IsNullOrEmpty(item.Format))
            {
                cell.SetCellValue(item.Format.Replace("[" + item.Key + "]", value.ToStringSafely()));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 解析文档
        /// </summary>
        /// <param name="sheet">导出文档</param>
        /// <returns></returns>
        private static ExportingDocument ParseDocument(ISheet sheet)
        {
            var document = new ExportingDocument();

            for (var r = 0; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);
                if (row == null)
                {
                    break;
                }

                SheetDataSet set = null;

                for (var c = 0; c < row.LastCellNum; c++)
                {
                    var cell = row.GetCell(c);
                    if (cell == null)
                    {
                        break;
                    }

                    if (cell.CellType == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue))
                    {
                        var text = cell.StringCellValue;
                        var si = text.IndexOf('[');
                        var ei = text.IndexOf(']', si + 1);
                        if (si != -1 && ei != -1)
                        {
                            set = ParseDataSet(cell, set, text.Substring(si, ei - si + 1), text, r);
                        }
                        else if (text.IndexOf("`") != -1)
                        {
                            document.Formulas.Add(ParseFormula(cell, text));
                        }
                        else if (text == "{Extend}")
                        {
                            document.Extend = cell;
                        }
                    }
                }

                if (set != null)
                {
                    document.DataSets.Add(set);
                }
            }

            return document;
        }

        /// <summary>
        /// 解析数据集
        /// </summary>
        /// <param name="cell">Cell</param>
        /// <param name="set">Set</param>
        /// <param name="key">Text</param>
        /// <param name="format">格式。</param>
        /// <param name="r">起始行索引</param>
        /// <returns></returns>
        private static SheetDataSet ParseDataSet(ICell cell, SheetDataSet set, string key, string format, int r)
        {
            if (set == null)
            {
                set = new SheetDataSet();
                set.StartRowIndex = r;
            }

            if (format == key)
            {
                format = string.Empty;
            }

            var ditem = new CellDataMap(key.Substring(1, key.Length - 2), format);
            set.Properties.Add(cell, ditem);

            return set;
        }

        /// <summary>
        /// 解析公式
        /// </summary>
        /// <param name="cell">Cell</param>
        /// <param name="text">Text</param>
        /// <returns></returns>
        private static SheetFormula ParseFormula(ICell cell, string text)
        {
            var formula = new SheetFormula { Cell = cell };
            var b = false;
            var start = 0;
            var sb = new StringBuilder();

            for (var i = 0; i < text.Length; i++)
            {

                if (text[i] == '`')
                {
                    if (!b)
                    {
                        start = i;
                        b = true;
                    }
                    else
                    {
                        sb.Append("{" + formula.Parameters.Count + "}");

                        formula.Parameters.Add(text.Substring(start + 1, i - start - 1));
                        b = false;
                    }
                }
                else if (!b)
                {
                    sb.Append(text[i]);
                }
            }

            formula.Formula = sb.ToString();

            return formula;
        }
    }

    /// <summary>
    /// 导出文档实体
    /// </summary>
    public class ExportingDocument
    {
        /// <summary>
        /// 默认实例化方法
        /// </summary>
        public ExportingDocument()
        {
            Formulas = new List<SheetFormula>();
            DataSets = new List<SheetDataSet>();
        }

        /// <summary>
        /// Sheet公式集合
        /// </summary>
        public List<SheetFormula> Formulas { get; set; }

        /// <summary>
        /// Sheet数据集集合
        /// </summary>
        public List<SheetDataSet> DataSets { get; set; }

        public ICell Extend { get; set; }
    }

    /// <summary>
    /// Sheet公式
    /// </summary>
    public class SheetFormula
    {
        /// <summary>
        /// 默认实例化方法
        /// </summary>
        public SheetFormula()
        {
            Parameters = new List<string>();
        }

        /// <summary>
        /// 公式
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Cell
        /// </summary>
        public ICell Cell { get; set; }

        /// <summary>
        /// 参数集合
        /// </summary>
        public List<string> Parameters { get; set; }
    }

    /// <summary>
    /// Sheet数据集
    /// </summary>
    public class SheetDataSet
    {
        /// <summary>
        /// 默认实例化方法
        /// </summary>
        public SheetDataSet()
        {
            Properties = new Dictionary<ICell, CellDataMap>();
        }
        /// <summary>
        /// 起始行索引
        /// </summary>
        public int StartRowIndex { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string SetName { get; set; }

        /// <summary>
        /// 属性集合
        /// </summary>
        public Dictionary<ICell, CellDataMap> Properties { get; set; }

        /// <summary>
        /// 添加映射。
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="key"></param>
        /// <param name="format"></param>
        public void AddDataMap(ICell cell, string key, string format)
        {
            Properties.Add(cell, new CellDataMap(key, format));
            cell.SetCellValue(format);
        }
    }

    /// <summary>
    /// 单元格的数据映射。
    /// </summary>
    public class CellDataMap
    {
        public CellDataMap(string key, string format)
        {
            Key = key;
            Format = format;
        }

        /// <summary>
        /// 数据字段名称。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 填充的格式。如 [PROGRESS] % 。
        /// </summary>
        public string Format { get; set; }
    }
}
