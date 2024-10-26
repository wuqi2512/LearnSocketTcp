using System.Collections.Generic;
using System.Text;
using Common;

namespace GamePlay
{
	public class StringMsg : BaseMessage
	{
		public override int MegId => 2002;
		public string Str;

		public override int GetByteCount()
		{
			int num = 0;
			num += Encoding.UTF8.GetByteCount(Str);
			return num;
		}


		public override void Write(byte[] bytes, ref int index)
		{
			WriteString(bytes, ref index, Str);
		}


		public override void Read(byte[] bytes, ref int index)
		{
			int temp = 0;
			Str = ReadString(bytes, ref index);
		}

	}
}
