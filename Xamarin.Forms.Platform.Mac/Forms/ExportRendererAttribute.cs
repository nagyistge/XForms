using System;
using AppKit;

namespace Xamarin.Forms
{
	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : HandlerAttribute
	{
		public ExportRendererAttribute (Type handler, Type target) : base (handler, target)
		{
		}

		public override bool ShouldRegister ()
		{
			return true;
		}
	}
}