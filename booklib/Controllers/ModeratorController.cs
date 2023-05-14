using AutoMapper;
using booklib.Entities;
using booklib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace booklib.Controllers
{
    public class ModeratorController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ModeratorController(DatabaseContext databaseContext, IConfiguration configuration, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(BookModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if(_databaseContext.Books.Any(x => x.BookName.ToLower() == model.BookName.ToLower() && x.Author.ToLower() == model.Author.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.BookName), "Kitap mevcut, Stok arttırımı yapabilirsiniz.");
                    return View(model);
                }
                Book book = new Book()
                {                    
                    BookName = model.BookName,
                    Author = model.Author,
                    BookType = model.BookType,
                    BookImageFileName = model.BookImageFileName,
                    BookSubject = model.BookSubject,
                    PublishingDate = model.PublishingDate,
                    Stock = model.Stock

                };
                       
                string fileName = $"p_{book.BookName}{book.Author}.jpg";

                Stream stream = new FileStream($"wwwroot/uploads/{fileName}", FileMode.OpenOrCreate);

                file.CopyTo(stream);
                stream.Close();
                stream.Dispose();
                book.BookImageFileName = fileName;

                _databaseContext.Books.Add(book);              
                int affectedRowCount = _databaseContext.SaveChanges();
                ViewData["Result"] = "Ok";

                

                if (affectedRowCount == 0)
                {
                    ModelState.AddModelError("", "Kitap eklenememiştir.");
                }
                else
                {
                    
                    return RedirectToAction(nameof(Index));
                }

                

            }
            return View("Index");
        }

        public IActionResult EditBook(Guid id)
            {
                Book book = _databaseContext.Books.Find(id);
                BookEditModel model = _mapper.Map<BookEditModel>(book);
                return View(model);
            }

        [HttpPost]
        public IActionResult EditBook(Guid id, BookModel model)
        {
            if (ModelState.IsValid)
            {
                Book book = _databaseContext.Books.Find(id);
                model.BookImageFileName = book.BookImageFileName;
                model.PublishingDate = book.PublishingDate;
                _mapper.Map(model, book);
                _databaseContext.SaveChanges();
                return RedirectToAction(nameof(BookList));
            }
            return View(model);
        }

        public IActionResult BookList(BookModel model)
        {            
                List<BookModel> books =
                    _databaseContext.Books.ToList().Select(x => _mapper.Map<BookModel>(x)).ToList();                    

                return View(books);
        }

        public IActionResult DeleteBook(Guid id)
        {
            Book book = _databaseContext.Books.Find(id);

            if(book != null)
            {
                _databaseContext.Books.Remove(book);
                _databaseContext.SaveChanges();
            }

            return RedirectToAction(nameof(BookList));
        }     
    }
}
