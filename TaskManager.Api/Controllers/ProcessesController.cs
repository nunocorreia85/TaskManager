using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using TaskManager.Core;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessesController : ControllerBase
    {
        private readonly ILogger<ProcessesController> _logger;
        private readonly IOperatingSystem _operatingSystem;
        private readonly IDiagnosticContext _diagnosticContext;

        public ProcessesController(ILogger<ProcessesController> logger,
            IDiagnosticContext diagnosticContext,
            IOperatingSystem operatingSystem)
        {
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));
            _operatingSystem = operatingSystem ??
                               throw new ArgumentNullException(nameof(operatingSystem));
            _diagnosticContext = diagnosticContext ??
                                 throw new ArgumentNullException(nameof(diagnosticContext));
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _logger.LogDebug("Method Delete");
            return Ok();
        }

        [HttpPost]
        public IActionResult Add(string addMethod, long id, Priority priority)
        {
            _logger.LogDebug("Method Add");
            if (!Enum.TryParse<AddMethod>(addMethod, true, out var addMethodEnum)) addMethodEnum = AddMethod.Default;

            if (!_operatingSystem.Add(new Process(id, priority), addMethodEnum)) return NoContent();

            return Ok();
        }

        [HttpGet]
        public IActionResult Get(string sortBy)
        {
            _diagnosticContext.Set("CatalogLoadTime", 1423);
            _logger.LogDebug("Method Get");
            if (!Enum.TryParse<SortBy>(sortBy, true, out var sortByEnum)) sortByEnum = SortBy.Id;

            return new OkObjectResult(_operatingSystem.List(sortByEnum));
        }
    }
}