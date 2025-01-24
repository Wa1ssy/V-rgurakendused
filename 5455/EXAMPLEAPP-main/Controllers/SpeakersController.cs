using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeakersController : ControllerBase
{
    private readonly DataContext _context;

    public SpeakersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Speaker>> GetSpeakers(string? name = null)
    {
        var query = _context.Speakers.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSpeakers(int id)
    {
        var speaker = _context.Speakers.Find(id);

        if (id == null)
        {
            return NotFound();
        }

        return Ok(speaker);
    }

    [HttpPut("{id}")]
    public IActionResult PutSpeakers(int id, Speaker speaker)
    {
        var dbTest = _context.Speakers.AsNoTracking().FirstOrDefault(x => x.Id == speaker.Id);
        if (id != speaker.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(speaker);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Speaker> PostTest(Speaker speaker)
    {
        var dbExercise = _context.Speakers.Find(speaker.Id);
        if (dbExercise == null)
        {
            _context.Add(speaker);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSpeakers), new { Id = speaker.Id }, speaker);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTest(int id)
    {
        var speaker = _context.Speakers.Find(id);
        if (speaker == null)
        {
            return NotFound();
        }

        _context.Remove(speaker);
        _context.SaveChanges();

        return NoContent();
    }
}
