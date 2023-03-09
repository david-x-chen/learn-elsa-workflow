namespace DocumentManagement.Workflows.Activities;

[Action(Category = "Document Management", Description = "Gets the specified document from the database.")]
public class GetDocument : Activity
{
    private readonly IDocumentStore _documentStore;
    private readonly IFileStorage _fileStorage;
    public GetDocument(IDocumentStore documentStore,
        IFileStorage fileStorage)
    {
        _documentStore = documentStore;
        _fileStorage = fileStorage;
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(
        ActivityExecutionContext context)
    {
        var document = await _documentStore.GetAsync(DocumentId, context.CancellationToken);
        var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);

        Output = new DocumentFile(document, fileStream);
        return Done();
    }

    [ActivityInput(
        Label = "Document ID",
        Hint = "The ID of the document to load",
        SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid})]
    public string DocumentId { get; set; } = default;

    [ActivityOutput(
        Hint = "The document + file",
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName)]
    public DocumentFile Output { get; set; } = default;
}

public record DocumentFile(Document Document, Stream FileStream);