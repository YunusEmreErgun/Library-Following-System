using AutoMapper;
using booklib.Entities;
using booklib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;

namespace booklib.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

               public HomeController(DatabaseContext databaseContext, IConfiguration configuration, IMapper mapper)
                {
                    _databaseContext = databaseContext;
                    _configuration = configuration;
                    _mapper = mapper;
                }

        public async Task<IActionResult> Index(string SearchString)
        {
            List<BookSearchModel> EmptyBooks = new List<BookSearchModel>();
            if (!string.IsNullOrEmpty(SearchString)) 
            {                 
                            try { 
                            var books = _databaseContext.Books.ToList()
                                .Select(x => _mapper.Map<BookSearchModel>(x)).Where(x => x.BookName.ToLower().Contains(SearchString.ToLower())).ToList();
                                return View(books);
                                

                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
            }
            

            return View(EmptyBooks);          
        }
        public IActionResult BorrowBook(Guid id)
        {
            Book book = _databaseContext.Books.Find(id);
            ClaimsPrincipal principal = HttpContext.User;
            Guid userId = new Guid(principal.FindFirst(ClaimTypes.NameIdentifier).Value);

            User user = _databaseContext.Users.Find(userId);


            Lib entity = new Lib();
            entity.Book = book;
            entity.User = user;
            entity.UserName = user.UserName;
            entity.BookName = book.BookName;
            entity.Book.Stock = book.Stock - 1;



            //List<LibViewModel> libs =
            //    _databaseContext.Libs.ToList().Select(x => _mapper.Map<LibViewModel>(x)).xToList();

            //var username = entity.User.UserName;
            //var bookname = entity.Book.BookName;

            _databaseContext.Add(entity);


            _databaseContext.SaveChanges();


            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "user, admin, moderator")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DetailsList(BookModel model)
        {

            List<BookModel> books=_databaseContext.Books.ToList().Select(x=>_mapper.Map<BookModel>(x)).ToList();

            return View(books);

        }
        
        public IActionResult BookDetails(Guid id, BookModel model)
        {            
            Book book = _databaseContext.Books.Find(id);
            //model.BookImageFileName = book.BookImageFileName;
            _mapper.Map(book, model);
            return View(model);
        } 
    }
}