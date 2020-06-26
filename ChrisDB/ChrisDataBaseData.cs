using System.Collections.Generic;

namespace ChrisDB
{
	public class ChrisDataBaseData
	{
		public IReadOnlyList<string> PartitionKey { get; }
		public IReadOnlyList<string> ClusteringKey { get; }
		public IReadOnlyDictionary<string, object> Properties { get; }

		public ChrisDataBaseData(object[] partition, object[] clustering, IReadOnlyDictionary<string, object> props)
		{
			PartitionKey = partition;
			ClusteringKey = clustering;
			Properties = props;
		}


	}
}
