using System;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace GeneraterLicens
{
	/// <summary>
	/// Description of Class1.
	/// </summary>

public static class Crypto{
	//Инициализация контейнера
	public static string CreateRSA(){
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
			string s="key\\"+DateTime.Now.ToString().Replace(":",string.Empty).Replace(".",string.Empty);
			RSAParameters r= rsa.ExportParameters(true);
			File.WriteAllText(s+"privatekey", rsa.ToXmlString(true));
			File.WriteAllText(s+"publickey", rsa.ToXmlString(false));
			return s;
		}
	public static string  EncryptData1(string s, string key){
			RSACryptoServiceProvider rs = new RSACryptoServiceProvider();
			rs.FromXmlString(key);
			return Encoding.UTF8.GetString(rs.Encrypt(Encoding.Unicode.GetBytes(s), false)); 
			//return Convert.ToBase64String(rs.Encrypt(Convert.FromBase64String(s), false));
		}
	public static string DecryptoData1(string s, string key){
			RSACryptoServiceProvider rs = new RSACryptoServiceProvider();
			rs.FromXmlString(key);
			//return Encoding.UTF8.GetString(rs.Decrypt(Encoding.UTF8.GetBytes(s), false));
			return Convert.ToBase64String(rs.Decrypt(Convert.FromBase64String(s), false));
		}
	public static string DecryptoData1(string s){
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
			RSACryptoServiceProvider rs = new RSACryptoServiceProvider();
			rs.FromXmlString(rsa.ToXmlString(true));
			//return Encoding.UTF8.GetString(rs.Decrypt(Encoding.UTF8.GetBytes(s), false));
			return Convert.ToBase64String(rs.Decrypt(Convert.FromBase64String(s), false));
		}

        public static string EncryptString(string inputString, string key){
            int dwKeySize = 2048;
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(key);
            int keySize = dwKeySize / 8;
			byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
			int iterations;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Clear();
			if (dataLength > maxLength)
			{
				iterations = dataLength / maxLength;
	
				for (int i = 0; i <= iterations; i++)
				{
					byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
					Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
					byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
					// Обратите внимание, что RSACryptoServiceProvider меняет порядок 
					// зашифрованных байт. Он делает это после шифрования и перед 
					// дешифровкой. Если Вам не нужна совместимость с Microsoft 
					// Cryptographic API (CAPI) или другими поставщиками, то закомментируйте
					// следующую строку и соотвтетствующую строчку в функции DecryptString.
					//Array.Reverse(encryptedBytes);
					// Зачем конвертировать в base 64?
					// Потому что это одно из основных печатных оснований использующих только 
					// символы ASCII
					stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
				}
			}
            else
            {
				byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(bytes, true);
				stringBuilder.Append(Convert.ToBase64String(encryptedBytes));


			}

				return stringBuilder.ToString();
        }

        public static string DecryptString(string inputString, string key) {
            int dwKeySize = 2048;
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(key);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            
			int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                // Обратите внимание, что RSACryptoServiceProvider меняет порядок 
                // зашифрованных байт. Он делает это после шифрования и перед 
                // дешифровкой. Если Вам не нужна совместимость с Microsoft 
                // Cryptographic API (CAPI) или другими поставщиками, то закомментируйте
                // следующую строку и соотвтетствующую строчку в функции EncryptString.
                // Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }
    /*
     public static string Md5Text(string s){
		return Md5Text(Encoding.UTF8.GetBytes(s));
	}
	
	public static string Md5Text(byte[] s){
		MD5 md5Hash=MD5.Create();
		Encoding encoding=Encoding.Default;
		byte[] data=md5Hash.ComputeHash(s);
		StringBuilder sb=new StringBuilder();
		for(int i=0;i<data.Length;i++)
			sb.Append(data[i].ToString("x2"));
		return sb.ToString();
	}
	
	public static string Md5File(string s){
		FileStream fs=System.IO.File.OpenRead(s);
		byte[] fileData= new byte[fs.Length];
		fs.Read(fileData,0, (int)fs.Length);
		return Md5Text(fileData);
	}
	
	 private static uint[] crc_table = new uint[256];
 
      public static void BuildTable(){
      	uint crc;
      	for (uint i = 0; i < 256; i++){
                crc = i;
                for (int j = 0; j < 8; j++)
                    crc = ((crc & 1) == 1) ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
                	// crc = ((crc & 1) == 1) ? (crc >> 1) ^ 0xA833982B : crc >> 1;
                crc_table[i] = crc;
            }
        }
 
      public static string Crc32(byte[] array){
     		BuildTable();
            uint result = 0xFFFFFFFF;
             for (int i = 0; i < array.Length; i++) {
                byte last_byte = (byte)(result & 0xFF);
                result >>= 8;
                result = result ^ crc_table[last_byte ^ array[i]];
            }
            return  String.Format("{0:X}",result);//.ToString("X");
     }
     public static string Crc32(string s){
     	return Crc32(Encoding.UTF32.GetBytes(s));
	}
public static string Crc32File(string s){
	s = File.ReadAllText(s);
	return Crc32(s);
}
	*/
[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int Md5(int len);
public static string MD5dll(string s1){
	int i = CreateMappingFile(s1);
	i = Md5(i);
	return ReadMapping(i);
}


[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int CRC32TXT(int len);
public static string CRC32TXT(string s1){
	int i = CreateMappingFile(s1);
	i = CRC32TXT(i);
	return ReadMapping(i);
}

[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int CRC32TXTFILE(int len);
public static string CRC32TXTFILE(string s1){
	s1 = System.IO.Path.GetFileName(s1);
	int i = CreateMappingFile(s1);
	i = CRC32TXTFILE(i);
	return ReadMapping(i);
}

[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int MD5File(int len);
public static string MD5File(string s1)	{
	s1 = System.IO.Path.GetFileName(s1);
	int i = CreateMappingFile(s1);
	i = MD5File(i);
	return ReadMapping(i);
}
[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int DeCrypto(int len);
public static string DeCrypto(string s1){
int i = CreateMappingFile(s1);
i = DeCrypto(i);
return ReadMapping(i);
}
[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
  private static extern void InitialDll(int i, string bom, long maxbyte);
[DllImport("ProtectCode.dll", CallingConvention = CallingConvention.Cdecl)]
public static extern string SetNameMap(string namemap);
public static void InitialDll(string pdn){
	namemap = "ProtectCode123";
	MaxByteFile = 24576*32;
	DelMapping();
	SetNameMap(namemap);
	int i = CreateMappingFile("pdn");
	InitialDll(1, Encoding.Unicode.GetString(new byte[] { 0, 255, 255, 254 }),MaxByteFile);
}
private static string namemap;
private static int MaxByteFile;
private static int CreateMappingFile(string txt){
	txt = DelBom(txt.Trim(' ', '\n', '\r', '\0')).Trim(' ', '\n', '\r', '\0');
	var mmf = MemoryMappedFile.CreateOrOpen(namemap, MaxByteFile, MemoryMappedFileAccess.ReadWrite);
	MemoryMappedViewStream stream = mmf.CreateViewStream(0, txt.Length * 32);//, MemoryMappedFileAccess.ReadWrite);
	stream.Position = 0;
	StreamWriter strmrdr = new StreamWriter(stream, System.Text.Encoding.GetEncoding(1200));
	strmrdr = new StreamWriter(stream, System.Text.Encoding.GetEncoding(1200));
	strmrdr.BaseStream.Seek(0, SeekOrigin.Begin);
	char[] df = txt.ToCharArray();
	int len = df.Length;
	strmrdr.Write(df, 0, len);
	strmrdr.Flush();
	stream.Dispose();
			//return Encoding.UTF8.GetBytes(txt).Length;
	return txt.Length;
}
		
		private static string ReadMapping(int len)
		{
			var mmf = MemoryMappedFile.CreateOrOpen(namemap, MaxByteFile, MemoryMappedFileAccess.ReadWrite);
			MemoryMappedViewStream stream = mmf.CreateViewStream(0, len * 32);// MaxByteFile, MemoryMappedFileAccess.ReadWrite); 
			StreamReader strmrdr = new StreamReader(stream, System.Text.Encoding.GetEncoding(1200));
			strmrdr.BaseStream.Seek(0, SeekOrigin.Begin);
			string s = strmrdr.ReadToEnd();
			s = s.Substring(0, len);
			mmf.Dispose();
			stream.Dispose();
			stream.Close();
			strmrdr.Close();
			strmrdr.Dispose();
			File.Delete(namemap);
			if (s != " ") s = DelBom(s.Trim(' ', '\n', '\r', '\0')).Trim(' ', '\n', '\r', '\0');
			return s;

		}

		
		public static string DelBom(string s){
	byte[] b=Encoding.Unicode.GetBytes(s);
	int len = s.Length;
	if((b[0]==0)&&(b[1]==255)&&(b[2]==255)&(b[3]==254)){
		b[0]=b[1]=b[2]=b[3]=0;
		return Encoding.Unicode.GetString(b).Trim(new char[] {'\0',' '});
	}
	return s;
}
		private static void DelMapping()
		{
			var mmf = MemoryMappedFile.CreateOrOpen(namemap, MaxByteFile, MemoryMappedFileAccess.ReadWrite);
			mmf.Dispose();
			File.Delete(namemap);

		}
	}
}
