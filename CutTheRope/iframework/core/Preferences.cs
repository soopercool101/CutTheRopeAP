using System;
using System.Collections.Generic;
using System.IO;
using CutTheRope.ios;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Storage;

// MonoGame removed Microsoft.Xna.Framework.Storage
// so we used System.IO.File instead of StorageDevice
// it may only works on standalone platforms

namespace CutTheRope.iframework.core
{
	internal class Preferences : NSObject
	{
		private static Dictionary<string, int> data_ = new Dictionary<string, int>();

		private static Dictionary<string, string> dataStrings_ = new Dictionary<string, string>();

		//public static IAsyncResult result;

		public static bool GameSaveRequested = false;

		public override NSObject init()
		{
			if (base.init() == null)
			{
				return null;
			}
			_loadPreferences();
			return this;
		}

		public virtual void setIntforKey(int v, string k, bool comit)
		{
			_setIntforKey(v, k, comit);
		}

		public virtual void setBooleanforKey(bool v, string k, bool comit)
		{
			_setBooleanforKey(v, k, comit);
		}

		public virtual void setStringforKey(string v, string k, bool comit)
		{
			_setStringforKey(v, k, comit);
		}

		public virtual int getIntForKey(string k)
		{
			return _getIntForKey(k);
		}

		public virtual float getFloatForKey(string k)
		{
			return 0f;
		}

		public virtual bool getBooleanForKey(string k)
		{
			return _getBooleanForKey(k);
		}

		public virtual string getStringForKey(string k)
		{
			return _getStringForKey(k);
		}

		public static void _setIntforKey(int v, string key, bool comit)
		{
			int value;
			if (data_.TryGetValue(key, out value))
			{
				data_[key] = v;
			}
			else
			{
				data_.Add(key, v);
			}
			if (comit)
			{
				_savePreferences();
			}
		}

		private static void _setStringforKey(string v, string k, bool comit)
		{
			string value;
			if (dataStrings_.TryGetValue(k, out value))
			{
				dataStrings_[k] = v;
			}
			else
			{
				dataStrings_.Add(k, v);
			}
			if (comit)
			{
				_savePreferences();
			}
		}

		public static int _getIntForKey(string k)
		{
			int value;
			if (data_.TryGetValue(k, out value))
			{
				return value;
			}
			return 0;
		}

		private static float _getFloatForKey(string k)
		{
			return 0f;
		}

		public static bool _getBooleanForKey(string k)
		{
			int num = _getIntForKey(k);
			return num != 0;
		}

		public static void _setBooleanforKey(bool v, string k, bool comit)
		{
			_setIntforKey(v ? 1 : 0, k, comit);
		}

		private static string _getStringForKey(string k)
		{
			string value;
			if (dataStrings_.TryGetValue(k, out value))
			{
				return value;
			}
			return "";
		}

		public virtual void savePreferences()
		{
			_savePreferences();
		}

		public static void _savePreferences()
		{
			try
			{
				if (!GameSaveRequested)
				{
					GameSaveRequested = true;
					//result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
				}
			}
			catch (Exception)
			{
			}
		}

		public static void Update()
		{
			try
			{
				if (!GameSaveRequested/* || !result.IsCompleted*/)
				{
					return;
				}
                /*
				StorageDevice storageDevice = StorageDevice.EndShowSelector(result);
				if (storageDevice != null && storageDevice.IsConnected)
				{
					IAsyncResult asyncResult = storageDevice.BeginOpenContainer("Preferences", null, null);
					asyncResult.AsyncWaitHandle.WaitOne();
					StorageContainer storageContainer = storageDevice.EndOpenContainer(asyncResult);
					asyncResult.AsyncWaitHandle.Close();
					string file = "ctr_save.bin";
					if (storageContainer.FileExists(file))
					{
						storageContainer.DeleteFile(file);
					}
					Stream stream = storageContainer.CreateFile(file);
					SaveToStream(stream);
					stream.Close();
					storageContainer.Dispose();
				}
				*/
                string file = "ctr_save.bin";
                Stream stream = File.Create(file);
                SaveToStream(stream);
                stream.Close();
                GameSaveRequested = false;
			}
			catch (Exception)
			{
				GameSaveRequested = false;
			}
		}

		public static bool SaveToStream(Stream stream)
		{
			bool flag = false;
			try
			{
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				binaryWriter.Write(data_.Count);
				foreach (KeyValuePair<string, int> item in data_)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value);
				}
				binaryWriter.Write(dataStrings_.Count);
				foreach (KeyValuePair<string, string> item2 in dataStrings_)
				{
					binaryWriter.Write(item2.Key);
					binaryWriter.Write(item2.Value);
				}
				binaryWriter.Close();
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				FrameworkTypes._LOG("Error: cannot save, " + ex.ToString());
				return flag;
			}
		}

		public static bool LoadFromStream(Stream stream)
		{
			bool flag = false;
			try
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string key = binaryReader.ReadString();
					int value = binaryReader.ReadInt32();
					data_.Add(key, value);
				}
				num = binaryReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					string key2 = binaryReader.ReadString();
					string value2 = binaryReader.ReadString();
					dataStrings_.Add(key2, value2);
				}
				binaryReader.Close();
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				FrameworkTypes._LOG("Error: cannot load, " + ex.ToString());
				return flag;
			}
		}

		public virtual void loadPreferences()
		{
			_loadPreferences();
		}

		internal static void _loadPreferences()
		{
			/*
			result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
			result.AsyncWaitHandle.WaitOne();
			StorageDevice storageDevice = StorageDevice.EndShowSelector(result);
			if (storageDevice != null && storageDevice.IsConnected)
			{
				IAsyncResult asyncResult = storageDevice.BeginOpenContainer("Preferences", null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer = storageDevice.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				string file = "ctr_save.bin";
				if (storageContainer.FileExists(file))
				{
					Stream stream = storageContainer.OpenFile(file, FileMode.Open);
					LoadFromStream(stream);
					stream.Close();
				}
				storageContainer.Dispose();
			}
			*/
			string file = "ctr_save.bin";
			if (File.Exists(file))
			{
                Stream stream = File.OpenRead(file);
                LoadFromStream(stream);
                stream.Close();
            }
		}
	}
}
