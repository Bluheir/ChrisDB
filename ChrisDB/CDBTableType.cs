using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace ChrisDB
{
	public class CDBTableType
	{
		[JsonProperty("properties")]
		private Dictionary<string, string> propertiesString;
		[JsonProperty("partition_key")]
		private HashSet<string> partitionKey;
		[JsonProperty("clustering_key")]
		private HashSet<string> clusteringKey;

		[JsonIgnore]
		public IReadOnlyDictionary<string, CDBPropertyType> Properties { get; }
		[JsonIgnore]
		public IReadOnlyDictionary<string, string> PropertiesString => propertiesString;
		[JsonIgnore]
		public IReadOnlyList<string> PartitionKey => (IReadOnlyList<string>)partitionKey;
		[JsonIgnore]
		public IReadOnlyList<string> ClusteringKey => (IReadOnlyList<string>)clusteringKey;
		[JsonProperty("asc_or_desc")]
		public string AscOrDesc { get; private set; }
		[JsonProperty("table_name")]
		public string TableName { get; private set; }
		[JsonProperty("max_partitions_per_file")]
		public int MaxPartitionsPerFile { get; set; }

		public CDBTableType(string tableName, Dictionary<string, string> properties, IEnumerable<string> partition, IEnumerable<string> clustering, string ascorDesc)
		{
			TableName = tableName;
			propertiesString = properties;
			Properties = new Dictionary<string, CDBPropertyType>();
			foreach(var item in properties)
			{
				CDBPropertyType ctype = default;
				var key = item.Key.TrimEnd('?');

				switch (key)
				{
					case "string":
						ctype = CDBPropertyType.String;
						break;
					case "int":
						ctype = CDBPropertyType.Int32;
						break;
					case "byte":
						ctype = CDBPropertyType.Int8;
						break;
					case "short":
						ctype = CDBPropertyType.Int16;
						break;
					case "long":
						ctype = CDBPropertyType.Int64;
						break;
					case "object":
						ctype = CDBPropertyType.Object;
						break;
					case "array":
						ctype = CDBPropertyType.Array;
						break;
					case "dictionary":
						ctype = CDBPropertyType.Dictionary;
						break;
				}

				if (item.Key.EndsWith("?"))
					ctype |= CDBPropertyType.Nullable;
			}

			partitionKey = new HashSet<string>(partition);
			clusteringKey = new HashSet<string>(clustering);

			AscOrDesc = ascorDesc;
		}
		public CDBTableType(string tableName, Dictionary<string, CDBPropertyType> properties, IEnumerable<string> partition, IEnumerable<string> clustering, string ascorDesc = null)
		{
			TableName = tableName;
			propertiesString = FromProperties(properties);
			Properties = properties;

			partitionKey = new HashSet<string>(partition);
			clusteringKey = new HashSet<string>(clustering);

			AscOrDesc = AscOrDesc;
		}
		[JsonConstructor]
		private CDBTableType() { }

		private static Dictionary<string, string> FromProperties(Dictionary<string, CDBPropertyType> properties)
		{
			Dictionary<string, string> retVal = new Dictionary<string, string>();
			foreach(var item in properties)
			{
				CDBPropertyType a = item.Value;
				bool nullable = false;
				string type = "";
				
				if((int)a > 128)
				{
					a -= CDBPropertyType.Nullable;
					nullable = true;
				}

				if(a == CDBPropertyType.Array)
				{
					type = "array";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.Dictionary)
				{
					type = "dictionary";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.Int16)
				{
					type = "short";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.Int32)
				{
					type = "int";
					if (nullable)
						type += "?";
				}
				else if(a== CDBPropertyType.Int64)
				{
					type = "long";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.Int8)
				{
					type = "byte";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.Object)
				{
					type = "object";
					if (nullable)
						type += "?";
				}
				else if(a == CDBPropertyType.String)
				{
					type = "string";
					if (nullable)
						type += "?";
				}

				retVal.Add(item.Key, type);
			}

			return retVal;
		}
	}
}