using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public static class DatabaseManager
{
	#region Database Data

	private const string FILE_PATH = "URI=file:Assets/Plugins/ProjectOcelotDatabase.s3db";

	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;

	#endregion // Database Data

	#region Public Functions

	/// <summary>
	/// Queries the database.
	/// </summary>
	/// <param name="query"> The SQL style query for the database. </param>
	/// <returns> Returns the data reader for data extraction. </returns>
	public static IDataReader Query ( string query )
	{
		// Connect to the database
		dbConnection = new SqliteConnection ( FILE_PATH );
		dbConnection.Open ( );

		// Create query command
		dbCommand = dbConnection.CreateCommand ( );
		dbCommand.CommandText = query;

		// Read data from the database
		return dbCommand.ExecuteReader ( );
	}

	/// <summary>
	/// Closes the database connection.
	/// Call this function once the data from query has been extracted.
	/// </summary>
	/// <param name="dbReader"> The reader created from the query. </param>
	public static void Close ( IDataReader dbReader )
	{
		// Delete the reader
		dbReader.Close ( );
		dbReader = null;

		// Delete the command
		dbCommand.Dispose ( );
		dbCommand = null;

		// Delete the database connection
		dbConnection.Close ( );
		dbConnection = null;
	}

	#endregion // Public Functions
}
