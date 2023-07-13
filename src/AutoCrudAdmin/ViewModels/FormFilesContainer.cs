namespace AutoCrudAdmin.ViewModels;

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a container for form files in the AutoCrudAdmin application.
/// This container encapsulates a collection of single form files and collections of form files.
/// </summary>
public class FormFilesContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormFilesContainer"/> class.
    /// </summary>
    public FormFilesContainer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormFilesContainer"/> class.
    /// </summary>
    /// <param name="formFiles">An array of form files.</param>
    public FormFilesContainer(params IFormFile?[] formFiles)
        => this.SingleFiles = formFiles.Where(f => f != null) !;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormFilesContainer"/> class.
    /// </summary>
    /// <param name="formFileCollections">An array of form file collections.</param>
    public FormFilesContainer(params IFormFileCollection?[] formFileCollections)
        => this.MultipleFiles = formFileCollections.Where(fc => fc != null) !;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormFilesContainer"/> class.
    /// </summary>
    /// <param name="formFiles">A collection of form files.</param>
    /// <param name="formFileCollections">A collection of form file collections.</param>
    public FormFilesContainer(IEnumerable<IFormFile> formFiles, IEnumerable<IFormFileCollection> formFileCollections)
    {
        this.SingleFiles = formFiles;
        this.MultipleFiles = formFileCollections;
    }

    /// <summary>
    /// Gets the collection of single form files.
    /// </summary>
    public IEnumerable<IFormFile> SingleFiles { get; private set; }
        = Enumerable.Empty<IFormFile>();

    /// <summary>
    /// Gets the collection of form file collections.
    /// </summary>
    public IEnumerable<IFormFileCollection> MultipleFiles { get; private set; }
        = Enumerable.Empty<IFormFileCollection>();
}