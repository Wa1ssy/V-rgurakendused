using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendeesController : ControllerBase
{
    private readonly DataContext _context;

    public AttendeesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Attendee>> GetAttendees(string? name = null)
    {
        var query = _context.Attendees.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetAttendees(int id)
    {
        var attendee = _context.Attendees.Find(id);

        if (attendee == null)
        {
            return NotFound();
        }

        return Ok(attendee);
    }

    [HttpPut("{id}")]
    public IActionResult PutAttendees(int id, Attendee attendee)
    {
        var dbTest = _context.Attendees.AsNoTracking().FirstOrDefault(x => x.Id == attendee.Id);
        if (id != attendee.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(attendee);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Test> PostAttendees(Attendee attendee)
    {
        var dbExercise = _context.Attendees.Find(attendee.Id);
        if (dbExercise == null)
        {
            _context.Add(attendee);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAttendees), new { Id = attendee.Id }, attendee);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAttendees(int id)
    {
        var attendee = _context.Attendees.Find(id);
        if (attendee == null)
        {
            return NotFound();
        }

        _context.Remove(attendee);
        _context.SaveChanges();

        return NoContent();
    }
}
