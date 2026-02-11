using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly DepartmentService _departmentService;
        private readonly IMapper _mapper;

        public DepartmentsController(DepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Doctor,Patient,Admin,Nurse")]
        public async Task<IActionResult> Index()
        {
            var dtos = await _departmentService.GetAllDepartmentsAsync();
            var viewModels = _mapper.Map<IEnumerable<DepartmentViewModel>>(dtos);
            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<DepartmentDto>(model);

            try
            {
                await _departmentService.AddDepartmentAsync(dto);
                TempData["ToastMessage"] = "Department created successfully.";
                TempData["ToastType"] = "success";
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "Failed to create department.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _departmentService.GetDepartmentByIdAsync(id);
            if (dto == null)
            {
                TempData["ToastMessage"] = "Department not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<DepartmentViewModel>(dto);
            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }

                TempData["ToastMessage"] = "Please correct the form errors.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var dto = _mapper.Map<DepartmentDto>(model);

            try
            {
                await _departmentService.UpdateDepartmentAsync(dto);
                TempData["ToastMessage"] = "Department updated successfully.";
                TempData["ToastType"] = "success";
                return Json(new { success = true }); // AJAX success signal
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "Failed to update department.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }

                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
                TempData["ToastMessage"] = "Department deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "Failed to delete department.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
