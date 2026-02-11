using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Core;
using UsalClinic.Core.Entities;

namespace UsalClinic.Application.Services
{
    public class ShiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<ShiftService> _logger;

        public ShiftService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShiftService> logger, IEmailService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService;
        }

        public async Task<List<ShiftDto>> GetAllShiftsAsync()
        {
            _logger.LogInformation("Retrieving all shifts.");
            var shifts = await _unitOfWork.Shifts.GetAllAsync();
            return _mapper.Map<List<ShiftDto>>(shifts);
        }

        public async Task<ShiftDto?> GetShiftByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving shift with ID {ShiftId}.", id);
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                _logger.LogWarning("Shift with ID {ShiftId} not found.", id);
                return null;
            }
            return _mapper.Map<ShiftDto>(shift);
        }

        public async Task<ShiftDto> CreateShiftAsync(ShiftDto shiftDto)
        {
            var shift = _mapper.Map<Shift>(shiftDto);
            _logger.LogInformation("Creating new shift.");
            try
            {
                await _unitOfWork.Shifts.AddAsync(shift);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Shift created successfully with ID {ShiftId}.", shift.Id);
                return _mapper.Map<ShiftDto>(shift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating shift.");
                throw;
            }
        }

        public async Task<ShiftDto?> UpdateShiftAsync(int id, ShiftDto shiftDto)
        {
            _logger.LogInformation("Updating shift with ID {ShiftId}.", id);
            var existingShift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (existingShift == null)
            {
                _logger.LogWarning("Shift with ID {ShiftId} not found for update.", id);
                return null;
            }
            _mapper.Map(shiftDto, existingShift);
            await _unitOfWork.Shifts.UpdateAsync(existingShift);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Shift with ID {ShiftId} updated successfully.", id);
            return _mapper.Map<ShiftDto>(existingShift);
        }

        public async Task<bool> DeleteShiftAsync(int id)
        {
            _logger.LogInformation("Attempting to delete shift with ID {ShiftId}.", id);
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                _logger.LogWarning("Shift with ID {ShiftId} not found for deletion.", id);
                return false;
            }
            await _unitOfWork.Shifts.DeleteAsync(shift);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Shift with ID {ShiftId} deleted successfully.", id);
            return true;
        }

        public async Task<List<ShiftDto>> GetShiftsByStaffIdAsync(string staffId)
        {
            _logger.LogInformation("Retrieving shifts for staff ID {StaffId}.", staffId);
            var shifts = await _unitOfWork.Shifts.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<ShiftDto>>(shifts);
        }

        public async Task<List<ShiftDto>> GetShiftsByRoleAsync(string role)
        {
            _logger.LogInformation("Retrieving shifts for role {Role}.", role);
            var shifts = await _unitOfWork.Shifts.GetByRoleAsync(role);
            return _mapper.Map<List<ShiftDto>>(shifts);
        }
        public async Task<bool> SendUrgentAlertAsync(string senderEmail, int departmentId, int roomId)
        {
            var currentTime = DateTime.Now;
            var currentDay = currentTime.DayOfWeek;
            var currentTimeSpan = currentTime.TimeOfDay;

            // Get all shifts for nurses
            var nurseShifts = await _unitOfWork.Shifts.GetActiveNurseShiftsAsync(currentDay, currentTimeSpan);


            var availableNurse = nurseShifts
                .OrderBy(s => s.StartTime)
                .Select(s => s.Staff)
                .FirstOrDefault();

            if (availableNurse == null)
                return false;

            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);

            var message = $"🚨 Urgent Emergency!\n\nDepartment: {department?.Name}\nRoom: {room?.RoomNumber}\nReported by: {senderEmail}";

            await _emailService.SendEmailAsync(
                availableNurse.Email,
                "Emergency Alert",
                message
            );

            return true;
        }


    }
}
