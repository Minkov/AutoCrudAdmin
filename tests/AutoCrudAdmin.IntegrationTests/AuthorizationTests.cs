namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AuthorizationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public AuthorizationTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ResetDatabase();
    }

    [Fact]
    public async Task OnActionExecuting_WithAuthFilter_CallsIsAuthorized()
    {
        var authFilterMock = new Mock<IAutoCrudAuthFilter>();
        authFilterMock
            .Setup(x => x.IsAuthorized(It.IsAny<ActionExecutingContext>()))
            .Returns(true);

        var controller = this.CreateEmployeeControllerWithAuth(new[] { authFilterMock.Object });
        var actionContext = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);

        await controller.OnActionExecutionAsync(
            actionContext,
            () => Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller)));

        authFilterMock.Verify(x => x.IsAuthorized(It.IsAny<ActionExecutingContext>()), Times.Once);
    }

    [Fact]
    public async Task OnActionExecuting_WithAsyncAuthFilter_CallsIsAuthorizedAsync()
    {
        var asyncAuthFilterMock = new Mock<IAsyncAuthCrudFilter>();
        asyncAuthFilterMock
            .Setup(x => x.IsAuthorizedAsync(It.IsAny<ActionExecutingContext>()))
            .ReturnsAsync(true);

        var controller = this.CreateEmployeeControllerWithAsyncAuth(new[] { asyncAuthFilterMock.Object });
        var actionContext = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);

        await controller.OnActionExecutionAsync(
            actionContext,
            () => Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller)));

        asyncAuthFilterMock.Verify(
            x => x.IsAuthorizedAsync(It.IsAny<ActionExecutingContext>()),
            Times.Once);
    }

    [Fact]
    public async Task OnActionExecuting_UnauthorizedFilter_ReturnsForbidResult()
    {
        var authFilterMock = new Mock<IAutoCrudAuthFilter>();
        authFilterMock
            .Setup(x => x.IsAuthorized(It.IsAny<ActionExecutingContext>()))
            .Returns(false);

        var controller = this.CreateEmployeeControllerWithAuth(new[] { authFilterMock.Object });
        var actionContext = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);

        await controller.OnActionExecutionAsync(
            actionContext,
            () => Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller)));

        actionContext.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task OnActionExecuting_MultipleAuthFilters_AllMustAuthorize()
    {
        var authFilter1 = new Mock<IAutoCrudAuthFilter>();
        authFilter1.Setup(x => x.IsAuthorized(It.IsAny<ActionExecutingContext>())).Returns(true);

        var authFilter2 = new Mock<IAutoCrudAuthFilter>();
        authFilter2.Setup(x => x.IsAuthorized(It.IsAny<ActionExecutingContext>())).Returns(false);

        var controller = this.CreateEmployeeControllerWithAuth(new[] { authFilter1.Object, authFilter2.Object });
        var actionContext = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller);

        await controller.OnActionExecutionAsync(
            actionContext,
            () => Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller)));

        actionContext.Result.Should().BeOfType<ForbidResult>();
    }

    private EmployeeControllerWithAuth CreateEmployeeControllerWithAuth(IEnumerable<IAutoCrudAuthFilter> authFilters)
    {
        var controller = new EmployeeControllerWithAuth(this.fixture.DbContext, authFilters)
        {
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()),
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        controller.HttpContext.Items["layout_name"] = "_Layout";
        controller.HttpContext.Items["application_name"] = "Test App";

        return controller;
    }

    private EmployeeControllerWithAsyncAuth CreateEmployeeControllerWithAsyncAuth(IEnumerable<IAsyncAuthCrudFilter> authFilters)
    {
        var controller = new EmployeeControllerWithAsyncAuth(this.fixture.DbContext, authFilters)
        {
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()),
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        controller.HttpContext.Items["layout_name"] = "_Layout";
        controller.HttpContext.Items["application_name"] = "Test App";

        return controller;
    }

    private class EmployeeControllerWithAuth : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithAuth(TestDbContext context, IEnumerable<IAutoCrudAuthFilter> authFilters)
            : base(context)
        {
            this.Authorization = authFilters;
        }
    }

    private class EmployeeControllerWithAsyncAuth : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithAsyncAuth(TestDbContext context, IEnumerable<IAsyncAuthCrudFilter> authFilters)
            : base(context)
        {
            this.AsyncAuthorization = authFilters;
        }
    }
}
