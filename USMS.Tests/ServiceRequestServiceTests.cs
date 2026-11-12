using Microsoft.EntityFrameworkCore;
using USMS.Core.Entities;
using USMS.Infrastructure.Persistence;
using USMS.Infrastructure.Services;
using USMS.Core.DTOs.ServiceRequests;

namespace USMS.Tests
{
    /// <summary>
    /// Contains unit tests for the ServiceRequestService class.
    /// Tests request creation, cancellation, retrieval, and status management.
    /// </summary>
    public class ServiceRequestServiceTests
    {
        /// <summary>
        /// Creates an isolated in-memory database context for a unit test.
        /// </summary>
        /// <returns>A new ApplicationDbContext instance.</returns>
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }


        /// <summary>
        /// Verifies that a valid certificate request is created successfully.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_ValidCertificateRequest_ReturnsCreatedRequest()
        {
            // Arrange
            using var context = CreateContext();

            var student = new Student
            {
                Id = 1
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            context.Students.Add(student);
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Certificate Request",
                ServiceId = 1,
                CertificateType = "Enrollment Certificate",
                Purpose = "University application",
                Notes = "Urgent request"
            };

            // Act
            var result = await serviceRequestService.CreateRequestAsync(dto, student.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Certificate Request", result.RequestType);
            Assert.Equal("Student Affairs", result.ServiceName);
            Assert.Equal("Submitted", result.Status);
            Assert.Equal("Enrollment Certificate", result.CertificateType);

            var savedRequest = await context.ServiceRequests.FirstOrDefaultAsync();

            Assert.NotNull(savedRequest);
            Assert.Equal(student.Id, savedRequest.StudentId);
            Assert.Equal(service.Id, savedRequest.ServiceId);
        }


        /// <summary>
        /// Verifies that an invalid request type throws an ArgumentException.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_InvalidRequestType_ThrowsArgumentException()
        {
            // Arrange
            using var context = CreateContext();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Invalid Request",
                ServiceId = 1,
                Purpose = "Testing"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, 1));

            Assert.Equal("Invalid request type.", exception.Message);
        }

        /// <summary>
        /// Verifies that a certificate request without a certificate type is rejected.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_MissingCertificateType_ThrowsArgumentException()
        {
            // Arrange
            using var context = CreateContext();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Certificate Request",
                ServiceId = 1,
                CertificateType = null,
                Purpose = "Testing"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, 1));

            Assert.Equal("Certificate type is required.", exception.Message);
        }


        /// <summary>
        /// Verifies that creating a request for a non-existing student is rejected.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_StudentNotFound_ThrowsArgumentException()
        {
            // Arrange
            using var context = CreateContext();

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Certificate Request",
                ServiceId = 1,
                CertificateType = "Enrollment Certificate",
                Purpose = "Testing"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, 999));

            Assert.Equal("Student not found.", exception.Message);
        }

        /// <summary>
        /// Verifies that requests cannot be created for inactive services.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_InactiveService_ThrowsArgumentException()
        {
            // Arrange
            using var context = CreateContext();

            var student = new Student
            {
                Id = 1
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = false
            };

            context.Students.Add(student);
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Certificate Request",
                ServiceId = 1,
                CertificateType = "Enrollment Certificate",
                Purpose = "Testing"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, student.Id));

            Assert.Equal("Service not found or inactive.", exception.Message);
        }


        /// <summary>
        /// Verifies that a submitted request can be cancelled successfully.
        /// </summary>
        [Fact]
        public async Task CancelRequestAsync_SubmittedRequest_ReturnsTrue()
        {
            // Arrange
            using var context = CreateContext();

            var student = new Student
            {
                Id = 1
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = 1
            };

            context.Students.Add(student);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            // Act
            var result = await serviceRequestService.CancelRequestAsync(1, 1);

            // Assert
            Assert.True(result);

            var cancelledRequest = await context.ServiceRequests.FindAsync(1);

            Assert.Equal("Cancelled", cancelledRequest!.Status);
            Assert.NotNull(cancelledRequest.UpdatedAt);
        }

        /// <summary>
        /// Verifies that a request cannot be cancelled after entering review.
        /// </summary>
        [Fact]
        public async Task CancelRequestAsync_UnderReviewRequest_ReturnsFalse()
        {
            // Arrange
            using var context = CreateContext();

            var student = new Student
            {
                Id = 1
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Under Review",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = 1
            };

            context.Students.Add(student);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            // Act
            var result = await serviceRequestService.CancelRequestAsync(1, 1);

            // Assert
            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(1);

            Assert.Equal("Under Review", unchangedRequest!.Status);
        }


        /// <summary>
        /// Verifies that a submitted request can move to Under Review.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_SubmittedToUnderReview_ReturnsTrue()
        {
            // Arrange
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Under Review"
            };

            // Act
            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            // Assert
            Assert.True(result);

            var updatedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Under Review", updatedRequest!.Status);
            Assert.Equal(employee.Id, updatedRequest.AssignedEmployeeId);
            Assert.NotNull(updatedRequest.UpdatedAt);
        }

        /// <summary>
        /// Verifies that invalid status transitions are rejected.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_InvalidTransition_ReturnsFalse()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Approved"
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Submitted", unchangedRequest!.Status);
        }

        /// <summary>
        /// Verifies that a request cannot be rejected without a reason.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_RejectedWithoutReason_ReturnsFalse()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Under Review",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Rejected",
                RejectionReason = null
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Under Review", unchangedRequest!.Status);
        }

        /// <summary>
        /// Verifies that a request can be rejected when a reason is provided.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_RejectedWithReason_ReturnsTrue()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Under Review",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Rejected",
                RejectionReason = "Missing required documents."
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            Assert.True(result);

            var updatedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Rejected", updatedRequest!.Status);
            Assert.Equal("Missing required documents.", updatedRequest.RejectionReason);
            Assert.Equal(employee.Id, updatedRequest.AssignedEmployeeId);
            Assert.NotNull(updatedRequest.UpdatedAt);
        }

        /// <summary>
        /// Verifies that students can retrieve only their own requests.
        /// </summary>
        [Fact]
        public async Task GetMyRequestsAsync_ReturnsOnlyStudentRequests()
        {
            using var context = CreateContext();

            var student1 = new Student { Id = 1 };
            var student2 = new Student { Id = 2 };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request1 = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "University application",
                StudentId = student1.Id,
                ServiceId = service.Id
            };

            var request2 = new ServiceRequest
            {
                Id = 2,
                RequestType = "Certificate Request",
                Status = "Under Review",
                Purpose = "Scholarship application",
                StudentId = student2.Id,
                ServiceId = service.Id
            };

            context.Students.AddRange(student1, student2);
            context.Services.Add(service);
            context.ServiceRequests.AddRange(request1, request2);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.GetMyRequestsAsync(student1.Id);

            Assert.Single(result);
            Assert.Equal(request1.Id, result.First().Id);
            Assert.Equal("Student Affairs", result.First().ServiceName);
        }

        /// <summary>
        /// Verifies that the owner can retrieve their request details.
        /// </summary>
        [Fact]
        public async Task GetRequestDetailsAsync_RequestOwner_ReturnsRequestDetails()
        {
            using var context = CreateContext();

            var student = new Student { Id = 1 };
            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "University application",
                StudentId = student.Id,
                ServiceId = service.Id
            };

            context.Students.Add(student);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.GetRequestDetailsAsync(
                request.Id, student.Id);

            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.Equal("Student Affairs", result.ServiceName);
            Assert.Equal("Submitted", result.Status);
        }

        /// <summary>
        /// Verifies that another student cannot access the request details.
        /// </summary>
        [Fact]
        public async Task GetRequestDetailsAsync_DifferentStudent_ReturnsNull()
        {
            using var context = CreateContext();

            var student = new Student { Id = 1 };
            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = student.Id,
                ServiceId = service.Id
            };

            context.Students.Add(student);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.GetRequestDetailsAsync(
                request.Id, 999);

            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that null is returned when the requested record does not exist.
        /// </summary>
        [Fact]
        public async Task GetRequestDetailsAsync_RequestNotFound_ReturnsNull()
        {
            using var context = CreateContext();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.GetRequestDetailsAsync(999, 1);

            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that employees can retrieve requests belonging to their department.
        /// </summary>
        [Fact]
        public async Task GetDepartmentRequestsAsync_ReturnsOnlyDepartmentRequests()
        {
            using var context = CreateContext();

            var department1 = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var department2 = new Department
            {
                Id = 2,
                Name = "Registration"
            };

            var service1 = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                DepartmentId = department1.Id
            };

            var service2 = new Service
            {
                Id = 2,
                Name = "Registration Service",
                IsActive = true,
                DepartmentId = department2.Id
            };

            var request1 = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing 1",
                StudentId = 1,
                ServiceId = service1.Id
            };

            var request2 = new ServiceRequest
            {
                Id = 2,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing 2",
                StudentId = 2,
                ServiceId = service2.Id
            };

            context.Departments.AddRange(department1, department2);
            context.Services.AddRange(service1, service2);
            context.ServiceRequests.AddRange(request1, request2);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.GetDepartmentRequestsAsync(
                department1.Id);

            Assert.Single(result);
            Assert.Equal(request1.Id, result.First().Id);
        }

        /// <summary>
        /// Verifies that employees cannot update requests belonging to another department.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_DifferentDepartment_ReturnsFalse()
        {
            using var context = CreateContext();

            var department1 = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var department2 = new Department
            {
                Id = 2,
                Name = "Registration"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department2.Id,
                Department = department2
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department1.Id,
                Department = department1
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.AddRange(department1, department2);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Under Review"
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department2.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Submitted", unchangedRequest!.Status);
        }

        /// <summary>
        /// Verifies that lowercase status values are normalized correctly.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_LowercaseStatus_ReturnsTrue()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "under review"
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            Assert.True(result);

            var updatedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Under Review", updatedRequest!.Status);
        }

        /// <summary>
        /// Verifies that unknown status values are rejected.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_UnknownStatus_ReturnsFalse()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var user = new ApplicationUser
            {
                Id = "user-1",
                UserName = "employee1",
                FullName = "Test Employee"
            };

            var employee = new Employee
            {
                Id = 1,
                FullName = "Test Employee",
                UserId = user.Id,
                ApplicationUser = user,
                DepartmentId = department.Id,
                Department = department
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Users.Add(user);
            context.Employees.Add(employee);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);

            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Unknown Status"
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                employee.Id,
                department.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(request.Id);

            Assert.Equal("Submitted", unchangedRequest!.Status);
        }


        /// <summary>
        /// Verifies that a valid enrollment confirmation request is created successfully.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_ValidEnrollmentRequest_ReturnsCreatedRequest()
        {
            using var context = CreateContext();

            var student = new Student { Id = 1 };
            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            context.Students.Add(student);
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Enrollment Confirmation Request",
                ServiceId = 1,
                Purpose = "University application",
                AcademicYear = "2026/2027",
                Semester = "First Semester"
            };

            var result = await serviceRequestService.CreateRequestAsync(dto, student.Id);

            Assert.NotNull(result);
            Assert.Equal("Enrollment Confirmation Request", result.RequestType);
            Assert.Equal("Submitted", result.Status);
            Assert.Equal("2026/2027", result.AcademicYear);
            Assert.Equal("First Semester", result.Semester);
        }

        /// <summary>
        /// Verifies that an enrollment request without an academic year is rejected.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_MissingAcademicYear_ThrowsArgumentException()
        {
            using var context = CreateContext();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Enrollment Confirmation Request",
                ServiceId = 1,
                Purpose = "Testing",
                AcademicYear = null,
                Semester = "First Semester"
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, 1));
        }

        /// <summary>
        /// Verifies that an enrollment request without a semester is rejected.
        /// </summary>
        [Fact]
        public async Task CreateRequestAsync_MissingSemester_ThrowsArgumentException()
        {
            using var context = CreateContext();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new CreateServiceRequestDto
            {
                RequestType = "Enrollment Confirmation Request",
                ServiceId = 1,
                Purpose = "Testing",
                AcademicYear = "2026/2027",
                Semester = null
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => serviceRequestService.CreateRequestAsync(dto, 1));
        }

        /// <summary>
        /// Verifies that a student cannot cancel another student's request.
        /// </summary>
        [Fact]
        public async Task CancelRequestAsync_DifferentStudent_ReturnsFalse()
        {
            using var context = CreateContext();

            var student = new Student { Id = 1 };
            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs",
                IsActive = true
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = 1
            };

            context.Students.Add(student);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var result = await serviceRequestService.CancelRequestAsync(1, 999);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(1);
            Assert.Equal("Submitted", unchangedRequest!.Status);
        }

        /// <summary>
        /// Verifies that request status cannot be updated by a non-existing employee.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_EmployeeNotFound_ReturnsFalse()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = "Under Review"
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                999,
                department.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(1);
            Assert.Equal("Submitted", unchangedRequest!.Status);
        }

        /// <summary>
        /// Verifies that an empty status value is rejected.
        /// </summary>
        [Fact]
        public async Task UpdateRequestStatusAsync_EmptyStatus_ReturnsFalse()
        {
            using var context = CreateContext();

            var department = new Department
            {
                Id = 1,
                Name = "Student Affairs"
            };

            var service = new Service
            {
                Id = 1,
                Name = "Student Affairs Service",
                IsActive = true,
                SupportsAppointments = false,
                DepartmentId = department.Id,
                Department = department
            };

            var request = new ServiceRequest
            {
                Id = 1,
                RequestType = "Certificate Request",
                Status = "Submitted",
                Purpose = "Testing",
                StudentId = 1,
                ServiceId = service.Id,
                Service = service
            };

            context.Departments.Add(department);
            context.Services.Add(service);
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();

            var serviceRequestService = new ServiceRequestService(context);

            var dto = new UpdateServiceRequestStatusDto
            {
                Status = ""
            };

            var result = await serviceRequestService.UpdateRequestStatusAsync(
                request.Id,
                999,
                department.Id,
                dto);

            Assert.False(result);

            var unchangedRequest = await context.ServiceRequests.FindAsync(1);
            Assert.Equal("Submitted", unchangedRequest!.Status);
        }

    }
}