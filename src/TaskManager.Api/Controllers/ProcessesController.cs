using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using TaskManager.Core;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController, Route("[controller]")]
    public class ProcessesController : ControllerBase
    {
        private readonly ILogger<ProcessesController> _logger;

        private readonly IOperatingSystem _operatingSystem;

        public ProcessesController(ILogger<ProcessesController> logger, IOperatingSystem operatingSystem)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operatingSystem = operatingSystem ?? throw new ArgumentNullException(nameof(operatingSystem));
        }

        [HttpGet]
        public IActionResult Get(string sortBy)
        {
            _logger.LogDebug("Method Get");

            if (!Enum.TryParse<SortBy>(sortBy, true, out var sortByEnum)) 
                sortByEnum = SortBy.Id;

            return new OkObjectResult(_operatingSystem.List(sortByEnum));
        }

        [HttpPost]
        public IActionResult Add(string addMethod, Process process)
        {
            _logger.LogDebug("Method Add");

            if (!Enum.TryParse<AddMethod>(addMethod, true, out var parsedAddMethod)) 
                parsedAddMethod = AddMethod.Default;

            if (!_operatingSystem.Add(parsedAddMethod, new Process(process.Id, process.Priority))) 
                return NoContent();

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _logger.LogDebug("Method Delete");

            return Ok();
        }
    }
}