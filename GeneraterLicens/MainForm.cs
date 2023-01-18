/*
 * Создано в SharpDevelop.
 * Пользователь: Пользователь
 * Дата: 12.03.2021
 * Время: 19:01
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;

using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace GeneraterLicens
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Loadkey(listBox1);
			Loadkey(listBox2);
			Crypto.InitialDll("mhgbbjh");
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button3Click(object sender, EventArgs e){
			ReadKey(Crypto.CreateRSA());
			Loadkey(listBox1);
			Loadkey(listBox2);
		}
		private void ReadKey(string s){
			richTextBox3.Text=File.ReadAllText(s+"privatekey", Encoding.UTF8);
			richTextBox2.Text=File.ReadAllText(s+"publickey", Encoding.UTF8);
			
			
		}
		private void Loadkey(ListBox lstbx){			
			lstbx.Items.Clear();
			string [] s=Directory.GetFiles("key\\");
			List<string> ListKey=new List<string>();
			for(int i=0;i<s.Length;i+=2){
				lstbx.Items.Add(s[i].Substring(4,15));
			}
	
		}
		void ListBox1SelectedIndexChanged(object sender, EventArgs e){
			if(listBox1.SelectedIndex>-1)
			ReadKey("key\\"+listBox1.Items[listBox1.SelectedIndex]);
		}
		void Button4Click(object sender, EventArgs e){
			if(saveFileDialog1.ShowDialog()==DialogResult.OK){
				File.AppendAllText(saveFileDialog1.FileName+".publickey", richTextBox2.Text);
			}
		}
		void Button6Click(object sender, EventArgs e){
			textBox1.Text=Crypto.MD5dll(richTextBox4.Text);
			textBox2.Text=Crypto.CRC32TXT(richTextBox4.Text);
	
		}
		void Button5Click(object sender, EventArgs e){
			if(openFileDialog1.ShowDialog()==DialogResult.OK){
				textBox1.Text=Crypto.MD5File(openFileDialog1.FileName);
				textBox2.Text=Crypto.CRC32TXTFILE(openFileDialog1.FileName);
			}
	
		}
		void Button7Click(object sender, EventArgs e){
			if(saveFileDialog1.ShowDialog()==DialogResult.OK){
			File.AppendAllText(saveFileDialog1.FileName+".privatekey","<RSAKeyValue>"+richTextBox3.Text.Substring(richTextBox3.Text.IndexOf("<P>"),richTextBox3.Text.Length-richTextBox3.Text.IndexOf("<P>")));
			}
		}
		void Button1Click(object sender, EventArgs e){
			if(openFileDialog1.ShowDialog()==DialogResult.OK){
				richTextBox1.Text = Crypto.DelBom(File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8));
				
				
			}
	
		}

void ListBox2SelectedIndexChanged(object sender, EventArgs e){
		//	try{

				richTextBox5.Text = "";
				richTextBox6.Text = "";
				ReadKey("key\\"+listBox2.Items[listBox2.SelectedIndex]);
				string s=richTextBox1.Text.Substring(0,richTextBox1.Text.IndexOf("#<RSAKeyValue><Modulus>"));
				string ss=richTextBox1.Text.Substring(richTextBox1.Text.IndexOf("<RSAKeyValue><Modulus>"),richTextBox1.Text.Length-richTextBox1.Text.IndexOf("<RSAKeyValue><Modulus>"));
				s= Crypto.DecryptString(s,richTextBox3.Text);
				s = Crypto.DeCrypto(s).Trim(new char[] { '\0', ' ' });
				s = Crypto.DelBom(s).Trim(new char[] { '\0', ' ' });
				string[] s1=s.Split(new char[] {'#'},StringSplitOptions.RemoveEmptyEntries);
				int a=s1.Length-1;
				string[] s2=File.ReadAllLines("listmd5.txt");
				string md5dll="";
				string md5exe="";
				for(int i=0;i<s2.Length;i++){
				if (s2[i].Length > 6)
				{
					string[] s3 = s2[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (s3[0] == s1[3]) md5dll = s3[1];
					else if (s3[2] == s1[3]) md5dll = s3[3];
					if (s3[0] == s1[4]) md5exe = s3[1];
					else if (s3[2] == s1[4]) md5exe = s3[3];
				}
				
				}
			if (md5exe == "") {
				label7.Text = "No correct crc exe";

			} else if (md5dll == "")
			{
					label7.Text = "No correct crc dll";
			}
				string str = md5exe + md5dll + s;
				str = Crypto.MD5dll(str);
			   richTextBox5.Text = s + ss;
				ss = Crypto.EncryptString(str, ss);
				richTextBox6.Text = ss;
			//}catch{
		//		label7.Text="error encoding";
		//	}
		}
		void Button2Click(object sender, EventArgs e){
	
		}

        private void button8_Click(object sender, EventArgs e){
			if (saveFileDialog1.ShowDialog() == DialogResult.OK){
				if (File.Exists(saveFileDialog1.FileName + ".lic")) File.Delete(saveFileDialog1.FileName + ".lic");
				File.AppendAllText(saveFileDialog1.FileName + ".lic",richTextBox6.Text );
			}
		}
void Button9Click(object sender, EventArgs e){
	byte[] b=Encoding.Unicode.GetBytes(textBox3.Text);
	byte[] c=Encoding.UTF32.GetBytes(textBox3.Text);
	byte[] d=Encoding.ASCII.GetBytes(textBox3.Text);
	byte[] r=Encoding.UTF8.GetBytes(textBox3.Text);
	byte[] f=Encoding.UTF7.GetBytes(textBox3.Text);
	textBox4.Text="";
	textBox5.Text="";
	textBox6.Text="";
	textBox7.Text="";
	textBox8.Text="";
	for(int i=0;i<b.Length;i++)textBox4.Text+=b[i].ToString("X2")+" ";
	for(int i=0;i<c.Length;i++)textBox5.Text+=c[i].ToString("X2")+" ";
	for(int i=0;i<d.Length;i++)textBox6.Text+=d[i].ToString("X2")+" ";
	for(int i=0;i<r.Length;i++)textBox7.Text+=r[i].ToString("X2")+" ";
	for(int i=0;i<f.Length;i++)textBox8.Text+=f[i].ToString("X2")+" ";
	textBox3.Text=Encoding.UTF32.GetString(c);
	
}
void Button10Click(object sender, EventArgs e){
if (openFileDialog1.ShowDialog() == DialogResult.OK){
	if (File.Exists("projects//" + System.IO.Path.GetFileName(openFileDialog1.FileName))) File.Delete("projects//" + System.IO.Path.GetFileName(openFileDialog1.FileName));
	System.IO.File.Copy(openFileDialog1.FileName, "projects//" + System.IO.Path.GetFileName(openFileDialog1.FileName), true);
	textBox1.Text = Crypto.MD5File("projects//" + System.IO.Path.GetFileName(openFileDialog1.FileName));
	textBox2.Text = Crypto.CRC32TXTFILE("projects//" + System.IO.Path.GetFileName(openFileDialog1.FileName));
}
			/*
string s=Crypto.CRC32TXTFILE("ProtectCode\\ProtectCode.dll");
s+=" "+Crypto.MD5File("ProtectCode\\ProtectCode.dll");
s+=" "+Crypto.CRC32TXTFILE("ProtectCode\\ProtectCode.exe");
s+=" "+Crypto.MD5File("ProtectCode\\ProtectCode.exe")+ "\r\n";
File.WriteAllText("listmd5.txt",s);
			*/
		}
    }
}
