using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Decorators
{
    /// <summary>Defines the ExceptionHandlingContentProviderDecorator API contract.</summary>
    public class ExceptionHandlingContentProviderDecorator : IReadContentProvider
    {
        private readonly IReadContentProvider _inner;

        /// <summary>Performs the ExceptionHandlingContentProviderDecorator operation.</summary>
        public ExceptionHandlingContentProviderDecorator(IReadContentProvider inner)
        {
            _inner = inner;
        }

        /// <summary>Performs the GetById operation asynchronously.</summary>
        public async Task<ContentItem?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _inner.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // wrap or log as needed
                throw new ContentProviderException($"Error fetching {id}", ex);
            }
        }

        /// <summary>Performs the GetAll operation asynchronously.</summary>
        public async Task<IEnumerable<ContentItem>> GetAllAsync()
        {
            try
            {
                return await _inner.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ContentProviderException("Error fetching all content", ex);
            }
        }
    }
}
