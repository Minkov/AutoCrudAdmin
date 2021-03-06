namespace AutoCrudAdmin.ViewModels;

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

public class FormFilesContainer
{
    public FormFilesContainer()
    {
    }

    public FormFilesContainer(params IFormFile?[] formFiles)
        => this.SingleFiles = formFiles.Where(f => f != null) !;

    public FormFilesContainer(params IFormFileCollection?[] formFileCollections)
        => this.MultipleFiles = formFileCollections.Where(fc => fc != null) !;

    public FormFilesContainer(IEnumerable<IFormFile> formFiles, IEnumerable<IFormFileCollection> formFileCollections)
    {
        this.SingleFiles = formFiles;
        this.MultipleFiles = formFileCollections;
    }

    public IEnumerable<IFormFile> SingleFiles { get; private set; }
        = Enumerable.Empty<IFormFile>();

    public IEnumerable<IFormFileCollection> MultipleFiles { get; private set; }
        = Enumerable.Empty<IFormFileCollection>();
}