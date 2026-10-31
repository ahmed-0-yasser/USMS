
using USMS.Core.DTOs.ServiceRequests;

namespace USMS.Core.Interfaces
{
    public interface IServiceRequestService
    {
        // Student Operations

        Task<ServiceRequestDetailsDto> CreateRequestAsync(
            CreateServiceRequestDto dto,
            int studentId);

        Task<IEnumerable<ServiceRequestResponseDto>> GetMyRequestsAsync(
            int studentId);

        Task<ServiceRequestDetailsDto?> GetRequestDetailsAsync(
            int requestId,
            int studentId);

        Task<bool> CancelRequestAsync(
            int requestId,
            int studentId);


        // Employee Operations

        Task<IEnumerable<ServiceRequestResponseDto>> GetDepartmentRequestsAsync(
            int departmentId);

        Task<bool> UpdateRequestStatusAsync(
            int requestId,
            int employeeId,
            int departmentId,
            UpdateServiceRequestStatusDto dto);
    }
}
