using ITB2203Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly DataContext _context;

        public AttendeesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Attendee>> GetAttendees(string? name = null, int? daysBeforeEvent = null)
        {
            var query = _context.Attendees!.AsQueryable();

            if (name != null)
                query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

            if (daysBeforeEvent.HasValue)
            {
                var thresholdDate = DateTime.UtcNow.AddDays(-daysBeforeEvent.Value);
                query = query.Where(x => x.RegistrationTime <= thresholdDate);
            }

            return Ok(query);
        }

        [HttpGet("{id}")]
        public ActionResult<Attendee> GetAttendee(int id)
        {
            var attendee = _context.Attendees!.Find(id);

            if (attendee == null)
            {
                return NotFound();
            }

            return Ok(attendee);
        }

        [HttpPut("{id}")]
        public IActionResult PutAttendee(int id, Attendee attendee)
        {
            if (!attendee.Email.Contains("@"))
            {
                return BadRequest("Email must contain '@'.");
            }

            var dbAttendee = _context.Attendees!.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (dbAttendee == null)
            {
                return NotFound();
            }

            if (_context.Attendees.Any(x => x.Email == attendee.Email && x.Id != id))
            {
                return BadRequest("An attendee with the same email already exists.");
            }

            var eventInfo = _context.Events!.Find(attendee.EventID);
            if (eventInfo == null)
            {
                return NotFound("Event not found.");
            }

            if (attendee.RegistrationTime > eventInfo.Date)
            {
                return BadRequest("Registration time cannot be later than the event date.");
            }

            _context.Update(attendee);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Attendee> PostAttendee(Attendee attendee)
        {
            if (!attendee.Email.Contains("@"))
            {
                return BadRequest("Email must contain '@'.");
            }

            if (_context.Attendees.Any(x => x.Email == attendee.Email))
            {
                return BadRequest("An attendee with the same email already exists.");
            }

            var eventInfo = _context.Events!.Find(attendee.EventID);
            if (eventInfo == null)
            {
                return NotFound("Event not found.");
            }

            if (attendee.RegistrationTime > eventInfo.Date)
            {
                return BadRequest("Registration time cannot be later than the event date.");
            }

            if (_context.Speakers.Any(x => x.Email == attendee.Email))
            {
                return BadRequest("An attendee cannot have the same email as a speaker at the event.");
            }

            _context.Add(attendee);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAttendee), new { id = attendee.Id }, attendee);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAttendee(int id)
        {
            var attendee = _context.Attendees!.Find(id);
            if (attendee == null)
            {
                return NotFound();
            }

            _context.Remove(attendee);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
