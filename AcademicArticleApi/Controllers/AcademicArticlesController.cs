using Microsoft.AspNetCore.Mvc;
using AcademicArticleApi.Data;
using AcademicArticleApi.Models;
using System;
using System.Linq;

namespace AcademicArticleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AcademicArticlesController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var articles = _context.AcademicArticles.ToList();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var article = _context.AcademicArticles.Find(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] AcademicArticle article)
        {
            try
            {
                _context.AcademicArticles.Add(article);
                _context.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = article.Id }, article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AcademicArticle article)
        {
            try
            {
                var existing = _context.AcademicArticles.Find(id);
                if (existing == null) return NotFound();
                existing.Title = article.Title;
                existing.Author = article.Author;
                existing.Year = article.Year;
                existing.Abstract = article.Abstract;
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var article = _context.AcademicArticles.Find(id);
                if (article == null) return NotFound();
                _context.AcademicArticles.Remove(article);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
