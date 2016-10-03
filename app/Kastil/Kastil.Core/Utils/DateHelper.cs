using System;
namespace Kastil.Core.Utils
{
	public static class DateHelper
	{
		public static DateTimeOffset? AsDateTimeOffset(this long utcTicks)
		{
			if (utcTicks == 0)
				return null;
			return new DateTimeOffset(utcTicks, TimeSpan.Zero);
		}

		public static long AsUtcTicks(this DateTimeOffset? dateTimeOffset)
		{
			if (dateTimeOffset == null)
				return 0;
			return dateTimeOffset.Value.UtcTicks;
		}
	}
}
