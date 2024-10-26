using System.Collections.Generic;
using System.Text;
using Common;

namespace GamePlay
{
	public class PlayerMsg : BaseMessage
	{
		public override int MegId => 2001;
		public int PlayerId;
		public PlayerData Data;

		public override int GetByteCount()
		{
			int num = 0;
			num += 4;
			num += Data.GetByteCount();
			return num;
		}


		public override void Write(byte[] bytes, ref int index)
		{
			WriteInt(bytes, ref index, PlayerId);
			WriteIData(bytes, ref index, Data);
		}


		public override void Read(byte[] bytes, ref int index)
		{
			int temp = 0;
			PlayerId = ReadInt(bytes, ref index);
			Data = ReadIData<PlayerData>(bytes, ref index);
		}

	}
}
