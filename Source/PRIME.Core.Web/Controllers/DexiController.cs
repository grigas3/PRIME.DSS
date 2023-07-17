using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class DexiController : Controller
    {
        #region Controllers

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="logger">Logger</param>
        public DexiController()
        {

        }

        #endregion


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            try
            {
                var name = file.FileName;
                var rootPath = "./DexiModels";
                var fullName = System.IO.Path.Combine(rootPath, name);
                if (System.IO.File.Exists(fullName))
                {
                    return BadRequest("File exists");
                }
                System.IO.StreamReader str = new System.IO.StreamReader(file.OpenReadStream());
                System.IO.StreamWriter strOut = new System.IO.StreamWriter(fullName);

                strOut.Write(str.ReadToEnd());
                strOut.Dispose();
                str.Dispose();

                return Ok(new
                {
                    FileName = fullName
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Post DSS Model failed");
                return BadRequest("Post DSS Model failed");
            }



        }

        #region Private Declarations

        private readonly IRepositoryService _context;
        private readonly ILogger _logger;

        #endregion
    }
}