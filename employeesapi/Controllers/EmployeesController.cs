using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using employeesapi.Models;

namespace employeesapi.Controllers 
{

  //this controller using Attribute routing instead of Conventional routing method
  //e.g. localhost:9001/Employees/
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class EmployeesController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    

    public EmployeesController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: api/Employees e.g. localhost:9001/Employees
    [HttpGet]
    //[Authorize]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
      return await _context.Employees.ToListAsync();
    }

    // GET: api/Employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(string id)
    {
      var employee = await _context.Employees.FindAsync(id);

      if (employee == null)
      {
        return NotFound();
      }

      return employee;
    }

    // POST: api/Employees
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
      if(string.IsNullOrEmpty(employee.Id))
      {
        _context.Employees.Add(employee);
      }
      else {
        _context.Employees.Update(employee);
      }
        await _context.SaveChangesAsync();

      return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
    }
    //POST: api/Employees/5
    [HttpPost("{id}")]
    public async Task<ActionResult<Employee>> EditEmployee(string id, Employee employee)
    {
      _context.Employees.Update(employee);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
    }

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Employee>> DeleteEmployee(string id)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound();
      }

      _context.Employees.Remove(employee);
      await _context.SaveChangesAsync();

      return employee;
    }

    private bool EmployeeExists(string id)
    {
       return _context.Employees.Any(e => e.Id == id);
    }
  }

}