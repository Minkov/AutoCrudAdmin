namespace AutoCrudAdmin.Demo.SqlServer.Tests.Controllers;

using System;
using System.Collections.Generic;
using Data;
using FluentAssertions;
using Models.Models;
using MyTested.AspNetCore.Mvc;
using SqlServer.Controllers;
using ViewModels;
using Xunit;

public class ProjectsControllerTests
{
    [Fact]
    public void IndexShouldReturnDefaultViewWithCorrectModel()
        => MyController<ProjectsController>
            .Instance()
            .Calling(c => c.Index())
            .ShouldReturn()
            .View("../AutoCrudAdmin/Index");

    [Fact]
    public void GetCreate_ReturnsViewResult()
        => MyController<ProjectsController>
            .Instance()
            .Calling(c => c.Create(
                new Dictionary<string, string>(),
                null))
            .ShouldReturn()
            .View("../AutoCrudAdmin/EntityForm");

    [Theory]
    [InlineData(1)]
    public void GetEdit_ReturnsViewResult(int id)
        => MyController<ProjectsController>
            .Instance(controller => controller
                .WithData(ProjectTestData.GetProject(id)))
            .Calling(c => c.Edit(
                new Dictionary<string, string>
                {
                    { Constants.Entity.SinglePrimaryKeyName, id.ToString() },
                },
                null))
            .ShouldReturn()
            .View("../AutoCrudAdmin/EntityForm");

    [Theory]
    [InlineData(1)]
    public void GetDelete_ReturnsViewResult(int id)
        => MyController<ProjectsController>
            .Instance()
            .Calling(c => c.Delete(
                new Dictionary<string, string>
                {
                    { Constants.Entity.SinglePrimaryKeyName, id.ToString() },
                },
                null))
            .ShouldReturn()
            .View("../AutoCrudAdmin/EntityForm");

    [Theory]
    [InlineData("Test Project")]
    public void PostCreate_SavesAndRedirects(string name)
        => MyController<ProjectsController>
            .Instance()
            .Calling(c => c.PostCreate(
                new Dictionary<string, string>
                {
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString() },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString() },
                },
                new FormFilesContainer()))
            .ShouldHave()
            .Data(data => data
                .WithSet<Project>(set => set
                    .Should()
                    .ContainSingle(p => p.Name == name)))
            .AndAlso()
            .ShouldReturn()
            .Redirect(redirect => redirect
                .To<ProjectsController>(c => c.Index()));
}