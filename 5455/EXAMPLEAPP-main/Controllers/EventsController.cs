using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace ITB2203Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly DataContext _context;

        public EventsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Event>> GetEvents(string? name = null)
        {
            var query = _context.Events!.AsQueryable();

            if (name != null)
                query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

            return Ok(query);
        }

        [HttpGet("{id}")]
        public ActionResult<Event> GetEvent(int id)
        {
            var eventItem = _context.Events!.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        [HttpPut("{id}")]
        public IActionResult PutEvent(int id, Event eventItem)
        {
            if (id != eventItem.Id)
            {
                return BadRequest();
            }

            if (!_context.Speakers!.Any(s => s.Id == eventItem.SpeakerId))
            {
                return NotFound("Speaker not found");
            }

            if (!IsValidEmail(eventItem.SpeakerId))
            {
                return BadRequest("Speaker email is invalid");
            }

            _context.Entry(eventItem).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Event> PostEvent(Event eventItem)
        {
            if (!_context.Speakers!.Any(s => s.Id == eventItem.SpeakerId))
            {
                return NotFound("Speaker not found");
            }

            if (!IsValidEmail(eventItem.SpeakerId))
            {
                return BadRequest("Speaker email is invalid");
            }

            _context.Events!.Add(eventItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, eventItem);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var eventItem = _context.Events!.Find(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            _context.SaveChanges();

            return NoContent();
        }

        private bool IsValidEmail(int speakerId)
        {
            var speaker = _context.Speakers!.Find(speakerId);
            return speaker != null && speaker.Email.Contains("@");
        }
    }
}