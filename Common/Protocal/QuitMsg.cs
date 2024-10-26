using System.Collections.Generic;
using System.Text;
using Common;

namespace GameSystem
{
	public class QuitMsg : BaseMessage
	{
		public override int MegId => 1002;

		public override int GetByteCount()
		{
			int num = 0;
			return num;
		}


		public override void Write(byte[] bytes, ref int index)
		{
		}


		public override void Read(byte[] bytes, ref int index)
		{
			int temp = 0;
		}

	}
}
