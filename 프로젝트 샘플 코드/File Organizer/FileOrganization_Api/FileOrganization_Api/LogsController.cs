using FileOrganization_Api.Data;
using FileOrganization_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileOrganization_Api.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LogsController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> PostLog(OrganizeLog log)
        {
            _db.OrganizeLogs.Add(log);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
