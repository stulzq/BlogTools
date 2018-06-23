using System.Collections.Generic;

namespace XC.BlogTools.Util.TagProcessor
{
    public interface ITagProcessor
    {
        /// <summary>
        /// 处理内容 提取出需要替换的内容列表
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        List<string> Process(string content);

        /// <summary>
        /// 替换内容，返回替换后结果
        /// </summary>
        /// <param name="repleceDic">需要替换的字符串</param>
        /// <param name="content">原内容</param>
        /// <returns></returns>
        string Replace(Dictionary<string, string> repleceDic,string content);
    }
}