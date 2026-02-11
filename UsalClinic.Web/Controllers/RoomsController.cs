using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Web.ViewModels;

[Authorize]
public class RoomsController : Controller
{
    private readonly RoomService _roomService;
    private readonly DepartmentService _departmentService;
    private readonly IMapper _mapper;

    public RoomsController(RoomService roomService, DepartmentService departmentService, IMapper mapper)
    {
        _roomService = roomService;
        _departmentService = departmentService;
        _mapper = mapper;
    }

    [Authorize(Roles = "Doctor,Patient,Admin,Nurse")]
    public async Task<IActionResult> Index()
    {
        var dto = await _roomService.GetAllRoomsAsync();
        var viewModel = _mapper.Map<List<RoomViewModel>>(dto);
        return View(viewModel);
    }

    [Authorize(Roles = "Doctor,Patient,Admin,Nurse")]
    public async Task<IActionResult> ByDepartment(int id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
            return NotFound();

        var rooms = await _roomService.GetRoomsByDepartmentAsync(id);
        var roomViewModels = _mapper.Map<List<RoomViewModel>>(rooms);

        ViewData["DepartmentName"] = department.Name;
        return View("RoomsByDepartment", roomViewModels);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await PopulateDepartmentsDropDown();
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "Validation failed. Please check the form.";
            TempData["ToastType"] = "error";
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<RoomDto>(vm);
            dto.CreatedAt = DateTime.UtcNow;

            await _roomService.CreateRoomAsync(dto);

            TempData["ToastMessage"] = "Room created successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "An error occurred while creating the room.";
            TempData["ToastType"] = "error";
            return View(vm);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _roomService.GetRoomByIdAsync(id);
        if (dto == null)
            return NotFound();

        var vm = _mapper.Map<RoomViewModel>(dto);
        await PopulateDepartmentsDropDown();
        return PartialView("_Edit", vm);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditDepartment(int id)
    {
        var dto = await _roomService.GetRoomByIdAsync(id);
        if (dto == null)
            return NotFound();

        var vm = _mapper.Map<RoomViewModel>(dto);
        await PopulateDepartmentsDropDown();
        return PartialView("_EditDepartment", vm);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoomViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "Validation failed while updating the room.";
            TempData["ToastType"] = "error";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Edit", vm);

            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<RoomDto>(vm);
            await _roomService.UpdateRoomAsync(dto);

            TempData["ToastMessage"] = "Room updated successfully.";
            TempData["ToastType"] = "success";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "An error occurred while updating the room.";
            TempData["ToastType"] = "error";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_Edit", vm);

            return View(vm);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDepartment(RoomViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "Validation failed while updating the room.";
            TempData["ToastType"] = "error";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditDepartment", vm);

            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<RoomDto>(vm);
            await _roomService.UpdateRoomAsync(dto);

            TempData["ToastMessage"] = "Room updated successfully.";
            TempData["ToastType"] = "success";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(ByDepartment), new { id = vm.DepartmentId });
        }
        catch
        {
            await PopulateDepartmentsDropDown();
            TempData["ToastMessage"] = "An error occurred while updating the room.";
            TempData["ToastType"] = "error";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditDepartment", vm);

            return View(vm);
        }
    }

    private async Task PopulateDepartmentsDropDown()
    {
        var depts = await _departmentService.GetAllDepartmentsAsync();
        ViewBag.Departments = depts
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            .ToList();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _roomService.DeleteRoomAsync(id);

            TempData["ToastMessage"] = "Room deleted successfully.";
            TempData["ToastType"] = "success";
        }
        catch
        {
            TempData["ToastMessage"] = "Failed to delete room.";
            TempData["ToastType"] = "error";
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound();

        int departmentId = room.DepartmentId;

        try
        {
            await _roomService.DeleteRoomAsync(id);

            TempData["ToastMessage"] = "Room deleted successfully.";
            TempData["ToastType"] = "success";
        }
        catch
        {
            TempData["ToastMessage"] = "Failed to delete room.";
            TempData["ToastType"] = "error";
        }

        return RedirectToAction(nameof(ByDepartment), new { id = departmentId });
    }

    [HttpGet]
    public async Task<IActionResult> GetRoomsByDepartment(int departmentId)
    {
        var rooms = await _roomService.GetRoomsByDepartmentAsync(departmentId);

        var roomOptions = rooms.Select(r => new
        {
            id = r.Id,
            name = r.RoomNumber.ToString()
        });

        return Json(roomOptions);
    }
}
