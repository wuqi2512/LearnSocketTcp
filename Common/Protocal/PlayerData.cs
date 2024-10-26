using System.Collections.Generic;
using System.Text;
using Common;

namespace GamePlay
{
	public class PlayerData : IData
	{
		public int TestInt;
		public float TestFloat;
		public List<int> TestList;
		public int[] TestArray;
		public Dictionary<int, string> TestDic;
		public Camp TestEnum;

		public override int GetByteCount()
		{
			int num = 0;
			num += 4;
			num += 4;
			num += 4;
			foreach (var item in TestList)
				num += 4;
			num += 4;
			foreach (var item in TestArray)
				num += 4;
			num += 4;
			foreach (var pair in TestDic)
				num += 4 + Encoding.UTF8.GetByteCount(pair.Value);
			num += 4;
			return num;
		}


		public override void Write(byte[] bytes, ref int index)
		{
			WriteInt(bytes, ref index, TestInt);
			WriteFloat(bytes, ref index, TestFloat);
			WriteInt(bytes, ref index, TestList.Count);
			foreach (var item in TestList)
				WriteInt(bytes, ref index, item);
			WriteInt(bytes, ref index, TestArray.Length);
			foreach (var item in TestArray)
				WriteInt(bytes, ref index, item);
			WriteInt(bytes, ref index, TestDic.Count);
			foreach (var pair in TestDic)
			{
				WriteInt(bytes, ref index, pair.Key);
				WriteString(bytes, ref index, pair.Value);
			}
			WriteInt(bytes, ref index, (int)TestEnum);
		}


		public override void Read(byte[] bytes, ref int index)
		{
			int temp = 0;
			TestInt = ReadInt(bytes, ref index);
			TestFloat =  ReadFloat(bytes, ref index);
			TestList = new List<int>();
			temp = ReadInt(bytes, ref index);
			for (int i = 0; i < temp; i++)
				TestList.Add(ReadInt(bytes, ref index));
			temp = ReadInt(bytes, ref index);
			TestArray = new int[temp];
			for (int i = 0; i < temp; i++)
				TestArray[i] = ReadInt(bytes, ref index);
			TestDic = new Dictionary<int, string>();
			temp = ReadInt(bytes, ref index);
			for (int i = 0; i < temp; i++)
				TestDic.Add(ReadInt(bytes, ref index), ReadString(bytes, ref index));
			TestEnum = (Camp)ReadInt(bytes, ref index);
		}

	}
}
