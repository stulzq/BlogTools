using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Polly;

namespace XC.BlogTools.Util
{
    /// <summary>
    /// 客户端文件上传工具类
    /// Author:晓晨Master(李志强)
    /// </summary>
    public class ClientUploader
    {
        public HttpClient Client { get; }

        public ClientUploader()
        {
            Client=new HttpClient();
        }

        public ClientUploader(string baseAddress)
        {
            Client = new HttpClient(){BaseAddress = new Uri(baseAddress)};
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">文件数据</param>
        /// <param name="fileUploadPath">上传地址</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentName">参数名称</param>
        /// <param name="headers">附加http请求header</param>
        /// <returns></returns>
        public async Task<string> UploadAsync(byte[] file,string fileUploadPath,string fileName, string contentName="",Dictionary<string,string> headers=null)
        {
            Client.DefaultRequestHeaders.Clear();

            //加入附加header
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    Client.DefaultRequestHeaders.Add(key,headers[key]);
                }
            }

            var policy = Policy.Handle<Exception>().RetryAsync(3, (exception, retryCount) =>
            {
                Console.WriteLine("上传失败，正在重试 {0}，异常：{1}", retryCount, exception.Message);
            });

            try
            {
                var res = await policy.ExecuteAsync<string>(async () =>
                {
                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        var streamContent = new StreamContent(new MemoryStream(file));
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                        content.Add(streamContent, contentName, fileName);

                        var result = await Client.PostAsync(fileUploadPath, content);
                        result.EnsureSuccessStatusCode();
                        return await result.Content.ReadAsStringAsync();
                    }
                });

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("上传失败,异常：{0}", e.Message);
                throw;
            }
            
        }

        /// <summary>
        /// 根据文件路径上传文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileUploadPath">上传地址</param>
        /// <param name="contentName">参数名称</param>
        /// <param name="headers">附加http请求header</param>
        /// <returns></returns>
        public async Task<string> UploadAsync(string filePath, string fileUploadPath, string contentName = "", Dictionary<string, string> headers = null)
        {
            Client.DefaultRequestHeaders.Clear();

            //加入附加header
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    Client.DefaultRequestHeaders.Add(key, headers[key]);
                }
            }

            var policy = Policy.Handle<Exception>().RetryAsync(3, (exception, retryCount) =>
            {
                Console.WriteLine("上传失败，正在重试 {0}，异常：{1}", retryCount, exception.Message);
            });

            try
            {
                var res = await policy.ExecuteAsync<string>(async () =>
                {
                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        var fileInfo = new FileInfo(filePath);
                        var streamContent = new StreamContent(File.OpenRead(filePath));
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileInfo.Name));
                        content.Add(streamContent, contentName, fileInfo.Name);

                        var result = await Client.PostAsync(fileUploadPath, content);
                        result.EnsureSuccessStatusCode();
                        return await result.Content.ReadAsStringAsync();
                    }
                });

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("上传失败,异常：{0}", e.Message);
                throw;
            }
            
        }
    }

}