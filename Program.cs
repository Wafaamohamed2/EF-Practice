using Microsoft.EntityFrameworkCore;
using NewEF;
using System.Net.WebSockets;

internal class Program
{
    private static void Main(string[] args)
    {
        var _context = new ApplicationDbContext();


        var stocks = GetData(1, 10);
        foreach (var stock in stocks)
            Console.WriteLine(stock.id);


        Console.WriteLine("----------------Group by method--------------");

        var stock2 = _context.Stock.GroupBy(m => m.gender)
            .Select(m => new { Gender = m.Key, Count = m.Count() })
            .OrderByDescending(m => m.Count);

        foreach (var stock in stock2)
            Console.WriteLine(stock.Gender + "-- " + stock.Count);


        Console.WriteLine("--------------Joins-------------------");

        // Inner Join

        var books = _context.Books
            .Join(
              _context.Authors,
              book => book.AuthorId,
              author => author.AuthorId,
              (book, author) => new
              {
                  BookId = book.Id,
                  BookName = book.Name,
                  AuthorName = author.Name,
                  AuthorNationalityId = author.NationalityId

              }


            ).Join(
                 _context.Nationalities,
                 book => book.AuthorNationalityId,
                 nationnality => nationnality.NationalityId,

                   (book, nationality) => new
                   {
                      book.BookId,
                      book.BookName,
                      book.AuthorName,
                      AuthorNationality = nationality.Name

                   }


            );

        foreach (var book in books)
            Console.WriteLine($"{book.BookId} -- {book.BookName} -- {book.AuthorName} -- {book.AuthorNationality}");



        Console.WriteLine("------------------Tracking VS NoTracking-----------------");

        //the value  of the price of the book with id 1 will change becous using of SaveChanges (Tracking)

        var bookT = _context.Books.SingleOrDefault(b => b.Id == 1);
        bookT.Price = 150;
        _context.SaveChanges();


        var bookNT = _context.Books.AsNoTracking().SingleOrDefault(b => b.Id == 1);
        bookNT.Price = 110;
        _context.SaveChanges();
    }

    public static List<Stock> GetData(int pagnum , int pagsize)
    {

        var _context = new ApplicationDbContext();

        return _context.Stock.Skip((pagnum-1)*pagsize).Take(pagsize).ToList();

    }
}