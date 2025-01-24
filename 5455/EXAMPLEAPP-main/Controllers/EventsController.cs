using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var query = _context.Events.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetEvents(int id)
    {
        var even = _context.Events.Find(id);

        if (even == null)
        {
            return NotFound();
        }

        return Ok(even);
    }

    [HttpPut("{id}")]
    public IActionResult PutEvents(int id, Event even)
    {
        var dbTest = _context.Events.AsNoTracking().FirstOrDefault(x => x.Id == even.Id);
        if (id != even.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(even);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Test> PostEvents(Event even)
    {
        var dbExercise = _context.Events.Find(even.Id);
        if (dbExercise == null)
        {
            _context.Add(even);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetEvents), new { Id = even.Id }, even);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTest(int id)
    {
        var even = _context.Events.Find(id);
        if (even == null)
        {
            return NotFound();
        }

        _context.Remove(even);
        _context.SaveChanges();

        return NoContent();
    }
}
