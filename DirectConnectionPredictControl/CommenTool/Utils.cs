using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DirectConnectionPredictControl.CommenTool
{
    class Utils
    {
        public static int timeInterval = 1000;
        public static string formatN1 = "{0:N1}";
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static short PositiveToNegative(byte high, byte low)
        {
            short res = 0;
            if ((high & 0x80) == 0x80)
            {
                res = (short)(high * 256 + low);
                res = (short)(res - 1);
                res = (short)-(~res);
            }
            else
            {
                res = (short)(high * 256 + low);
            }
            return res;
        }

        /// <summary>
        /// 读取xml中的列头数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="colunmName"></param>
        /// <returns></returns>
        public static IList<string> getXml(string fileName, string colunmName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            XmlNode root = document.SelectSingleNode(colunmName);
            XmlNodeList list = root.ChildNodes;
            IList<string> header = new List<string>();
            foreach (var item in list)
            {
                XmlNode node = (XmlNode)item;
                header.Add(node.InnerText);
            }
            return header;
        }

        
    }
}
