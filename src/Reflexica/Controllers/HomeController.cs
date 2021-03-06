﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Reflexica.Models;
using RestSharp;

namespace Reflexica.Controllers
{
    public class HomeController : Controller
    {
      private const string TEMP_PATH = @"D:\Reflexica";

      private const string apiUrl = "http://192.168.84.140/cgi-enabled/Reflexica.cgi";


      public ActionResult Index()
      {
        Journal journal = new Journal();
          return View(journal);
      }

      [HttpPost]
      public ActionResult UploadFiles(IEnumerable files)
      {
          foreach (HttpPostedFileBase file in files)
          {
              string filePath = Path.Combine(TEMP_PATH, file.FileName);
              System.IO.File.WriteAllBytes(filePath, ReadData(file.InputStream));
          }

          return Json("All files have been successfully stored.");
      }

      //[HttpPost]
      public ActionResult Submit(Journal journals)
      {
        try
        {
          ca();
          return  new JsonResult();
        }
        catch (Exception)
        {
          throw;
        }
      }

      private void ca()
      {
        var client = new RestClient("http://192.168.84.140/cgi-enabled/Reflexica.cgi");
        var request = new RestRequest(Method.POST);
        request.AddHeader("postman-token", "0a76f452-2211-1654-ff29-f24ab7a88378");
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
        request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"index.html\"\r\nContent-Type: text/html\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"style\"\r\n\r\nBasic\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"Client\"\r\n\r\nSpringer\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"Language\"\r\n\r\nEn\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"Format\"\r\n\r\ncolor\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);
      }

      //...POST working fine
      private  void ClientCall()
      {
        try
        {
          var client = new HttpClient();
          var fileInfo = new FileInfo("D:\\index.html");
          var fileContent = new StreamContent(fileInfo.OpenRead());
          fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
          {
            Name = "\"file\"",
            FileName = "\"" + fileInfo.Name + "\""
          };
          fileContent.Headers.ContentType =
              MediaTypeHeaderValue.Parse(MimeMapping.GetMimeMapping(fileInfo.Name));
          // This is the postdata
          var postData = new List<KeyValuePair<string, string>>();
          postData.Add(new KeyValuePair<string, string>("file", fileContent.ToString()));
          postData.Add(new KeyValuePair<string, string>("style", "Basic"));
          postData.Add(new KeyValuePair<string, string>("Client ", "Springer"));
          postData.Add(new KeyValuePair<string, string>("style", "Basic"));
          postData.Add(new KeyValuePair<string, string>("Language ", "En"));
          postData.Add(new KeyValuePair<string, string>("Format", "color"));

         // HttpContent content = new FormUrlEncodedContent(postData);
          MultipartFormDataContent form = new MultipartFormDataContent();
          form.Add(new StringContent("Basic"), "style");
          form.Add(new StringContent("Springer"), "Client");
          form.Add(new StringContent("Basic"), "style");
          form.Add(new StringContent("En"), "Language");
          form.Add(new StringContent("color"), "Format");
          client.PostAsync("http://192.168.84.140/cgi-enabled/Reflexica.cgi", form).ContinueWith(
              (postTask) =>
              {
                var res = postTask.Result.Content.ReadAsStringAsync();
                postTask.Result.EnsureSuccessStatusCode();
              });
        }
        catch (Exception ex)
        {
          throw new Exception(ex.Message);
        }
        
      }
      //...GET working fine
      //private async Task ClientCall()
      //{
      //  using (HttpClient client = new HttpClient())
      //  using (HttpResponseMessage response = await client.GetAsync("http://192.168.84.140/cgi-enabled/Download.cgi?File= /var/www/html/cgi-enabled/OUTPUT/508_2016_551_Article.docm-org.html"))
      //  using (HttpContent content = response.Content)
      //  {
      //    // ... Read the string.
      //    string result = await content.ReadAsStringAsync();

      //    // ... Display the result.
      //    if (result != null &&
      //        result.Length >= 50)
      //    {
      //      Console.WriteLine(result.Substring(0, 50) + "...");
      //    }
      //  }
      //}

      private byte[] ReadData(Stream stream)
      {
          byte[] buffer = new byte[16 * 1024];

          using (MemoryStream ms = new MemoryStream())
          {
              int read;
              while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
              {
                  ms.Write(buffer, 0, read);
              }

              return ms.ToArray();
          }
      }
    }
}