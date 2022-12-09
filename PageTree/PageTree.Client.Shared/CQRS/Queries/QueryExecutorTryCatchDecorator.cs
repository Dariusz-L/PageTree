﻿using Common.Basic.Blocks;
using Mediator;

namespace PageTree.Client.Shared.CQRS
{
    public sealed class QueryExecutorTryCatchDecorator<TException> : IMQueryExecutor
        where TException : Exception
    {
        private readonly IMQueryExecutor _decorated;
        private readonly Action<TException> _onCatch;

        public QueryExecutorTryCatchDecorator(IMQueryExecutor decorated, Action<TException> onCatch)
        {
            _decorated = decorated;
            _onCatch = onCatch;
        }

        public async Task<Result<TResponse>> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _decorated.Execute(query, cancellationToken);
            }
            catch (TException exception) // AccessTokenNotAvailableException
            {
                _onCatch.Invoke(exception);
                return Result<TResponse>.Failure(exception.Message);
            }
        }

        public async Task<TResponse> ExecuteForDTO<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _decorated.ExecuteForDTO(query, cancellationToken);
            }
            catch (TException exception) // AccessTokenNotAvailableException
            {
                _onCatch.Invoke(exception);
                return default;
            }
        }
    }
}
