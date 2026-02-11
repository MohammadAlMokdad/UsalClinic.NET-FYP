using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Core;
using UsalClinic.Core.Entities;

namespace UsalClinic.Application.Services
{
    public class PatientRequestService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientRequestService> _logger;

        public PatientRequestService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientRequestService> logger,
            UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PatientRequestDto>> GetAllRequestsAsync()
        {
            var entities = await _unitOfWork.PatientRequests.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientRequestDto>>(entities);
        }

        public async Task<PatientRequestDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.PatientRequests.GetByIdAsync(id);
            return _mapper.Map<PatientRequestDto>(entity);
        }

        public async Task AddRequestAsync(PatientRequestDto dto)
        {
            var entity = _mapper.Map<PatientRequest>(dto);
            await _unitOfWork.PatientRequests.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRequestAsync(int id)
        {
            var entity = await _unitOfWork.PatientRequests.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.PatientRequests.DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<PatientRequestDto>> GetPendingRequestsAsync()
        {
            _logger.LogInformation("Fetching pending patient requests.");
            var requests = await _unitOfWork.PatientRequests.GetPendingRequestsAsync();
            return _mapper.Map<IEnumerable<PatientRequestDto>>(requests);
        }

        public async Task ApproveAsync(int id)
        {
            var request = await _unitOfWork.PatientRequests.GetByIdAsync(id);
            if (request == null)
            {
                _logger.LogWarning("Patient request with ID {Id} not found.", id);
                throw new Exception("Request not found.");
            }

            if (request.IsApproved)
            {
                _logger.LogInformation("Patient request with ID {Id} is already approved.", id);
                return;
            }

            // Mark request as approved
            request.IsApproved = true;

            // Generate normalized email from full name
            var normalized = request.FullName.Replace(" ", "").ToLower();
            var generatedEmail = $"{normalized}@clinic.com";

            var user = new ApplicationUser
            {
                UserName = generatedEmail,
                Email = generatedEmail,
                FullName = request.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "U@u123456"); // Or generate a random password

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create ApplicationUser for request ID {Id}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("Failed to create user.");
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            // Create Patient entity
            var patient = new Patient
            {
                UserId = user.Id,
                Gender = request.Gender,
                Address = request.Address,
                Major = request.Major,
                BloodType = request.BloodType,
                DateOfBirth = request.DateOfBirth
            };

            await _unitOfWork.Patients.AddAsync(patient);

            // Save everything
            await _unitOfWork.PatientRequests.UpdateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            // Send email
            string defaultPassword = "U@u123456"; // Or your chosen default
            string subject = "Your USAL Clinic Account Has Been Approved";
            string body = $"Dear {request.FullName},\n\n" +
                          $"Your patient request has been approved. You can now log in using the username: {generatedEmail} and the default password: {defaultPassword}\n\n" +
                          "Please change your password after first login.";

            await _emailService.SendEmailAsync(generatedEmail, subject, body);


            _logger.LogInformation("Patient request with ID {Id} approved and account created.", id);
        }


        public async Task CreateRequestAsync(PatientRequestDto dto)
        {
            var entity = _mapper.Map<PatientRequest>(dto);
            await _unitOfWork.PatientRequests.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New patient request created for {FullName}.", dto.FullName);
        }

        public async Task RejectAsync(int id)
        {
            var request = await _unitOfWork.PatientRequests.GetByIdAsync(id);
            if (request == null)
            {
                _logger.LogWarning("Patient request with ID {Id} not found.", id);
                throw new Exception("Request not found.");
            }

            if (request.IsApproved)
            {
                _logger.LogInformation("Cannot reject an already approved request with ID {Id}.", id);
                throw new Exception("Cannot reject approved request.");
            }
           
            request.IsRejected = true;
            request.IsApproved = false;

            await _unitOfWork.PatientRequests.UpdateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            // Optionally send rejection email
            string subject = "Your USAL Clinic Account Request Has Been Rejected";
            string body = $"Dear {request.FullName},\n\nWe regret to inform you that your patient request has been rejected.\n\nFor further inquiries, please contact support.";
            await _emailService.SendEmailAsync(request.UserName, subject, body);

            _logger.LogInformation("Patient request with ID {Id} rejected.", id);
        }

    }
}
