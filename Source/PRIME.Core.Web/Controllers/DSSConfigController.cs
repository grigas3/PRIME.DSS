using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Treatment;
using PRIME.Core.Web.Controllers;
using PRIME.Core.Web.Extensions;

namespace PRIME.Core.Web.Controllers
{
    [Route("api/v1/[controller]")]
public class DSSConfigController : Controller
{
    #region Controllers

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="context">Data Context</param>
    /// <param name="logger">Logger</param>
    public DSSConfigController(IRepositoryService context, ILogger<DSSController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    /// <summary>
    ///     Get DSS Config
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get(string id)
    {
        //If Id is empty then we load the default model
        if (string.IsNullOrEmpty(id))
        {
            //FOR Test
            var ret = ModelExtensions.GetDSSConfig("modelyesno.json");
            return Ok(ret);
        }

        else
        {
            var model = await _context.FindAsync<DSSModel>(int.Parse(id));

            if (model == null)
                return NotFound("DSS Model not found");

            var ret = model.Config;
            if (!string.IsNullOrEmpty(model.Config))
            {
                var classifier = TreatmentClassifier.FromJson(model.Config);
                return Ok(classifier);
            }

            return Ok(new TreatmentClassifier
            {
                Code = model.Code,
                Name = model.Name
            });
        }
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] TreatmentClassifier model)
    {
        try
        {
            var dss = await _context.FindAsync<DSSModel>(int.Parse(model.Id));
            dss.Config = JsonConvert.SerializeObject(model);
            await _context.InserOrUpdateAsync(dss);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post DSS Model failed");
            return BadRequest("Post DSS Model failed");
        }


        return Ok(model);
    }

    #region Private Declarations

    private readonly IRepositoryService _context;
    private readonly ILogger _logger;

    #endregion
}

}