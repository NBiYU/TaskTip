using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Markup;

namespace TaskTip.Common.Converter.Version
{
    public class FlowHTMLConverter
    {
        public string ConvertFlowDocumentToHtml(string content)
        {
            if (!content.Contains("<FlowDocument", StringComparison.OrdinalIgnoreCase)) return content;
            var flowDocument = XamlReader.Parse(content); ;
            if (flowDocument == null) return string.Empty;
            return ConvertFlowDocumentToHtml((FlowDocument)flowDocument);
        }
        public string ConvertFlowDocumentToHtml(FlowDocument flowDocument)
        {
            var stringBuilder = new StringBuilder();

            foreach (var block in flowDocument.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    stringBuilder.Append("<p>");
                    foreach (var inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            stringBuilder.Append(run.Text);
                        }
                        else if (inline is InlineUIContainer uiContainer && uiContainer.Child is System.Windows.Controls.Image image)
                        {
                            stringBuilder.Append(ConvertImageToHtml(image));
                        }
                    }
                    stringBuilder.Append("</p>");
                }
                else if (block is BlockUIContainer blockUI && blockUI.Child is System.Windows.Controls.Image image)
                {
                    stringBuilder.Append(ConvertImageToHtml(image));
                }
            }

            return $"<html><body>{stringBuilder}</body></html>";
        }
        #region FLowDocumentToHtml
        private string GetInlineContent(InlineCollection inlines)
        {
            var contentBuilder = new StringBuilder();

            foreach (var inline in inlines)
            {
                if (inline is Run run)
                {
                    contentBuilder.Append(run.Text);
                }
                else if (inline is Bold bold)
                {
                    contentBuilder.Append($"<b>{GetInlineContent(bold.Inlines)}</b>");
                }
                else if (inline is Italic italic)
                {
                    contentBuilder.Append($"<i>{GetInlineContent(italic.Inlines)}</i>");
                }
                else if (inline is Hyperlink hyperlink)
                {
                    var navigateUri = hyperlink.NavigateUri?.ToString() ?? "#";
                    contentBuilder.Append($"<a href=\"{navigateUri}\">{GetInlineContent(hyperlink.Inlines)}</a>");
                }
                // 可以根据需要添加其他 Inline 类型处理逻辑
            }

            return contentBuilder.ToString();
        }
        private string ConvertBlockToHtml(Block block)
        {
            if (block is Paragraph paragraph)
            {
                var contentBuilder = new StringBuilder("<p>");
                contentBuilder.Append(GetInlineContent(paragraph.Inlines));
                contentBuilder.Append("</p>");
                return contentBuilder.ToString();
            }
            return string.Empty;
        }
        private string ConvertImageToHtml(System.Windows.Controls.Image image)
        {
            if (image.Source is BitmapImage bitmapImage)
            {
                string imagePath = bitmapImage.UriSource?.LocalPath;

                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    // 将图片转换为 Base64
                    string base64String = ConvertImageToBase64(imagePath);
                    string mimeType = GetMimeType(imagePath);
                    return $"<img src=\"data:{mimeType};base64,{base64String}\" />";
                }

                // 如果路径无效，返回占位符
                return "<img src=\"\" alt=\"Image not found\" />";
            }

            return string.Empty;
        }
        private string ConvertImageToBase64(string imagePath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }
        private string GetMimeType(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath)?.ToLowerInvariant();
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream",
            };
        }
        #endregion
    }
}
