using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Linq.Dynamic;

namespace WebApplication1.Controllers {
    public class SqlController : Controller {
        public SqlController() {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-NZ");
        }

        // GET: WebGrid?page=1&rowsPerPage=10&sort=OrderID&sortDir=ASC
        public ActionResult WebGrid(int page = 1, int rowsPerPage = 10, string sortCol = "OrderID", string sortDir = "ASC") {
            List<Model> res;
            if (sortCol == "CompanyName" | sortCol == "ContactName") { sortCol = "Customer." + sortCol;}
            if (sortCol == "EmpFirstName" | sortCol == "EmpLastName") { sortCol = "Employee." + sortCol.Substring(3); }
            int count;
            string sql;

            using (var nwd = new NorthwindEntities()) {
                var _res = nwd.Orders
                    .OrderBy(sortCol + " " + sortDir + ", OrderID " + sortDir )
                    .Skip((page - 1) * rowsPerPage)
                    .Take(rowsPerPage)
                    .Select(o => new Model {
                        OrderID = o.OrderID,
                        OrderDate = o.OrderDate,
                        Freight = "$" + o.Freight,
                        ShipCity = o.ShipCity,
                        ShipCountry = o.ShipCountry,
                        CompanyName = o.Customer.CompanyName,
                        ContactName = o.Customer.ContactName,
                        EmpFirstName = o.Employee.FirstName,
                        EmpLastName = o.Employee.LastName
                    });

                res = _res.ToList();
                count = nwd.Orders.Count();
                sql = _res.ToString();
            }
            
            ViewBag.sortCol = sortCol;
            ViewBag.sortDir = sortDir;
            ViewBag.rowsPerPage = rowsPerPage;
            ViewBag.count = count;
            ViewBag.sql = sql;
            return View(res);
        }
    }

    public class Model {
        [Key]
        public int OrderID { get; set; }
        public System.DateTime? OrderDate { get; set; }
        public string Freight { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
    }
}