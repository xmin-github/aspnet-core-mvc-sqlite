using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;

using dotnetcoremvcclient.Models;

namespace dotnetcoremvcclient.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ITokenService _tokenService;
        // public EmployeesController(ApplicationDbContext context)
        // {
        //     _context = context;

        // }
        public EmployeesController(ITokenService tokenService)
        {
		      _tokenService = tokenService;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Employees.ToListAsync());
            return View(await  GetEmployees());
        
        }

        
        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var employee = await _context.Employees
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var employee = await GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age")] Employee employee)
        {
            // if (ModelState.IsValid)
            // {
            //     _context.Add(employee);
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }
            // return View(employee);

            if (ModelState.IsValid)
            {
                
                await CreateEmployee(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        
        }
        

        //GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var employee = await _context.Employees.FindAsync(id);
            var employee = await GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        
        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Age")] Employee employee)
        {
            Console.Write (id + "\n");
            Console.Write ("emp:" + employee.Id);
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // try
                // {
                //     _context.Update(employee);
                //     await _context.SaveChangesAsync();
                // }
                // catch (DbUpdateConcurrencyException)
                // {
                //     if (!EmployeeExists(employee.Id))
                //     {
                //         return NotFound();
                //     }
                //     else
                //     {
                //         throw;
                //     }
                // }
                await EditEmployee(id, employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var employee = await _context.Employees
            //     .FirstOrDefaultAsync(m => m.Id == id);
            var employee = await GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //var employee = await _context.Employees.FindAsync(id);
            //_context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();

            await DeleteEmployee(id);
            return RedirectToAction(nameof(Index));
        }
        
        private async Task<List<Employee>> GetEmployees()
        {
          string apiBaseUri = _tokenService.ApiBaseUri;
	        var token = await _tokenService.GetToken();
          _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
	        var res = await _httpClient.GetAsync(apiBaseUri + "/api/employees");
	        if (res.IsSuccessStatusCode)
	        {
		        var content = await res.Content.ReadAsStringAsync();
		        var employees = JsonConvert.DeserializeObject<List<Employee>>(content);
		        return employees;
	        }

	        return null;
        }

        private async Task<Employee> GetEmployee(string id)
        {
          string apiBaseUri = _tokenService.ApiBaseUri;
	        var token = await _tokenService.GetToken();
          _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
	        var res = await _httpClient.GetAsync(apiBaseUri + "/api/employees/" + id);
	        if (res.IsSuccessStatusCode)
	        {
		        var content = await res.Content.ReadAsStringAsync();
		        var employee = JsonConvert.DeserializeObject<Employee>(content);
		        return employee;
	        }

	        return null;
        }

        private async Task<string> DeleteEmployee(string id)
        {
            string apiBaseUri = _tokenService.ApiBaseUri;
	          var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
	          var deleteTask = _httpClient.DeleteAsync(apiBaseUri + "/api/employees/" + id);
            deleteTask.Wait();
            var result = deleteTask.Result;
            return result.StatusCode.ToString();

        }

        private async Task<Employee> CreateEmployee(Employee employee)
        {
            string apiBaseUri = _tokenService.ApiBaseUri;
	          var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
	          var postTask = _httpClient.PostAsJsonAsync<Employee>(apiBaseUri + "/api/employees",employee);
            postTask.Wait();

            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {

                var readTask = result.Content.ReadAsAsync<Employee>();
                readTask.Wait();

                var insertedEmployee = readTask.Result;

                return insertedEmployee;
            }
            else
            {
                return null;
            }

        }
        
        private async Task<Employee> EditEmployee(string id, Employee employee)
        {
            string apiBaseUri = _tokenService.ApiBaseUri;
	          var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            Console.Write("\ncall edit employee api");
	          var postTask = _httpClient.PostAsJsonAsync<Employee>(apiBaseUri + "/api/employees/" + id, employee);
            postTask.Wait();

            var result = postTask.Result;
            Console.Write("\n" + result.StatusCode.ToString()+ "\n");
            if (result.IsSuccessStatusCode)
            {
                
                var readTask = result.Content.ReadAsAsync<Employee>();
                readTask.Wait();

                var editedEmployee = readTask.Result;

                return editedEmployee;
            }
            else
            {
                return null;
            }

        }
        // private bool EmployeeExists(string id)
        // {
        //     return _context.Employees.Any(e => e.Id == id);

        // }
    }
}
