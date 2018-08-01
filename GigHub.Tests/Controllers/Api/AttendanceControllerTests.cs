using FluentAssertions;
using GigHub.Controllers.Api;
using GigHub.Core;
using GigHub.Core.Dtos;
using GigHub.Core.Models;
using GigHub.Core.Repositories;
using GigHub.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Http.Results;

namespace GigHub.Tests.Controllers.Api
{
    [TestClass]
    public class AttendanceControllerTests
    {
        private AttendancesController _attendancesController;
        private Mock<IAttendanceRepository> _mockRepository;
        private string _userId;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IAttendanceRepository>();

            var mockUnitofWork = new Mock<IUnitOfWork>();
            mockUnitofWork.SetupGet(a => a.Attendances).Returns(_mockRepository.Object);

            _attendancesController = new AttendancesController(mockUnitofWork.Object);
            _userId = "1";
            _attendancesController.MockCurrentUser(_userId, "user1@domain.com");
        }

        [TestMethod]
        public void Attend_UserAttendsGigHeAlreadyHasAttendanceTo_ShouldReturnBadRequest()
        {
            var attendance = new Attendance();

            _mockRepository.Setup(r => r.GetAttendance(1, _userId)).Returns(attendance);

            var result = _attendancesController.Attend(new AttendanceDto {GigId = 1});

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [TestMethod]
        public void Attend_UserSuccessfullyAttendsGig_ShouldReturnOk()
        {
            var result = _attendancesController.Attend(new AttendanceDto{ GigId = 1});

            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public void DeleteAttendance_UserAttemptsToDeleteGigWhichDoesNotExist_ShouldReturnNotFound()
        {
            var result = _attendancesController.DeleteAttendance(1);

            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void DeleteAttendance_ValidDeleteRequest_ShouldReturnOk()
        {
            var attendance = new Attendance();

            _mockRepository.Setup(r => r.GetAttendance(1, _userId)).Returns(attendance);

            var result = _attendancesController.DeleteAttendance(1);

            result.Should().BeOfType<OkNegotiatedContentResult<int>>();
        }

        [TestMethod]
        public void DeleteAttendance_ValidDeleteRequest_ShouldReturnIdOfDeletedGig()
        {
            var attendance = new Attendance();

            _mockRepository.Setup(r => r.GetAttendance(1, _userId)).Returns(attendance);

            var result = (OkNegotiatedContentResult<int>) _attendancesController.DeleteAttendance(1);

            result.Content.Should().Be(1);
        }

    }
}
