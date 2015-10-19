using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;



public class AsynchronousSocketListener
{
	// Thread signal.     
	public static ManualResetEvent allDone = new ManualResetEvent(false);
	public static bool stop = false;
	public AsynchronousSocketListener()
	{
	}
	public static void StartListening()
	{
		// Data buffer for incoming data.     
		// Establish the local endpoint for the socket.     
		// The DNS name of the computer     
		// running the listener is "host.contoso.com".     
		//IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
		//IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
		// Create a TCP/IP socket.     
		Socket listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
		// Bind the socket to the local     
		//endpoint and listen for incoming connections.     
		try
		{
			listener.Bind(localEndPoint);
			listener.Listen(100);
			while (!stop)
			{
				// Set the event to nonsignaled state.     
				allDone.Reset();
				// Start an asynchronous socket to listen for connections.     
				LogMgr.Log("Waiting for a connection...");
				listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);
				// Wait until a connection is made before continuing.     
				allDone.WaitOne();
			}
		}
		catch (Exception e)
		{
			LogMgr.LogError(e);
		}

	}
	public static void AcceptCallback(IAsyncResult ar)
	{
//		LogMgr.Log("Server AcceptCallback ");
		// Signal the main thread to continue.     
		allDone.Set();
		// Get the socket that handles the client request.     
		Socket listener = (Socket)ar.AsyncState;
		Socket handler = listener.EndAccept(ar);
		// Create the state object.     
		StateObject state = new StateObject();
		state.workSocket = handler;
		handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
	}
	public static void ReadCallback(IAsyncResult ar)
	{


		String content = String.Empty;
		// Retrieve the state object and the handler socket     
		// from the asynchronous state object.     
		StateObject state = (StateObject)ar.AsyncState;
		Socket handler = state.workSocket;
		// Read data from the client socket.     
		int bytesRead = handler.EndReceive(ar);

//		LogMgr.Log("Server ReadCallback  bytesRead ="+ bytesRead);
		if (bytesRead > 0)
		{
			// There might be more data, so store the data received so far.     
//			state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
//			// Check for end-of-file tag. If it is not there, read     
//			// more data.     
//			content = state.sb.ToString();
//
//			LogMgr.LogError("Read "+ content);

			byte[] bydata =new byte[bytesRead];
			System.Array.Copy(state.buffer,0,bydata,0,bytesRead );

			Send(handler, bydata);
			if (content.IndexOf("<EOF>") > -1)
			{
				// All the data has been read from the     
				// client. Display it on the console.     

				// Echo the data back to the client.     

			}
			else
			{
				// Not all data received. Get more.     
				handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
			}
		}
	}

	private static void Send(Socket handler, byte[] byteData)
	{
		// Convert the string data to byte data using ASCII encoding.     
		// Begin sending the data to the remote device.     
		handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
	}
	private static void Send(Socket handler, String data)
	{
		// Convert the string data to byte data using ASCII encoding.     
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		// Begin sending the data to the remote device.     
		handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
	}
	private static void SendCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.     
			Socket handler = (Socket)ar.AsyncState;
			// Complete sending the data to the remote device.     
			int bytesSent = handler.EndSend(ar);
			LogMgr.Log("Server Send "+ bytesSent + "bytes to client");

//			handler.Shutdown(SocketShutdown.Both);
//			handler.Close();
		}
		catch (Exception e)
		{
			LogMgr.LogError(e);
		}
	}
}