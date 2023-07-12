namespace AutoCrudAdmin.Filters
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The IAutoCrudAuthFilter interface defines the contract for an authorization filter in the AutoCrudAdmin system.
    /// Implementations of this interface should provide a method for synchronously authorizing an HTTP context.
    /// </summary>
    public interface IAutoCrudAuthFilter
    {
        /// <summary>
        /// The Authorize method determines whether the provided HTTP context is authorized.
        /// </summary>
        /// <param name="context">The HTTP context to be authorized.</param>
        /// <returns>True if the context is authorized; otherwise, false.</returns>
        bool Authorize(HttpContext context);
    }

    /// <summary>
    /// The IAsyncAuthCrudFilter interface defines the contract for an asynchronous authorization filter in the AutoCrudAdmin system.
    /// Implementations of this interface should provide a method for asynchronously authorizing an HTTP context.
    /// </summary>
    public interface IAsyncAuthCrudFilter
    {
        /// <summary>
        /// The Authorize method asynchronously determines whether the provided HTTP context is authorized.
        /// </summary>
        /// <param name="context">The HTTP context to be authorized.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean that is true if the context is authorized; otherwise, false.</returns>
        Task<bool> Authorize(HttpContext context);
    }
}