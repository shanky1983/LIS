using System;
using System.Collections.Generic;

namespace Attune.Kernel.Build
{
	/// <summary>
	/// Class that stores information for tables in a database.
	/// </summary>
	public class Table {
		string name;
		List<Column> columns;
		List<Column> primaryKeys;
		Dictionary<string, List<Column>> foreignKeys;
		
		/// <summary>
		/// Default constructor.  All collections are initialized.
		/// </summary>
		public Table() {
			columns = new List<Column>();
			primaryKeys = new List<Column>();
			foreignKeys = new Dictionary<string, List<Column>>();
		}
		
		/// <summary>
		/// Contains the list of Column instances that define the table.
		/// </summary>
		public List<Column> Columns {
			get { return columns; }
		}

		/// <summary>
		/// Contains the list of Column instances that define the table.  The Dictionary returned 
		/// is keyed on the foreign key name, and the value associated with the key is an 
		/// List of Column instances that compose the foreign key.
		/// </summary>
		public Dictionary<string, List<Column>> ForeignKeys {
			get { return foreignKeys; }
		}

		/// <summary>
		/// Name of the table.
		/// </summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Contains the list of primary key Column instances that define the table.
		/// </summary>
		public List<Column> PrimaryKeys {
			get { return primaryKeys; }
		}
	}
}
