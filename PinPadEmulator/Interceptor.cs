﻿using PinPadEmulator.Devices;
using PinPadEmulator.Extensions;
using System;
using System.Diagnostics;

namespace PinPadEmulator
{
	public class Interceptor : IDisposable
	{
		public event Action<string> Request;
		public event Action<string> Response;

		private DataLink virtualLink;
		private DataLink realLink;

		private IDevice virtualDevice;
		private IDevice realDevice;

		public Interceptor(IDevice virtualDevice, IDevice realDevice)
		{
			if (virtualDevice == null) { throw new ArgumentNullException(nameof(virtualDevice)); }
			if (realDevice == null) { throw new ArgumentNullException(nameof(realDevice)); }

			this.virtualLink = new DataLink();
			this.virtualLink.CommandReceived += this.OnVirtualLinkCommandReceived;

			this.realLink = new DataLink();
			this.realLink.CommandReceived += this.OnRealLinkCommandReceived;

			this.virtualDevice = virtualDevice;
			this.virtualDevice.Output += this.OnVirtualDeviceOutput;

			this.realDevice = realDevice;
			this.realDevice.Output += this.OnRealDeviceOutput;
		}

		private void OnVirtualLinkCommandReceived(string command)
		{
			Debug.WriteLine($"OnVirtualLinkCommandReceived: {command}");
			this.Request?.Invoke(command);
		}

		private void OnRealLinkCommandReceived(string command)
		{
			Debug.WriteLine($"OnRealLinkCommandReceived: {command}");
			this.Response?.Invoke(command);
		}

		private void OnVirtualDeviceOutput(byte[] dataCollection)
		{
			Debug.WriteLine($"OnVirtualDeviceOutput: {dataCollection.ToHexString()}");
			foreach (var data in dataCollection)
			{
				this.virtualLink.Input(data);
			}
			this.realDevice.Input(dataCollection);
		}

		private void OnRealDeviceOutput(byte[] dataCollection)
		{
			Debug.WriteLine($"OnRealDeviceOutput: {dataCollection.ToHexString()}");
			foreach (var data in dataCollection)
			{
				this.realLink.Input(data);
			}
			this.virtualDevice.Input(dataCollection);
		}

		public void Dispose()
		{
			if (this.virtualDevice != null)
			{
				this.virtualDevice.Dispose();
				this.virtualDevice = null;
			}
			if (this.realDevice != null)
			{
				this.realDevice.Dispose();
				this.realDevice = null;
			}
		}
	}
}
