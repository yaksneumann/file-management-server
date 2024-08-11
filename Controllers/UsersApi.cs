/*
 * API for file upload
 *
 * This is a simple API for file upload
 *
 * OpenAPI spec version: 1.0.0
 * Contact: you@your-company.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IO.Swagger.Attributes;

using Microsoft.AspNetCore.Authorization;
using IO.Swagger.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IO.Swagger.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private string documentDirectory { get; set; }
        private string fileDirectory { get; set; }

        public UsersApiController(IConfiguration configuration)
        {
            //     C:\\Users\\whywh\\OneDrive\\Documents\\my-files\\
            documentDirectory = configuration.GetValue<string>("AppSettings:DocumentDirectory");
            fileDirectory = configuration.GetValue<string>("AppSettings:FileDirectory");
            fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), documentDirectory, fileDirectory);
        }

        /// <summary>
        /// gets a file
        /// </summary>
        /// <remarks>download a file by id</remarks>
        /// <param name="fileName">pass the file name</param>
        /// <response code="200">returns a Document</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/document/{fileName}")]
        [ValidateModelState]
        [SwaggerOperation("DownloadDocument")]
        [SwaggerResponse(statusCode: 200, type: typeof(Document), description: "returns a Document")]
        public virtual async Task<IActionResult> DownloadDocument([FromRoute][Required] string fileName)
        {
            Document document;

            if (!System.IO.File.Exists(fileDirectory + fileName))
                return StatusCode(400, "אין כזה קובץ");

            document = new Document();
            document.Name = fileName;
            document.DocumentContent = Convert.ToBase64String(System.IO.File.ReadAllBytes(fileDirectory + fileName));

            return StatusCode(200, document);
        }

        /// <summary>
        /// gets all files
        /// </summary>
        /// <remarks>gets all Documents</remarks>
        /// <response code="200">returns a list of Documents</response>
        [HttpGet]
        [Route("/documents")]
        [ValidateModelState]
        [SwaggerOperation("GetDocuments")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Document>), description: "returns a list of Documents")]
        public virtual async Task<IActionResult> GetDocuments()
        {
            string[] fileNames;
            Document document;
            List<Document> documents;

            documents = new List<Document>();
            fileNames = Directory.GetFiles(fileDirectory);
            //trim the begining of path
            foreach (string fileName in fileNames)
            {
                document = new Document();
                document.Name = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                documents.Add(document);
            }
            return StatusCode(200, documents);
        }

        /// <summary>
        /// uploads a file
        /// </summary>
        /// <remarks>Uploads a file to impress הפניקס</remarks>
        /// <param name="body">Document to upload</param>
        /// <response code="201">file uploaded</response>
        /// <response code="400">פעולת ההעלאת הקובץ נכשלה</response>
        [HttpPost]
        [Route("/document")]
        [ValidateModelState]
        [SwaggerOperation("UploadDocument")]
        public virtual async Task<IActionResult> UploadDocument([FromBody] Document body)
        {
            try
            {
                if (body.Name == null)
                    return StatusCode(400, "שם קובץ חובה");
                byte[] bytes = Convert.FromBase64String(body.DocumentContent);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                System.IO.File.WriteAllBytes(fileDirectory + body.Name, bytes);
                return StatusCode(201, "הקובץ הועלה בהצלחה");
            }
            catch (System.Exception)
            {
                return StatusCode(500, "פעולת ההעלאת הקובץ נכשלה");
            }

        }
    }
}
