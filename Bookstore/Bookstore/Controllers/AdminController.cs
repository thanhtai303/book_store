using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Controllers;

public class AdminViewController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult UserManagement()
    {
        return View();
    }

    public IActionResult OrderManagement()
    {
        return View();
    }
    
    public IActionResult BookListManagement()
    {
        return View();
    }
    
    public IActionResult CategoryListManagement()
    {
        return View();
    }
    
    public IActionResult BookAdd()
    {
        return View();
    }
}