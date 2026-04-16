using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

namespace ElectricalOverStressData
{
    public class DataTool
    {
        public static double FormulaToValue(string Formula, double Parameter)
        {
            try
            {
                DataTable data = new DataTable();
                string formula = Formula.ToUpper().Replace("X", Parameter.ToString());  //字符串Parameter替换Formula
                double result = Convert.ToDouble(data.Compute(formula, ""));
                return result;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取Class文件
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object GetClassDeserializationXML(string data, Type type)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);  //将当前流中的位置设置为指定值。第一个参数流内的新位置，第二个参数用作查找引用点。
                byte[] fi = System.Text.ASCIIEncoding.UTF8.GetBytes(data);  //GetBytes()方法：将一组字符编码为一个字节序列。
                ms.Write(fi, 0, fi.Length);
                ms.Flush();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                XmlSerializer xs = new XmlSerializer(type);
                return xs.Deserialize(ms);  //反序列化
            }
        }
        /// <summary>
        /// 获取XML文件
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="value">目标对象</param>
        /// <returns></returns>
        public static byte[] GetXMLSerializationClass(Type type, object value)
        {
            byte[] xml = null;  //声明一个byte类型变量
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(type);
                xs.Serialize(ms, value);  //序列化
                xml = ms.ToArray();  //将流内容写入字节组
            }
            return xml;
        }

        public static string[] GetPlanNameList()
        {
            List<string> Result = new List<string>();
            DirectoryInfo thefolder = new DirectoryInfo(Global.PlanPath);
            FileInfo[] files = thefolder.GetFiles();
            foreach (FileInfo fi in files)
            {
                Result.Add(fi.Name.Replace(".xml", "")); //把.xml去掉
            }
            return Result.ToArray();  //将result里的值复制到新数组，返回一个新数组
        }
    }
}
