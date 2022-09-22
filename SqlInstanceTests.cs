// <copyright file="SqlInstanceTests.cs" company="Total Quality Logistics"> 
// Copyright (c) Total Quality Logistics. All rights reserved. 
// </copyright> 
using AutoFixture;
using ChangeControlAPI.Models;
using ChangeControlAPI.Repository;
using ChangeControlAPI.Services;
using FluentAssertions;
using Moq;
using System.Linq;
using Xunit;

namespace ChangeControlAPI.Test
{
    public class SqlInstanceTests
    {
        [Fact]
        public void GetAllSqlInstance_Success()
        {
            //Arrange
            var sqlInstance = new Fixture().CreateMany<SqlInstance>(0).AsQueryable();
            var sqlInstanceRepositoryMock = new Mock<ISqlInstancesRepository>();
            sqlInstanceRepositoryMock.Setup(p => p.GetAll()).Returns((sqlInstance));
            var service = new SqlInstancesService(sqlInstanceRepositoryMock.Object);
            //Act
            var results = service.GetSqlInstances();
            //Assert
            results.Count().Should().Be(sqlInstance.Count());
            results.Should().BeSameAs(sqlInstance);
        }
        [Fact]
        public void GetAllSqlInstance_Failure()
        {
            //Arrange
            var sqlInstance = new Fixture().CreateMany<SqlInstance>(0).AsQueryable();
            var sqlInstanceRepositoryMock = new Mock<ISqlInstancesRepository>();
            sqlInstanceRepositoryMock.Setup(p => p.GetAll()).Returns((sqlInstance));
            var service = new SqlInstancesService(sqlInstanceRepositoryMock.Object);
            //Act
            var results = service.GetSqlInstances();
            //Assert
            results.Should().BeNullOrEmpty();
        }
    }
}
