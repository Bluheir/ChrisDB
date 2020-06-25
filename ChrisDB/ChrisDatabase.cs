using System;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ChrisDB.Helpers;
using System.Linq;
using System.Collections.Generic;
using static ChrisDB.Helpers.StringHelpers;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

namespace ChrisDB
{
	public class ChrisDatabase : IDisposable
	{
		private readonly string filePath;
		private string privateKey;
		private string publicKey;
		private RSACryptoServiceProvider enc;
		private bool encrypted;
		private Dictionary<string, CDBTableType> tables;

		private const string regex1 = "{(.*)};\\s*key=((?:(?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*,?)*);" +
									 "\\s*(?:(?:clustering=((?:(?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*,?)*))?;" +
									"\\s*(?:orderby=((?:desc)|(?:asc)))?;\\s*)?";


		public string FilePath { get => filePath; }

		public bool IsEncrypted {
			get
			{
				return encrypted;
			}
		}
		public ChrisDatabase(string filepath)
		{
			filePath = filepath;
		}
		public Task WriteAsync(string data, string location, Encoding enc = null)
		{
			return WriteAsync((enc ?? Encoding.UTF8).GetBytes(data), location);
		}
		public async Task WriteAsync(byte[] data, string location)
		{
			using (var fs = new FileStream(location, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, data.Length, true))
			{
				await fs.WriteAsync(data, 0, data.Length);
			}
		}
		
		public async Task LoadTablesAsync()
		{
			string tables = filePath + "/tables.json";

			if (!File.Exists(tables))
			{
				await WriteAsync("{}", tables);
			}

			this.tables = JsonConvert.DeserializeObject<Dictionary<string, CDBTableType>>(File.ReadAllText(tables));
		}
		public async Task SaveTablesAsync(bool indented = false)
		{
			string tables = filePath + "/tables.json";
			await WriteAsync(JsonConvert.SerializeObject(this.tables, indented ? Formatting.Indented : Formatting.None), tables);
		}
		public (string publickey, string privatekey) SetPrivateKey(string key = null)
		{
			if (enc == null)
				enc = new RSACryptoServiceProvider();

			encrypted = true;

			if (key != null)
			{
				enc.ImportParameters(KeyToParam(key));
			}
			else
			{
				enc = new RSACryptoServiceProvider(2048);
			}

			string pbkey = ParamToKey(enc.ExportParameters(false));
			string pvkey = ParamToKey(enc.ExportParameters(true));
			privateKey = pvkey;

			return (pbkey, pvkey);
		}

		public byte[] Encrypt(byte[] bytes)
		{
			if (encrypted == false)
				return null;

			return enc.Encrypt(bytes, false);
		}
		public byte[] Decrypt(byte[] cypher)
		{
			if (encrypted == false)
				return null;
			
			return enc.Decrypt(cypher, false);
		}
		public static RSAParameters KeyToParam(string key)
		{
			var sr = new System.IO.StringReader(key);
			var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
			
			return (RSAParameters)xs.Deserialize(sr);
		}
		public static string ParamToKey(RSAParameters param)
		{
			var sw = new System.IO.StringWriter();
			var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

			xs.Serialize(sw, param);
			return sw.ToString();
		}
		public CDBTableType ParseCreate(string query)
		{
			List<string> regexGroups = StringHelpers.GetMatchesAndGroups(query, $"(?:CREATE )((?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*)\\s{regex1}")[0];

			var tableName = regexGroups[0];
			var properties = ToProperties(regexGroups[1]);
			var partition = new HashSet<string>(regexGroups[2].Split(','));
			var clustering = new HashSet<string>(regexGroups[3].Split(','));
			var ascordesc = regexGroups[4];

			return new CDBTableType(tableName, properties, partition, clustering, ascordesc);
		}
		public async Task<string> ExecuteCreateAsync(CDBTableType table, bool indented = false)
		{
			if (tables.ContainsKey(table.TableName))
				return "There is already a table of that name.";
		
			tables.Add(table.TableName, table);
			await SaveTablesAsync(indented);
			return "Success.";
		}
		//public async Task<string> ExecuteInsertAsyc()
		//{
			
	//	}
		private static Dictionary<string, CDBPropertyType> ToProperties(string value)
		{
			var matches = GetMatchesAndGroups(value, "((?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*)\\s*:\\s*((?:string\\??)|(?:int\\??)|(?:byte\\??)|(?:short\\??)|(?:long\\??)|(?:object)|(?:array\\??)|(dictionary\\??))(?:,?)");
			Dictionary<string, CDBPropertyType> retVal = new Dictionary<string, CDBPropertyType>();

			foreach(var i in matches)
			{
				var name = i[0];
				var t = i[1];
				var type = t.TrimEnd('?');
				

				CDBPropertyType ctype = default;

				switch (type)
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

				if (t.EndsWith("?"))
					ctype |= CDBPropertyType.Nullable;

				retVal.Add(name, ctype);
			}
			return retVal;
		}
		
			

		public void Dispose()
		{
			if (enc != null)
				enc.Dispose();
		}
	}
}
