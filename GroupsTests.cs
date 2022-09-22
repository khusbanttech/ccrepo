// <copyright file="GroupTests.cs" company="Total Quality Logistics"> 
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
    public class GroupTests
    {
        [Fact]
        public void GetAllGroup_Success()
        {
            //Arrange
            var group = new Fixture().CreateMany<Group>(1).AsQueryable();
            var groupRepositoryMock = new Mock<IGroupsRepository>();
            groupRepositoryMock.Setup(p => p.GetAll()).Returns((group));
            var service = new GroupsService(groupRepositoryMock.Object);
            //Act
            var results = service.GetGroups();
            //Assert
            results.Count().Should().Be(group.Count());
            results.Select(x => x.GroupID).Should().BeEquivalentTo(group.Select(y => y.GroupID));
        }
        [Fact]
        public void GetAllGroup_Failure()
        {
            //Arrange
            var group = new Fixture().CreateMany<Group>(0).AsQueryable();
            var groupRepositoryMock = new Mock<IGroupsRepository>();
            groupRepositoryMock.Setup(p => p.GetAll()).Returns((group));
            var service = new GroupsService(groupRepositoryMock.Object);
            //Act
            var results = service.GetGroups();
            //Assert
            results.Should().BeNullOrEmpty();
        }
    }
}
