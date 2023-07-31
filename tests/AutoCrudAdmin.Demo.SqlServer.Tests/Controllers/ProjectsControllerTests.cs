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
            .Instance(controller => controller
                .WithData(ProjectTestData.GetProject(id)))
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
        => MyPipeline
            .Configuration()
            .ShouldMap(request => request
                .WithMethod(HttpMethod.Post)
                .WithLocation("/Projects/PostCreate")
                .WithAntiForgeryToken()
                .WithFormFields(new Dictionary<string, string>
                {
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString() },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString() },
                }))
            .To<ProjectsController>(c => c.PostCreate(
                new Dictionary<string, string>
                {
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString() },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString() },
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

    [Theory]
    [InlineData(1, "Test Edit Project")]
    public void PostEdit_SavesAndRedirects(int id, string name)
        => MyPipeline
            .Configuration()
            .ShouldMap(request => request
                .WithMethod(HttpMethod.Post)
                .WithLocation("/Projects/PostEdit")
                .WithAntiForgeryToken()
                .WithFormFields(new Dictionary<string, string>
                {
                    { nameof(Project.Id), id.ToString() },
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString() },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString() },
                }))
            .To<ProjectsController>(c => c.PostEdit(
                new Dictionary<string, string>
                {
                    { nameof(Project.Id), id.ToString() },
                    { nameof(Project.Name), name },
                    { nameof(Project.OpenDate), DateTime.Now.ToString() },
                    { nameof(Project.DueDate), DateTime.Now.AddDays(1).ToString() },
                },
                new FormFilesContainer()))
            .Which()
            .WithData(ProjectTestData.GetProject(id))
            .ShouldHave()
            .Data(data => data
                .WithSet<Project>(set => set
                    .Should()
                    .ContainSingle(p => p.Id == id && p.Name == name)))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("Index");
}