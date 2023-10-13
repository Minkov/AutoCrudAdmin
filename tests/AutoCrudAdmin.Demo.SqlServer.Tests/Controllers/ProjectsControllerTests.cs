namespace AutoCrudAdmin.Demo.SqlServer.Tests.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
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
    [InlineData("Test Project")]
    public void PostCreate_SavesAndRedirects(string name)
        => MyPipeline
            .Configuration()
            .ShouldMap(request => request
                .WithMethod(HttpMethod.Post)
                .WithLocation("/Projects/PostCreate")
                .WithAntiForgeryToken()
                .WithFormFields(new Dictionary<string, string>
                {
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString(CultureInfo.CurrentCulture) },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString(CultureInfo.CurrentCulture) },
                }))
            .To<ProjectsController>(c => c.PostCreate(
                new Dictionary<string, string>
                {
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString(CultureInfo.CurrentCulture) },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString(CultureInfo.CurrentCulture) },
                },
                new FormFilesContainer()))
            .Which()
            .ShouldHave()
            .Data(data => data
                .WithSet<Project>(set => set
                    .Should()
                    .ContainSingle(p => p.Name == name)))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("Index");
}