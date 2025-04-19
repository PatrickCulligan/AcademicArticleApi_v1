using Xunit;
using Microsoft.AspNetCore.Mvc;
using AcademicArticleApi.Models;
using AcademicArticleApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

// Fix: Ensure the correct namespace for the controller is used.  
// If the 'Controllers' namespace is missing, verify the project structure and add the correct namespace.  
using AcademicArticleApi.Controllers;

namespace AcademicArticleApi.Tests
{
    /// <summary>
    /// Unit tests for AcademicArticlesController covering CRUD operations.
    /// </summary>
    public class AcademicArticlesControllerCrudTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        /// <summary>
        /// Constructor sets up an in-memory EF Core database for isolation.
        /// </summary>
        public AcademicArticlesControllerCrudTests()
        {
            // Configure EF Core to use an in-memory database named "CrudTestDb".
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CrudTestDb")
                .Options;
        }

        [Fact]
        public void Post_CreatesArticle_ReturnsCreatedAtAction()
        {
            // Arrange: create context and controller, prepare a new article
            using var context = new AppDbContext(_options);
            var controller = new AcademicArticlesController(context);
            var newArticle = new AcademicArticle
            {
                Title = "New Article",
                Author = "Author X",
                Year = 2025,
                Abstract = "Sample abstract"
            };

            // Act: call the Post endpoint
            IActionResult actionResult = controller.Post(newArticle);

            // Assert: ensure we got a CreatedAtActionResult
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult);

            // Extract the returned article from the result
            var createdArticle = Assert.IsType<AcademicArticle>(createdAtResult.Value);

            // Verify the article fields and that an Id was assigned
            Assert.Equal("New Article", createdArticle.Title);
            Assert.True(createdArticle.Id > 0, "Expected database to assign a positive Id.");
        }

        [Fact]
        public void Put_ExistingArticle_UpdatesArticle()
        {
            // Arrange: seed the in-memory database with one article
            using var context = new AppDbContext(_options);
            var original = new AcademicArticle
            {
                Title = "Before",
                Author = "A",
                Year = 2020,
                Abstract = "Old"
            };
            context.AcademicArticles.Add(original);
            context.SaveChanges();

            // Prepare updated values
            var updated = new AcademicArticle
            {
                Id = original.Id,
                Title = "After",
                Author = "B",
                Year = 2021,
                Abstract = "New"
            };
            var controller = new AcademicArticlesController(context);

            // Act: call the Put endpoint
            IActionResult result = controller.Put(original.Id, updated);

            // Assert: should return NoContentResult on success
            Assert.IsType<NoContentResult>(result);

            // Verify that the database article was updated
            var dbArticle = context.AcademicArticles.Find(original.Id);
            Assert.Equal("After", dbArticle.Title);
            Assert.Equal("B", dbArticle.Author);
            Assert.Equal(2021, dbArticle.Year);
            Assert.Equal("New", dbArticle.Abstract);
        }

        [Fact]
        public void Delete_ExistingArticle_RemovesArticle()
        {
            // Arrange: add an article to be deleted
            using var context = new AppDbContext(_options);
            var toDelete = new AcademicArticle
            {
                Title = "Temp",
                Author = "X",
                Year = 2022,
                Abstract = "Temp Abstract"
            };
            context.AcademicArticles.Add(toDelete);
            context.SaveChanges();
            var controller = new AcademicArticlesController(context);

            // Act: call the Delete endpoint
            IActionResult result = controller.Delete(toDelete.Id);

            // Assert: ensure NoContentResult and that the article no longer exists
            Assert.IsType<NoContentResult>(result);
            Assert.Null(context.AcademicArticles.Find(toDelete.Id));
        }

        [Fact]
        public void Delete_NonExisting_ReturnsNotFound()
        {
            // Arrange: no articles in database
            using var context = new AppDbContext(_options);
            var controller = new AcademicArticlesController(context);

            // Act: attempt to delete an article with an ID that doesn't exist
            IActionResult result = controller.Delete(999);

            // Assert: should return NotFoundResult
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
