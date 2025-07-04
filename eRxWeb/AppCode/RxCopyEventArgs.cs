using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb
{
/// <summary>
/// Custom EventArgs class for RxClone user control
/// </summary>
public class RxCopyEventArgs : EventArgs
{
	public bool IsSuccessful { get; set; }
	public bool IsCancel { get; set; }
	public string ExceptionReferenceID { get; set; }
	public string Message { get; set; }

	public RxCopyEventArgs()
	{
	}

	public RxCopyEventArgs(bool isSuccessful)
	{
		this.IsSuccessful = isSuccessful;
	}
}
}