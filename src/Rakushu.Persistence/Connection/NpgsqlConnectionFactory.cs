using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Connection;

internal sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
	private readonly string _connectionString;

	public NpgsqlConnectionFactory(IConfiguration configuration)
	{
		_connectionString =
			configuration.GetConnectionString("DefaultConnection") 
			?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
	}

	public DbConnection CreateConnection()
	{
		return new NpgsqlConnection(_connectionString);
	}
}