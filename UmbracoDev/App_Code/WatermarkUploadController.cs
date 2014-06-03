
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Core.Models;


public class WatermarkUploadController : UmbracoApiController
{
  
    [HttpPost]
    public HttpResponseMessage UploadWatermark()
    {
   

        var link = "";
        var fileName = "";
        HttpResponseMessage result = null;
        var httpRequest = HttpContext.Current.Request;
        if (httpRequest.Files.Count > 0)
        {
            foreach (string file in httpRequest.Files)
            {
                var mediaService = Services.MediaService;
                var postedFile = httpRequest.Files[file];
                var umbracoMediaType = "Image";
                //TODO Kolla Contenttype och sätt som rätt umbMediaType 
                switch (postedFile.ContentType.ToLower())
                {
                    case "image/jpeg":
                        break;
                    case "application/pdf":
                        umbracoMediaType = "File";
                        break;
                }
                		

                var newImage = mediaService.CreateMedia(postedFile.FileName, -1, umbracoMediaType);
                newImage.SetValue("umbracoFile", postedFile);

                mediaService.Save(newImage);

                link = newImage.GetValue("umbracoFile") as string;
                fileName = newImage.Name;

            }
            result = Request.CreateResponse(HttpStatusCode.Created, new { filelink = link, filename = fileName });
        }
        else
        {
            result = Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        return result; 

    }


}
