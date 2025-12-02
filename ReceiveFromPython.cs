using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Python.Runtime;

public partial class ReceiveFromPython : Node2D
{
	// Thread thread;
	// public string IP = "127.0.0.1";
	// public int connectionPort = 25001;
	// IPAddress localAddr;
	// TcpListener server;
	// TcpClient client;
	string receivedData;

	public override void _Ready()
	{
		// Runtime.PythonDLL = @"C:\Users\Gabriel\AppData\Local\Programs\Python\python38.dll";
		// ThreadStart threadStart = new ThreadStart(GetInfo);
		// thread = new Thread(threadStart);
		// thread.Start();
		// System.Diagnostics.Process.Start("python", "product/QLearnAgent.py");
		//ExecutePythonCode(@"print(""hello world python"")");
	}

	public static void init(){
		System.Environment.SetEnvironmentVariable("PATH", @"C:\Users\Gabriel\AppData\Local\Programs\Python" +System.Environment.GetEnvironmentVariable("PATH"));
		System.Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\Users\Gabriel\AppData\Local\Programs\Python");
		System.Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\Users\Gabriel\AppData\Local\Programs\Python\python38.dll");
		PythonEngine.Initialize();
	}

	public static void ExecutePythonCode(string code){
		// System.Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\Users\Gabriel\AppData\Local\Programs\Python");
		// System.Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\Users\Gabriel\AppData\Local\Programs\Python\python38.dll");
		// PythonEngine.Initialize();
		init();
		using (Py.GIL()){
			// dynamic testMod = Py.Import("QLearnAgent");
			// dynamic test = testMod.Test();
			// string result = test.test();
			PythonEngine.RunSimpleString(code);
		}
		PythonEngine.Shutdown();
	}
	public void _Process(){
		// GD.Print(receivedData);
	}

	// void GetInfo(){
	// 	try{
	// 		localAddr = IPAddress.Parse(IP);
	// 		server = new TcpListener(IPAddress.Any, connectionPort);
	// 		server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
	// 		server.Start();
	// 		client = server.AcceptTcpClient();

	// 		SendAndReceiveData();
	// 		GD.Print(receivedData);

	// 	} catch (Exception e){
	// 		GD.Print(e);
	// 	}
	// }

	// void SendAndReceiveData(){
	// 	NetworkStream stream = client.GetStream();
	// 	byte[] buffer = new byte[client.ReceiveBufferSize];
	// 	int bytes = stream.Read(buffer, 0, buffer.Length);
	// 	receivedData = Encoding.UTF8.GetString(buffer, 0, bytes);
	// 	if (receivedData != null){
	// 		// print received data
	// 		GD.Print(receivedData);

	// 		// send data back to python
	// 		byte[] writeBuffer = Encoding.ASCII.GetBytes("CONFIRM: Data received");
	// 		stream.Write(writeBuffer, 0, writeBuffer.Length);
	// 	}
	// }
}
