// <copyright file="SubmitterDeploymentTest.cs" company="Total Quality Logistics"> 
// Copyright (c) Total Quality Logistics. All rights reserved. 
// </copyright> 
using AutoFixture;
using ChangeControlAPI.Constants;
using ChangeControlAPI.Models;
using ChangeControlAPI.Repository;
using ChangeControlAPI.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ChangeControlAPI.Test
{
   public class SubmitterDeploymentTest
    {
        public SubmitterDeploymentTest()
        {

        }

        [Fact]
        public void GetSubmitterDeploymentStatus_Success()
        {
            //Arrage
            var deploymentStatuses = new Fixture().CreateMany<SubmitterDeploymentStatus>(3).AsQueryable();
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentStatusRepositoryMock.Setup(x => x.GetAll()).Returns(deploymentStatuses);
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            //Act
            var results = service.GetSubmitterDeploymentStatus();

            //Assert
            results.Should().NotBeNull();
            results.Count().Should().Be(deploymentStatuses.Count());
            results.Should().BeEquivalentTo(deploymentStatuses);
        }

        [Fact]
        public void GetSubmitterDeploymentStatus_Failure()
        {
            //Arrage
            var deploymentStatuses = new Fixture().CreateMany<SubmitterDeploymentStatus>(0).AsQueryable();
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentStatusRepositoryMock.Setup(x => x.GetAll()).Returns(deploymentStatuses);
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            //Act
            var results = service.GetSubmitterDeploymentStatus();

            //Assert
            results.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetSubmitterDeploymentHistory_Success()
        {
            //Arrage
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var deploymentHistories = fixture.CreateMany<SubmitterDeploymentHistory>(9).AsQueryable();
            var ticketId = deploymentHistories.Select(x => x.TicketID).First();
            var expectedResult = deploymentHistories.Where(x => x.TicketID == ticketId);
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentHistoryRepositoryMock.Setup(x => x.GetAll()).Returns(deploymentHistories);
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            //Act
            var results = service.GetSubmitterDeploymentHistory(ticketId);

            //Assert
            results.Should().NotBeNull();
            results.Count().Should().Be(expectedResult.Count());
            results.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetSubmitterDeploymentHistory_Failure()
        {
            //Arrage
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var deploymentHistories = fixture.CreateMany<SubmitterDeploymentHistory>(0).AsQueryable();
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentHistoryRepositoryMock.Setup(x => x.GetAll()).Returns(deploymentHistories);
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            //Act
            var results = service.GetSubmitterDeploymentHistory(1);

            //Assert
            results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async System.Threading.Tasks.Task AddsubmitterDeploymentHistory_SuccessAsync()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var newDeployment = fixture.Create<SubmitterDeploymentHistory>();
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentHistoryRepositoryMock.Setup(x => x.CreateAsync(newDeployment));
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            // Act
            var result = await service.AddsubmitterDeploymentHistory(newDeployment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(newDeployment);
            result.Should().BeSameAs(newDeployment);
        }

        [Fact]
        public void AddsubmitterApprovalHistory_Exception()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var newDeployment = fixture.Create<SubmitterDeploymentHistory>();
            var submitterDeploymentStatusRepositoryMock = new Mock<ISubmitterDeploymentStatusRepository>();
            var submitterDeploymentHistoryRepositoryMock = new Mock<ISubmitterDeploymentHistoryRepository>();
            submitterDeploymentHistoryRepositoryMock.Setup(x => x.CreateAsync(newDeployment));
            var service = new SubmitterDeploymentService(submitterDeploymentStatusRepositoryMock.Object, submitterDeploymentHistoryRepositoryMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => service.AddsubmitterDeploymentHistory(null));
            // Assert
            ex.Should().NotBeNull();
            ex.Result.Should().NotBeNull();
            ex.Result.Message.Should().NotBeNull();
            
            Assert.Equal(string.Format(Messages.ArgumentNullExceptionMessage, service.GetType().GetMethod("AddsubmitterDeploymentHistory").GetParameters()[0].Name), ex.Result.Message);
        }
    }
}