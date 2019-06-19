using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using OCR_Operations;

namespace PnG_Cpe_OCR.Controllers
{
    public class ValuesController : ApiController
    {
        // POST api/values
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            //dynamic model = jObject;
            //dynamic id = jObject["cpeEntryId"];
            //try
            //{
            //    string root = HttpContext.Current.Server.MapPath("~/App_Data");
            //    string fileName = Path.GetFileNameWithoutExtension(model.file.FileName);
            //    string extension = Path.GetExtension(model.file.FileName);
            //    //string fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\") + 1);

            //    string filePath = root + "/" + fileName + extension;
            //    model.file.SaveAs(filePath); //Save file in App_Data folder

            //    OcrProgram ocrProgram = new OcrProgram();
            //    bool success = ocrProgram.InsertFileData(model.cpeEntryId, model.cpeDefinitionId, filePath);
            //    if (success)
            //    {
            //        return Request.CreateResponse(HttpStatusCode.OK);
            //    }
            //    else
            //    {
            //        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, new Exception("Data Not Inserted"));
            //    }
            //}
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            int cpeEntryId = 0, cpeDefinitionId = 0;
            string filePath = "";
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);
                    fileInfo.MoveTo(Path.ChangeExtension(file.LocalFileName, ".png"));
                    filePath = fileInfo.FullName;
                }

                // Show all the key-value pairs.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key.Equals("cpeEntryId"))
                        {
                            cpeEntryId = Convert.ToInt32(val);
                        }
                        else if (key.Equals("cpeDefinitionId"))
                        {
                            cpeDefinitionId = Convert.ToInt32(val);
                        }
                    }
                }
                if (cpeEntryId == 0 || cpeDefinitionId == 0 || filePath == "")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, new Exception("Invalid Data"));
                }
                OcrProgram ocrProgram = new OcrProgram();
                bool success = ocrProgram.InsertFileData(cpeEntryId, cpeDefinitionId, filePath);
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, new Exception("Data Not Inserted"));
                }
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}
