/****************************** Module Header ******************************\ 
Module Name:  SqlHelper.cs 
Project:      CSDataSqlCommand 
Copyright (c) Microsoft Corporation. 
 
We can create and execute different types of SqlCommand. In this application,  
we will demonstrate how to create and execute SqlCommand. 
The file contains some methods that set the connection, command and exectute  
the command. 
 
This source is subject to the Microsoft Public License. 
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL. 
All other rights reserved. 
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebApplication1.Helper
{
	public static class SqlHelper
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["Autotask"].ConnectionString;

		public static DataSet SelectRows(DataSet dataset, string queryString)
		{
			using (SqlConnection connection =
				new SqlConnection(connectionString))
			{
				SqlDataAdapter adapter = new SqlDataAdapter();
				adapter.SelectCommand = new SqlCommand(
					queryString, connection);
				connection.Open();
				adapter.Fill(dataset);
				return dataset;
			}
		}

		/// <summary> 
		/// Set the connection, command, and then execute the command with non query. 
		/// </summary> 
		public static Int32 ExecuteNonQuery(String commandText,
			CommandType commandType, params SqlParameter[] parameters)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(commandText, conn))
				{
					// There're three command types: StoredProcedure, Text, TableDirect. The TableDirect  
					// type is only for OLE DB.   
					cmd.CommandType = commandType;
					cmd.Parameters.AddRange(parameters);

					conn.Open();
					return cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary> 
		/// Set the connection, command, and then execute the command and only return one value. 
		/// </summary> 
		public static Object ExecuteScalar(String commandText,
			CommandType commandType, params SqlParameter[] parameters)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(commandText, conn))
				{
					cmd.CommandType = commandType;
					cmd.Parameters.AddRange(parameters);

					conn.Open();
					return cmd.ExecuteScalar();
				}
			}
		}

		/// <summary> 
		/// Set the connection, command, and then execute the command with query and return the reader. 
		/// </summary> 
		public static SqlDataReader ExecuteReader(String commandText,
			CommandType commandType, params SqlParameter[] parameters)
		{
			SqlConnection conn = new SqlConnection(connectionString);

			using (SqlCommand cmd = new SqlCommand(commandText, conn))
			{
				cmd.CommandType = commandType;
				cmd.Parameters.AddRange(parameters);

				conn.Open();
				// When using CommandBehavior.CloseConnection, the connection will be closed when the  
				// IDataReader is closed. 
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

				return reader;
			}
		}
	}
}