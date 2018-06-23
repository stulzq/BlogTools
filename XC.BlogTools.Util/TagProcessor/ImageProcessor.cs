using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XC.BlogTools.Util.TagProcessor
{
    public class ImageProcessor:ITagProcessor
    {
        const string MatchRule= @"!\[.*?\]\((.*?)\)";
        public List<string> Process(string content)
        {
            List<string> result=new List<string>();

            var matchs = Regex.Matches(content, MatchRule, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            foreach (Match match in matchs)
            {
                var val = match.Groups[1].Value;
                //只需要解析本地图片
                if (!val.StartsWith("http")&& !result.Contains(val))
                {
                    result.Add(val);
                }
                
            }

            return result;
        }

        public string Replace(Dictionary<string, string> repleceDic,string content)
        {
            foreach (var key in repleceDic.Keys)
            {
                content = content.Replace(key, repleceDic[key]);
            }

            return content;
        }
    }
}