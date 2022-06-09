using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Dapper;

namespace PortfolioTracker.DataAccess
{
    public class SqlDatabase
    {
        private const int CommandTimeoutSeconds = 3000;
        private readonly SqlConnection defaultConnection;
        private readonly string connectionString;

        public SqlDatabase(string connectionString)
        {
            defaultConnection = new SqlConnection(connectionString);
            this.connectionString = connectionString;
        }

        public async Task<Maybe<T>> GetSingleOrDefault<T>(
            string query,
            CancellationToken cancellationToken,
            SqlConnection? existingConnection = null,
            DbTransaction? transaction = null,
            object? queryParameters = null)
        {
            var connection = existingConnection ?? new SqlConnection(connectionString);
            if (existingConnection == null)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                    query,
                    queryParameters,
                    transaction,
                    commandType: CommandType.Text,
                    commandTimeout: CommandTimeoutSeconds,
                    cancellationToken: cancellationToken);

            var result = await connection.QuerySingleOrDefaultAsync<T>(command);

            if (existingConnection == null)
                await connection.CloseAsync();

            return result ?? Maybe<T>.None;
        }

        /// <typeparam name="T">Result type</typeparam>
        /// <param name="query"> Sql query to be executed</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="useDefaultConnection">SqlDatabase will use the same(one per instance of a repository)
        /// - default connection for each query, can't be used in parallel</param>
        /// <param name="existingConnection">A connection that managed outside SQL database class</param>
        /// <param name="transaction">Transaction for SQL query</param>
        /// <param name="queryParameters">Query Parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Query<T>(
            string query,
            CancellationToken cancellationToken,
            bool useDefaultConnection = false,
            SqlConnection? existingConnection = null,
            DbTransaction? transaction = null,
            object? queryParameters = null)
        {
            var connection = existingConnection ?? defaultConnection;
            if (existingConnection == null)
            {
                if (!useDefaultConnection)
                    connection = new SqlConnection(connectionString);

                await connection.OpenAsync(cancellationToken);
            }

            var command = new CommandDefinition(
                query,
                queryParameters,
                transaction,
                commandType: CommandType.Text,
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<T>(command);

            if (existingConnection == null)
                await connection.CloseAsync();

            return result;
        }

        public async Task ExecuteNonQuery(
            string query,
            CancellationToken cancellationToken,
            SqlConnection? existingConnection = null,
            DbTransaction? transaction = null,
            object? queryParameters = null)
        {
            var connection = existingConnection ?? new SqlConnection(connectionString);
            if (existingConnection == null)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                query,
                queryParameters,
                transaction,
                commandType: CommandType.Text,
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken);

            await connection.QueryAsync(command);

            if (existingConnection == null)
                await connection.CloseAsync();
        }

        public async Task<TResult> ExecuteScalar<TResult>(
            string query,
            CancellationToken cancellationToken,
            SqlConnection? existingConnection = null,
            DbTransaction? transaction = null,
            object? queryParameters = null)
        {
            var connection = existingConnection ?? new SqlConnection(connectionString);
            if (existingConnection == null)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                query,
                queryParameters,
                transaction,
                commandType: CommandType.Text,
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken);

            var result = await connection.ExecuteScalarAsync<TResult>(command);

            if (existingConnection == null)
                await connection.CloseAsync();

            return result;
        }

        public SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(connectionString);
        }

        public string GetDatabaseName() => defaultConnection.Database;

        public void Dispose()
        {
            defaultConnection.Dispose();
        }
    }
}
