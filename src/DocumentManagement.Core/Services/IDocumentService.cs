using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Services;

public interface IDocumentService
{
    Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, CancellationToken cancellationToken = default);
}