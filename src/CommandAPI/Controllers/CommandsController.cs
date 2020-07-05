using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Models;
using System;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandDBContext _context;

        public CommandsController(CommandDBContext context) => _context = context;

        //GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandItems() => _context.CommandItems;


        //GET api/commands/{id}
        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandItem(int id)
        {
            var commandItem = _context.CommandItems.Find(id);
            if (commandItem == null)
            {
                return NotFound();
            }
            return commandItem;
        }
    }
}