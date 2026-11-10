
using USMS.Core.DTOs.Complaints;

namespace USMS.Core.Interfaces
{
    /// <summary>
    /// Defines operations for creating, retrieving, assigning, and updating complaints.
    /// </summary>
    /// <remarks>Implementations manage complaint lifecycle tasks, including creation, retrieval by student,
    /// department assignment, and status updates.</remarks>
    public interface IComplaintService
    {
        
        Task<ComplaintResponseDto> CreateComplaintAsync(
            CreateComplaintRequestDto request, int studentId);

        Task<List<ComplaintResponseDto>> GetMyComplaintsAsync(
            int studentId);

        Task<bool> AssignComplaintDepartmentAsync(
           int complaintId, int departmentId);


        Task<bool> UpdateComplaintStatusAsync(
           int complaintId,
           int employeeId,
           int departmentId,
           UpdateComplaintStatusRequestDto request);
    }
}
