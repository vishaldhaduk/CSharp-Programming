using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCApplication.DataAccessLayer;

namespace MVCApplication.Models
{
    public class EmployeeBusinessLayer
    {
        public List<Employee> GetEmployees()
        {
            SalesERPDAL salesDal = new SalesERPDAL();
            return salesDal.Employees.ToList();

            List<Employee> employees = new List<Employee>();
            Employee emp = new Employee();
            emp.FirstName = "johnson";
            emp.LastName = " fernandes";
            emp.Salary = 14000;
            employees.Add(emp);

            return employees;
        }

        public Employee SaveEmployee(Employee e)
        {
            SalesERPDAL s = new SalesERPDAL();
            s.Employees.Add(e);
            s.SaveChanges();
            return e;
        }
    }
}