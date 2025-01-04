using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

using TaskTip.Common.Extends;
using TaskTip.Models.Entities;
using TaskTip.Services;

namespace TaskTip.Common.Converter.Version
{
    public class RecordFileVersionConvert
    {
        public void VersionIterator()
        {
            var dir = new DirectoryInfo(GlobalVariable.RecordFilePath);
            if (dir.Exists)
            {
                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    var guid = Path.GetFileNameWithoutExtension(file.FullName);
                    var xamlPath = Path.Combine(GlobalVariable.RecordFilePath, "Xaml", guid) + GlobalVariable.EndFileFormat;

                    var path = Path.Combine(GlobalVariable.RecordFilePath, guid) + GlobalVariable.EndFileFormat;
                    var obj = ReadJsonContent(path);
                    var content = obj.Text;
                    //if (!string.IsNullOrEmpty(content)) content = Convert1(content);
                    if (!string.IsNullOrEmpty(content)) content = Convert2(content);
                    if (!string.IsNullOrEmpty(content)) content = Convert3(content);

                }
            }
        }

        private string Convert1(RecordFileModel content)
        {
            return null;
        }

        private string Convert2(string content)
        {
            try
            {
                var document = XamlReader.Parse(content);
                return (document as FlowDocument).GetString();
            }
            catch (XamlParseException ex)
            {
                return string.Empty;
            }
        }
        private string Convert3(string content)
        {
            return string.Empty;
        }
        private RecordFileModel ReadJsonContent(string path)
        {

            var file = File.ReadAllText(path);

            if (string.IsNullOrEmpty(file))
                throw new Exception($"{path}的内容为空！");

            return JsonConvert.DeserializeObject<RecordFileModel>(file) ?? new RecordFileModel();

        }
        private string ConvertPlainTextToXaml(string text)
        {
            return
                $"<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run Foreground=\"#FF000000\" xml:lang=\"zh-cn\">{text}</Run></Paragraph></FlowDocument>";
        }
    }
}
