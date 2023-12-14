﻿using EmployeeWeb.Models;
using EmployeeWeb.Proxy;
using Microsoft.AspNetCore.Mvc;
using System.Fabric;

namespace EmployeeWeb.Controllers
{
    /// <summary>
    /// Employee Management Controller
    /// </summary>
    public class HomeController : Controller
    {
        private ServiceContext _serviceContext = null;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;
        private static long EmployeeId = 0;

        public HomeController(HttpClient httpClient, StatelessServiceContext context, FabricClient fabricClient)
        {
            this._fabricClient = fabricClient;
            this._httpClient = httpClient;
            this._serviceContext = context;
        }

        /// <summary>
        /// Loads the employee list
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            EmployeeDataAPIProxy employeeProxy = new(this._serviceContext, this._httpClient, this._fabricClient);

            List<Employee> employees = await employeeProxy.GetEmployees();

            EmployeeViewModel viewModel = new()
            {
                EmployeeList = employees
            };

            return View(viewModel);
        }

        /// <summary>
        /// Responsible for creating Employee
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel)
        {
            EmployeeDataAPIProxy employeeProxy = new(this._serviceContext, this._httpClient, this._fabricClient); ;

            if (ModelState.IsValid)
            {
                //Not for production at all.
                EmployeeId++;

                employeeViewModel.Employee.Id = EmployeeId;

                await employeeProxy.CreateEmployee(employeeViewModel.Employee);

            }

            List<Employee> employees = await employeeProxy.GetEmployees();

            employeeViewModel.EmployeeList = employees;

            return View("Index",employeeViewModel);
        }


        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Delete(long? id)
        {
            EmployeeViewModel viewModel = new();

            EmployeeDataAPIProxy employeeProxy = new(this._serviceContext, this._httpClient, this._fabricClient);

            if (id != null)
            {
                await employeeProxy.DeleteEmployee(id.Value);
            }

            List<Employee> employees = await employeeProxy.GetEmployees();

            viewModel.EmployeeList = employees;

            return View("Index", viewModel);
        }

    }
}
