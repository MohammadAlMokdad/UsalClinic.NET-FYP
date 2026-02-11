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
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IEmailService _emailService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AppointmentService> logger, IEmailService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            _logger.LogInformation("Retrieving all appointments.");
            var appointments = await _unitOfWork.Appointments.GetAllAppointmentsAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving appointment with ID {AppointmentId}", id);
            var appointment = await _unitOfWork.Appointments.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found.", id);
                return null;
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task AddAppointmentAsync(AppointmentDto dto)
        {
            _logger.LogInformation("Adding a new appointment for PatientId {PatientId}, DoctorId {DoctorId}.", dto.PatientId, dto.DoctorId);

            try
            {
                var appointment = _mapper.Map<Appointment>(dto);
                appointment.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Appointments.AddAsync(appointment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Appointment added successfully.");

                // Get patient info
                var patient = await _unitOfWork.Patients.GetPatientByIdAsync(dto.PatientId);

                // Get doctor info
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(dto.DoctorId);

                if (patient != null && doctor != null)
                {
                    var patientEmail = patient.User?.Email;
                    var doctorName = doctor.User?.FullName ?? "Your Doctor";
                    var appointmentDate = dto.AppointmentDate.ToString("f"); // Full date/time format

                    if (!string.IsNullOrWhiteSpace(patientEmail))
                    {
                        var subject = "Appointment Confirmation - USAL Clinic";
                        var body = $"Hello {patient.User.FullName},\n\n" +
                                   $"Your appointment has been scheduled with Dr. {doctorName}.\n" +
                                   $"📅 Date & Time: {appointmentDate}\n" +
                                   $"📝 Status: {dto.Status}\n" +
                                   $"📌 Notes: {dto.Notes}\n\n" +
                                   $"Thank you,\nUSAL Clinic";

                        await _emailService.SendEmailAsync(patientEmail, subject, body);
                        _logger.LogInformation("Confirmation email sent to patient at {Email}.", patientEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding appointment.");
                throw;
            }
        }



        public async Task UpdateAppointmentAsync(AppointmentDto dto)
        {
            _logger.LogInformation("Updating appointment with ID {AppointmentId}.", dto.Id);
            var existing = await _unitOfWork.Appointments.GetAppointmentByIdAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Attempted to update non-existent appointment with ID {AppointmentId}.", dto.Id);
                return;
            }

            _mapper.Map(dto, existing);
            await _unitOfWork.Appointments.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appointment with ID {AppointmentId} updated successfully.", dto.Id);
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            _logger.LogInformation("Deleting appointment with ID {AppointmentId}.", id);
            try
            {
                await _unitOfWork.Appointments.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Appointment with ID {AppointmentId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting appointment with ID {AppointmentId}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            _logger.LogInformation("Retrieving appointments for DoctorId {DoctorId}.", doctorId);
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            _logger.LogInformation("Retrieving appointments for PatientId {PatientId}.", patientId);
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}
