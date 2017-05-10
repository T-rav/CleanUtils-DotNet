﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CleanArchitecture.Utils.Output;

namespace CleanArchitecture.Utils.HttpResponses
{
    public class DownloadFileResult : IHttpActionResult
    {
        public IFileOutput FileOutput { get; }

        public DownloadFileResult(IFileOutput fileOutput)
        {
            FileOutput = fileOutput;
        }

        Task<HttpResponseMessage> IHttpActionResult.ExecuteAsync(CancellationToken cancellationToken)
        {
            var pushStreamContent = new PushStreamContent((outputStream, httpContent, transportContext) =>
            {
                FileOutput.Write(outputStream);
                outputStream.Flush();
                outputStream.Close();
            });

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = pushStreamContent,
            };

            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = FileOutput.FileName
            };

            return Task.FromResult(httpResponseMessage);
        }
    }
}