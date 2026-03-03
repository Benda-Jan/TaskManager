namespace TaskManager.Domain.Exceptions;

public sealed class NotFoundException(string entityName, object key)
    : DomainException($"{entityName} '{key}' was not found.");
