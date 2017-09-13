﻿namespace PinPadEmulator.Devices
{
	public class DeviceOutputBuffer : DeviceBuffer
	{
		private readonly IDevice device;

		public DeviceOutputBuffer(IDevice device)
			: base(device)
		{
			this.Device.Output += this.OnDeviceOutput;
		}

		private void OnDeviceOutput(byte[] output)
		{
			lock (this.Buffer)
			{
				foreach (var data in output) { this.Buffer.Enqueue(data); }
			}
		}
	}
}
