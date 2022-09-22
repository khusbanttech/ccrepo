// <copyright file="TicketTests.cs" company="Total Quality Logistics"> 
// Copyright (c) Total Quality Logistics. All rights reserved. 
// </copyright> 
using AutoFixture;
using AutoMapper;
using ChangeControlAPI.ApplicationServices.Application;
using ChangeControlAPI.ApplicationServices.ApplicationConfigurationOption;
using ChangeControlAPI.ApplicationServices.Email;
using ChangeControlAPI.Common;
using ChangeControlAPI.Constants;
using ChangeControlAPI.DTO;
using ChangeControlAPI.Models;
using ChangeControlAPI.Repository;
using ChangeControlAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChangeControlAPI.Test
{
    public class TicketTests
    {
        private readonly IOptions<ApplicationConfigurationOptions> appConfigOptions;

        TicketFormDTO ticketForm = new TicketFormDTO()
        {
            ChangeTitle = "Test Change Title",
            Applications = new List<int>() { 1, 2 },
            ImpactedSystems = new List<int>() { 1, 2 },
            AzureDevopsNo = "Az",
            ChangeDescription = "CD",
            RollbackStrategy = "Rollback",
            Databases = new List<int>() { 1, 2 },
            DownloadedPath = null,
            EmergencyReason = "test",
            EndDateTime = DateTime.Now.AddDays(1),
            Files = null,
            Groups = new List<int>() { 1, 2 },
            ChangeType = 1,
            SqlInstances = new List<int>() { 1, 1 },
            StartDateTime = DateTime.Now,
            Submitter = "Abhishek.Sinha",
            TicketFiles = null,
            TicketID = 1,
            IsHighRisk = true,
            IsQAApproved = true,
            IsSecurityConcern = true,
            IsActive = true,
            DBANotes = null
        };
        Ticket ticket = new Ticket()
        {
            ChangeTitle = "Test Change Title",
            Applications = new List<TicketApplication>() {
                new TicketApplication(){
                    ApplicationID = 1,
                    TicketApplicationID = 0
                },
                new TicketApplication(){
                    ApplicationID = 2,
                    TicketApplicationID = 0
                }
                },
            ImpactedSystems = new List<ImpactedSystem>() {
                new ImpactedSystem(){
                    ApplicationID = 1,
                    ImpactedSystemID = 0
                }
                },
            AzureDevOpsNo = "Az",
            ChangeDescription = "CD",
            RollbackStrategy = "Rollback",
            Databases = new List<TicketDataBase>() {
                    new TicketDataBase(){ DatabaseID = 1, TicketDataBaseID = 0},
                    new TicketDataBase(){ DatabaseID = 2, TicketDataBaseID = 1}
                },
            DownloadedPath = null,
            EmergencyReason = "test",
            EndDateTime = DateTime.Now.AddDays(1),
            Groups = new List<TicketGroup>() {
                new TicketGroup(){GroupID = 1, TicketGroupID = 0 },
                new TicketGroup(){GroupID = 2, TicketGroupID = 1 }
                },
            ChangeType = 1,
            SqlInstances = new List<TicketSqlInstance>() {
                new TicketSqlInstance(){SqlInstanceID = 1, TicketSqlInstanceID = 1}, },
            StartDateTime = DateTime.Now,
            Submitter = "Abhishek.Sinha",
            TicketFiles = null,
            IsHighRisk = true,
            IsQAApproved = true,
            IsSecurityConcern = true,
            IsActive = true,
            TicketID = 1,
            ChangeTypeRef = new ChangeType() { ChangeTypeID =1, ChangeTypeName="Standard", EmailTemplateID= 123},
            DBANotes = null
        };
        public TicketTests()
        {
            var mockAppConfigOptions = Substitute.For<ApplicationConfigurationOptions>();
            mockAppConfigOptions.DownloadLocation = "";
            mockAppConfigOptions.UploadLocation = "";
            mockAppConfigOptions.getCalendarLink = "";
            appConfigOptions = Options.Create(mockAppConfigOptions);
        }

        [Fact]
        public void Submit_Success()
        {
            //Arrange
            var formFileMock = new Mock<IFormFile>();

            var ticketRepoMock = new Mock<ITicketRepository>();
            var mapperMock = new Mock<IMapper>();
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();
            mapperMock.Setup(a => a.Map<Ticket>(ticketForm)).Returns(ticket);
            mapperMock.Setup(a => a.Map<TicketFormDTO>(ticket)).Returns(ticketForm);
            //appConfigServiceMock.Setup(a => a.GetByKey(Messages.UploadLocationKey)).ReturnsAsync("dummy/location");
            webHostMock.Setup(w => w.EnvironmentName).Returns("Local");
            var httpContext = new Mock<IHttpContextAccessor>();
            var emailService = new Mock<IFireForgetHandler<IEmailService>>();
            var applicationService = new Mock<IApplicationsService>();
            var notification = new Mock<INotificationMgmtRepository>();
            var groupService = new Mock<IGroupService>();

            httpContext.Setup(h => h.HttpContext.Request.Scheme).Returns("Test");
            httpContext.Setup(h => h.HttpContext.Request.Host).Returns(new HostString("Test", 123));
            httpContext.Setup(h => h.HttpContext.Request.PathBase).Returns("/Test/Submit");

            using (var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4 }))
            {
                formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
                    .Callback<Stream, CancellationToken>((stream, token) =>
                     {
                        memoryStream.CopyTo(stream);
                     }).Returns(Task.CompletedTask);
            }
            ticketRepoMock.Setup(p => p.CreateAsync(ticket));
            ticketRepoMock.Setup(p => p.GetAsync(ticket.TicketID));
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object, applicationService.Object, 
                emailService.Object, httpContext.Object, notification.Object, groupService.Object);
            //Act
            var results = service.SubmitTicket(ticketForm).Result;
            //Assert
            results.Should().BeSameAs(ticketForm);
        }

        [Fact]
        public void Submit_TicketNull_Exception()
        {
            // Arrange
            var ticketRepoMock = new Mock<ITicketRepository>();
            var mapperMock = new Mock<IMapper>();
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object, null, null, null, null, null);
            // Act
            // Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitTicket(null));
            ex.Should().NotBeNull();
            ex.Result.Should().NotBeNull();
            ex.Result.Message.Should().NotBeNull();
            Assert.Equal(string.Format(Messages.ArgumentNullExceptionMessage,service.GetType().GetMethod("SubmitTicket").GetParameters()[0].Name), ex.Result.Message);
        }

        [Fact]
        public void GetAllTicketGrid_Success()
        {
            // Arrange
            var ticketGrid = new Fixture().CreateMany<TicketGrid>(0).AsQueryable();
            var ticketRepoMock = new Mock<ITicketRepository>();
            var mapperMock = new Mock<IMapper>();
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object,null,null,null, null, null);
            //Act
            ticketRepoMock.Setup(p => p.GetTicketsForGrid("1", "3")).Returns((ticketGrid));
            //Act
            var results = service.GetTicketsForGrid("1","3");
            //Assert
            results.Count().Should().Be(ticketGrid.Count());
            results.Should().BeSameAs(ticketGrid);
        }

        [Fact]
        public void GetTicketGrid_WhenTicketStatusIsNotAll_Success()
        {
            // Arrange
            var ticketGrid = new Fixture().CreateMany<TicketGrid>(0).AsQueryable();
            var ticketRepoMock = new Mock<ITicketRepository>();
            var mapperMock = new Mock<IMapper>();
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object, null, null, null, null, null);
            //Act
            ticketRepoMock.Setup(p => p.GetTicketsForGrid("4", "1")).Returns((ticketGrid));
            //Act
            var results = service.GetTicketsForGrid("4", "1");
            //Assert
            results.Count().Should().Be(ticketGrid.Count());
            results.Should().BeSameAs(ticketGrid);
        }


        [Fact]
        public void RemoveTicket_Success()
        {
            // Arrange
            int TicketId = 1;
            var ticketRepoMock = new Mock<ITicketRepository>();
            ticketRepoMock.Setup(x => x.GetAsync(TicketId)).ReturnsAsync(ticket);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<Ticket>(ticket));
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();           
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object, null, null, null, null, null);
            //Act
            var results = service.RemoveTicket(TicketId).Result;
            //Assert
            results.Should().BeSameAs(ticket);
        }
        [Fact]
        public void RemoveTicket_Failure()
        {
            // Arrange           
            var ticketRepoMock = new Mock<ITicketRepository>();
            ticketRepoMock.Setup(x => x.GetAsync(null)).ReturnsAsync(ticket);
            ticketRepoMock.Setup(x => x.UpdateAsync(ticket));
            var mapperMock = new Mock<IMapper>();
            var appConfigServiceMock = new Mock<IApplicationConfigurationService>();
            var webHostMock = new Mock<IWebHostEnvironment>();
            var service = new TicketService(ticketRepoMock.Object, mapperMock.Object, appConfigOptions, webHostMock.Object, null, null, null, null, null);
            //Act
            var results = service.RemoveTicket(0);
            //Assert
            Assert.Equal(string.Format(Messages.TicketIdNotExists),results.Exception.InnerException.Message);
        }
    }
}
