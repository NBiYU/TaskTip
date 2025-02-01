using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using TaskTip.Models.DataModel;

namespace TaskTip.Common.Extends
{
    internal static class ClassExtent
    {
        //public static List<TreeInfo> GetFiles(this TreeInfo menuTree)
        //{
        //    var list = new List<TreeInfo>();
        //    foreach (var tree in menuTree.Menu.Files)
        //    {
        //        if (tree.Menu.Files.Count != 0)
        //        {
        //            list.AddRange(tree.GetFiles());
        //        }
        //        list.Add(tree);
        //    }
        //    return list;
        //}

        public static string GetString(this FlowDocument document)
        {
            var settings = new System.Xml.XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = System.Xml.NewLineHandling.Replace
            };

            StringBuilder stringBuilder = new StringBuilder();

            using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringBuilder, settings))
            {
                XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(xmlWriter);
                XamlWriter.Save(document, manager);
            }
            return stringBuilder.ToString();
        }
    }

}
