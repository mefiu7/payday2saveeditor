﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace PaydaySaveEditor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		byte[] xorKey = { 0x5E, 0x2E, 0x23, 0x56, 0x25, 0x33, 0x34, 0x39, 0x35, 0x32, 0xF3, 0x32, 0x64, 0xB4, 0x33, 0x25, 0xA4, 0x48, 0xA4, 0x64, 0x32, 0x63, 0x32, 0x63, 0x20, 0x6C, 0x4D, 0x3C };
		FileStream saveFileStream;

		private void btnLoadSave_Click(object sender, EventArgs e)
		{
			Stream saveStream;
			if (openSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					if ((saveStream = openSaveFileDialog.OpenFile()) != null)
					{
						saveStream = new FileStream(openSaveFileDialog.FileName, FileMode.Open, FileAccess.Read);
						transform((FileStream)saveStream, false, "");
						saveStream.Close();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (saveFileStream != null && saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					MessageBox.Show(saveFileDialog.FileName);
					transform(saveFileStream, true, saveFileDialog.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		public void transform(FileStream file, bool save, String filePath)
		{
			byte[] data = new byte[(file.Length - 16)]; // minus 16 because we need to remove the old MD5 footer
			file.Read(data, 0, data.Length);

			int xorOffset;

			for (int i = 0; i < data.Length; i++)
			{
				xorOffset = (i % xorKey.Length);
				data[i] ^= xorKey[xorOffset];
			}

			if (save == true)
			{
				byte[] md5 = MD5.Create().ComputeHash(saveFileStream);

				FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

				newFile.Write(data, 0, data.Length);
				newFile.Write(md5, 0, md5.Length);

				newFile.Close();
			}
			else
			{
				saveFileStream = new FileStream(Application.StartupPath + "/temp.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
				saveFileStream.Write(data, 0, data.Length);
			}
		}
	}
}