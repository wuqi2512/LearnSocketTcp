using System.Collections.Generic;
using System.Text;
using Common;

namespace GameSystem
{
	public class HeartMsg : BaseMessage
	{
		public override int MegId => 1001;

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
