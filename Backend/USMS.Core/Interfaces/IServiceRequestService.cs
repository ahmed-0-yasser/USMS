
using USMS.Core.DTOs.ServiceRequests;

namespace USMS.Core.Interfaces
{
    /// <summary>
    /// Defines the operations available for managing student service requests.
    /// </summary>
    public interface IServiceRequestService
    {
        /// <summary>
        /// Creates a new service request for a student.
        /// </summary>
        /// <param name="dto">The data required to create the service request.</param>
        /// <param name="studentId">The ID of the student creating the request.</param>
        /// <returns>The details of the newly created service request.</returns>
        Task<ServiceRequestDetailsDto> CreateRequestAsync(
            CreateServiceRequestDto dto,
            int studentId);

        /// <summary>
        /// Retrieves all service requests belonging to a specific student.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <returns>A collection of service request summaries.</returns>
        Task<IEnumerable<ServiceRequestResponseDto>> GetMyRequestsAsync(
            int studentId);

        /// <summary>
        /// Retrieves the details of a specific request belonging to a student.
        /// </summary>
        /// <param name="requestId">The ID of the requested service request.</param>
        /// <param name="studentId">The ID of the student requesting the details.</param>
        /// <returns>
        /// The request details if found and owned by the student; otherwise, null.
        /// </returns>
        Task<ServiceRequestDetailsDto?> GetRequestDetailsAsync(
            int requestId,
            int studentId);

        /// <summary>
        /// Cancels a service request if it belongs to the student
        /// and its current status allows cancellation.
        /// </summary>
        /// <param name="requestId">The ID of the service request.</param>
        /// <param name="studentId">The ID of the student requesting cancellation.</param>
        /// <returns>
        /// True if the request was cancelled successfully; otherwise, false.
        /// </returns>
        Task<bool> CancelRequestAsync(
            int requestId,
            int studentId);

        /// <summary>
        /// Retrieves service requests associated with a specific department.
        /// </summary>
        /// <param name="departmentId">The ID of the department.</param>
        /// <returns>A collection of service request summaries.</returns>
        Task<IEnumerable<ServiceRequestResponseDto>> GetDepartmentRequestsAsync(
            int departmentId);

        /// <summary>
        /// Updates the status of a service request after validating
        /// the employee's department and the allowed status transition.
        /// </summary>
        /// <param name="requestId">The ID of the service request.</param>
        /// <param name="employeeId">The ID of the employee handling the request.</param>
        /// <param name="departmentId">The ID of the employee's department.</param>
        /// <param name="dto">The data containing the new status and rejection reason.</param>
        /// <returns>
        /// True if the status was updated successfully; otherwise, false.
        /// </returns>
        Task<bool> UpdateRequestStatusAsync(
            int requestId,
            int employeeId,
            int departmentId,
            UpdateServiceRequestStatusDto dto);
    }
}
