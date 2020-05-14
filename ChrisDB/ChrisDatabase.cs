using System;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ChrisDB.Helpers;
using System.Collections.Generic;

namespace ChrisDB
{
	public class ChrisDatabase : IDisposable
	{
		private readonly string filePath;
		private string privateKey;
		private string publicKey;
		private RSACryptoServiceProvider enc;
		private bool encrypted;

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

		public async Task ExecuteCreate(string query)
		{
			List<string> regexGroups = StringHelpers.GetMatchesAndGroups(query, "(?:CREATE )((?:[A-Z]|[a-z])(?:[A-Z]|[a-z]|[0-9])*) (.*)")[0];

			var tableName = regexGroups[0];
			var parameters = regexGroups[1];

			
		}

		public void Dispose()
		{
			if (enc != null)
				enc.Dispose();
		}
	}
}
