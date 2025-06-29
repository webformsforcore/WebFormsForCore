#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Hosting;
using Microsoft.Win32.SafeHandles;
using Microsoft.Extensions.Primitives;
using System.Security.Principal;
using Microsoft.AspNetCore;
using Core = Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace System.Web.Hosting
{
	public class AspNetCoreWorkerRequest : SimpleWorkerRequest, IAspNetCoreWorkerRequest
	{
		private const int MaxChunkLength = 64 * 1024;

		private const int MaxHeaderBytes = 32 * 1024;

		private static readonly char[] BadPathChars = new[] { '%', '>', '<', ':', '\\' };

		// TODO read default files from web.config
		private static readonly string[] DefaultFileNames = new[] { "default.aspx", "default.htm", "default.html" };

		private TaskCompletionSource<bool> Completed = new TaskCompletionSource<bool>();

		private AutoResetEvent CompletedEvent = new AutoResetEvent(false);

		private static readonly char[] IntToHex = new[]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
			};

		private static readonly string[] RestrictedDirs = new[]
			{
				"/bin",
				"/appbrowsers",
				"/appcode",
				"/appdata",
				"/applocalresources",
				"/appglobalresources",
				"/appwebreferences"
			};

		public AspNetCoreHost Host { get; private set; }

		private string allRawHeaders;

		private byte[] body;

		private int bodyLength;

		private int contentLength;

		// security permission to Assert remoting calls to connection
		private int endHeadersOffset;

		private string filePath;

		private byte[] headerBytes;

		private List<ByteString> headerByteStrings;

		private bool headersSent;

		// parsed request data

		private bool isClientScriptPath;

		private string[] knownRequestHeaders;

		private string path;

		private string pathInfo;

		private string pathTranslated;

		private string protocol;

		private string queryString;
		private byte[] queryStringBytes;

		private List<byte[]> responseBodyBytes;

		private StringBuilder responseHeadersBuilder;

		private int responseStatus;

		private bool specialCaseStaticFileHeaders;

		private int startHeadersOffset;

		private string[][] unknownRequestHeaders;

		private string url;

		private string verb;

		public Core.HttpContext Context { get; private set; }
		public AspNetCoreWorkerRequest(AspNetCoreHost host, Core.HttpContext context)
			: base(string.Empty, string.Empty, null)
		{
			this.Host = host;
			Context = context;
		}

		public override void CloseConnection()
		{
			Context.Connection.RequestClose();
		}

		public override void EndOfRequest()
		{
#if DebugWF4C
			var path = Context.Request.Path;
			Debug.WriteLine($"EndOfRequestStart {path}");
#endif
			try
			{
				if (Context != null && Context.Response != null)
				{
					var task = Context.Response.CompleteAsync();
					if (task != null)
					{
						task.ContinueWith(t =>
							{
#if DebugWF4C
								Debug.WriteLine($"EndOfRequest {path}");
#endif
								if (t.Exception != null) Completed.SetException(t.Exception);
								else Completed.SetResult(true);
							},
							CancellationToken.None,
							TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
							TaskScheduler.Default);
					}
					else
					{
#if DebugWF4C
						Debug.WriteLine($"EndOfRequest Fail three {path}");
#endif
						Completed.SetResult(true);
					}
				} else
				{
#if DebugWF4C
					Debug.WriteLine($"EndOfRequest Fail {path}");
#endif
					Completed.SetResult(true);
				}
			}
			catch (Exception ex)
			{
#if DebugWF4C
				Debug.WriteLine($"EndOfRequest Fail two {path}");
#endif
				//Completed.SetException(ex);
				Completed.SetResult(true);
			}
		}

		public override void FlushResponse(bool finalFlush)
		{
			using (var noSyncContext = new SafeAsync())
			{
				Context.Response.Body.Flush();
				if (finalFlush) Context.Response.Body.Close();
			}
		}

		public override string GetAppPath() => Host.VirtualPath;
		public override string GetAppPathTranslated() => Host.PhysicalPath;
		public override string GetFilePath()
		{
			ParseRequest();
			return filePath;
		}
		public override string GetFilePathTranslated()
		{
			ParseRequest();
			return pathTranslated;
		}
		public override string GetHttpVerbName() => Context.Request.Method;
		public override string GetHttpVersion() => Context.Request.Protocol;
		public override string GetKnownRequestHeader(int index) => knownRequestHeaders[index];
		public override string GetLocalAddress() => Context.Connection.LocalIpAddress.ToString();
		public override int GetLocalPort() => Context.Connection.LocalPort;
		public override string GetPathInfo() => pathInfo;
		public override byte[] GetPreloadedEntityBody() => null;
		public override string GetQueryString() {
			var str = Context.Request.QueryString.ToString();
			if (str.StartsWith("?")) return str.Substring(1);
			else return str;
		}
		public override byte[] GetQueryStringRawBytes() => Encoding.ASCII.GetBytes(GetQueryString());
		public override string GetRawUrl()
		{
			var query = GetQueryString();
			if (string.IsNullOrEmpty(query)) return path;
			return $"{path}?{query}";
		}
		public override string GetRemoteAddress() => Context.Connection.RemoteIpAddress.ToString();
		public override int GetRemotePort() => Context.Connection.RemotePort;
		public override string GetServerName()
		{
			string localAddress = GetLocalAddress();
			if (localAddress.Equals("127.0.0.1") || localAddress.Equals("::1"))
			{
				return "localhost";
			}
			return localAddress;
		}

		public override string GetServerVariable(string name)
		{
			string processUser = string.Empty;
			string str2 = name;
			if (str2 == null)
			{
				return processUser;
			}
			if (str2 != "ALLRAW")
			{
				if (str2 != "SERVERPROTOCOL")
				{
					if (str2 == "LOGONUSER")
					{
						if (GetUserToken() != IntPtr.Zero)
						{
							processUser = Host.GetProcessUser();
						}
						return processUser;
					}
					if ((str2 == "AUTHTYPE") && (GetUserToken() != IntPtr.Zero))
					{
						processUser = "NTLM";
					}
					return processUser;
				}
			}
			else
			{
				return Context.Request.Headers.ToString();
			}
			return protocol;
		}

		public override string GetUnknownRequestHeader(string name)
		{
			int n = unknownRequestHeaders.Length;

			for (int i = 0; i < n; i++)
			{
				if (string.Compare(name, unknownRequestHeaders[i][0], StringComparison.OrdinalIgnoreCase) == 0)
				{
					return unknownRequestHeaders[i][1];
				}
			}

			return null;
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			return unknownRequestHeaders;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		// Implementation of HttpWorkerRequest

		public override string GetUriPath()
		{
			var request = Context.Request;
			if (string.IsNullOrEmpty(request.Path)) return request.PathBase;
			else return $"{request.PathBase}/{request.Path}";
		}

		public override IntPtr GetUserToken()
		{
			return Host.GetProcessToken();
		}

		public string GetProcessUser()
		{
			return Host.GetProcessUser();
		}

		public override string GetAppPoolID()
		{
			return Host.PhysicalPath.GetHashCode().ToString("X");
		}

		public override bool HeadersSent()
		{
			return headersSent;
		}

		public override bool IsClientConnected() => true;
		public override bool IsEntireEntityBodyIsPreloaded() => false;
		public override string MapPath(string path)
		{
			string mappedPath;
			bool isClientScriptPath;

			if (string.IsNullOrEmpty(path) || path.Equals("/"))
			{
				// asking for the site root
				mappedPath = Host.VirtualPath == "/" ? Host.PhysicalPath : Environment.SystemDirectory;
			}
			else if (Host.IsVirtualPathAppPath(path))
			{
				// application path
				mappedPath = Host.PhysicalPath;
			}
			else if (Host.IsVirtualPathInApp(path, out isClientScriptPath))
			{
				if (isClientScriptPath)
				{
					mappedPath = Path.Combine(Host.PhysicalClientScriptPath,
								 path.Substring(Host.NormalizedClientScriptPath.Length));
				}
				else
				{
					// inside app but not the app path itself
					mappedPath = Path.Combine(Host.PhysicalPath, path.Substring(Host.NormalizedVirtualPath.Length));
				}
			}
			else
			{
				// outside of app -- make relative to app path
				if (path.StartsWith("/", StringComparison.Ordinal))
				{
					mappedPath = Path.Combine(Host.PhysicalPath, path.Substring(1));
				}
				else
				{
					mappedPath = Path.Combine(Host.PhysicalPath, path);
				}
			}

			mappedPath = mappedPath.Replace('/', Path.DirectorySeparatorChar);

			if (mappedPath.EndsWith("\\", StringComparison.Ordinal) &&
				!mappedPath.EndsWith(":\\", StringComparison.Ordinal))
			{
				mappedPath = mappedPath.Substring(0, mappedPath.Length - 1);
			}

			return mappedPath;
		}

		[AspNetHostingPermission(SecurityAction.Assert, Level = AspNetHostingPermissionLevel.Medium)]
		public async Task Process()
		{
			// read the request
			if (!TryParseRequest())
			{
				return;
			}

			if (!Host.RequireAuthentication || true)
			{
				// deny access to code, bin, etc.
				if (IsRequestForRestrictedDirectory())
				{
					Context.Response.StatusCode = 403;
					Context.Response.CompleteAsync();
					return;
				}

				PrepareResponse();

				// Hand the processing over to HttpRuntime
				// Run processing in separate ASP.NET Worker Thread
				//var completed = Completed.Task;
				await Task.Factory.StartNew(() =>
					{
						Thread.CurrentThread.Name = $"ASP.NET WorkerThread {Context.Request.Path}";
#if DebugWF4C
						Debug.WriteLine($"Start ProcessRequest {Context.Request.Path}");
#endif
						HttpRuntime.ProcessRequest(this);
#if DebugWF4C
						Debug.WriteLine($"End ProcessRequest {Context.Request.Path}");
#endif
					},
					TaskCreationOptions.LongRunning);

#if DebugWF4C
				Debug.WriteLine($"Waiting {Context.Request.Path} to finish");
#endif
				await Completed.Task;
#if DebugWF4C
				Debug.WriteLine($"Request {Context.Request.Path} finished");
#endif
			}
		}


		public override int ReadEntityBody(byte[] buffer, int size) => ReadEntityBody(buffer, 0, size);
		public override int ReadEntityBody(byte[] buffer, int offset, int size)
		{
			if (offset < 0) throw new ArgumentOutOfRangeException("offset must be grater than zero.");

			int bytesRead = 0;

			using (var noSyncContext = new SafeAsync())
			{
				var reader = Context.Request.Body;

				bytesRead = reader.Read(buffer, 0, size);
			}
			return bytesRead;
		}

		public override void SendCalculatedContentLength(int contentLength)
		{
			if (!headersSent)
			{
				Context.Response.ContentLength = contentLength;
			}
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			if (headersSent)
			{
				return;
			}

			switch (index)
			{
				case HeaderServer:
				case HeaderDate:
				case HeaderConnection:
					// ignore these
					return;
				case HeaderAcceptRanges:
					// FIX: #14359
					if (value != "bytes")
					{
						// use this header to detect when we're processing a static file
						break;
					}
					specialCaseStaticFileHeaders = true;
					return;

				case HeaderExpires:
				case HeaderLastModified:
					// FIX: #14359
					if (!specialCaseStaticFileHeaders)
					{
						// NOTE: Ignore these for static files. These are generated
						//       by the StaticFileHandler, but they shouldn't be.
						break;
					}
					return;


				// FIX: #12506
				case HeaderContentType:

					string contentType = null;

					if (value == "application/octet-stream")
					{
						// application/octet-stream is default for unknown so lets
						// take a shot at determining the type.
						// don't do this for other content-types as you are going to
						// end up sending text/plain for endpoints that are handled by
						// asp.net such as .aspx, .asmx, .axd, etc etc
						//contentType = CommonExtensions.GetContentType(pathTranslated);
					}
					value = contentType ?? value;
					Context.Response.ContentType = value;
					return;
			}

			SendUnknownResponseHeader(GetKnownResponseHeaderName(index), value);
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			if (length == 0)
			{
				return;
			}

			FileStream f = null;
			try
			{
				f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
				SendResponseFromFileStream(f, offset, length);
			}
			finally
			{
				if (f != null)
				{
					f.Close();
				}
			}
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			if (length == 0)
			{
				return;
			}

			using (var sfh = new SafeFileHandle(handle, false))
			{
				using (var f = new FileStream(sfh, FileAccess.Read))
				{
					SendResponseFromFileStream(f, offset, length);
				}
			}
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			if (length > 0)
			{
				using (var noSyncContext = new SafeAsync())
				{
					Context.Response.Body.Write(data, 0, length);
				}
			}
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			// TODO description
			Context.Response.StatusCode = statusCode;
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			if (headersSent)
				return;

			if (Context.Response.Headers.ContainsKey(name)) {
				var header = Context.Response.Headers[name];
				Context.Response.Headers[name] = StringValues.Concat(header, value);
			} else
			{
				Context.Response.Headers[name] = value;
			}
		}

		private bool IsBadPath()
		{
			ParseRequest();

			if (path.IndexOfAny(BadPathChars) >= 0)
			{
				return true;
			}

			if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(path, "..", CompareOptions.Ordinal) >= 0)
			{
				return true;
			}

			if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(path, "//", CompareOptions.Ordinal) >= 0)
			{
				return true;
			}

			return false;
		}

		private bool IsRequestForRestrictedDirectory()
		{
			String p = CultureInfo.InvariantCulture.TextInfo.ToLower(path);

			if (Host.VirtualPath != "/")
			{
				p = p.Substring(Host.VirtualPath.Length);
			}

			foreach (String dir in RestrictedDirs)
			{
				if (p.StartsWith(dir, StringComparison.Ordinal))
				{
					if (p.Length == dir.Length || p[dir.Length] == '/')
					{
						return true;
					}
				}
			}

			return false;
		}

		private void ParseHeaders()
		{
			knownRequestHeaders = new string[RequestHeaderMaximum];

			// construct unknown headers as array list of name1,value1,...
			var headers = new List<string>();


			foreach (var header in Context.Request.Headers)
			{
				string name = header.Key;
				var strValues = header.Value;
				string str;
				switch (strValues.Count)
				{
					case 0:
						str = (string)null;
						break;
					case 1:
						str = strValues[0];
						break;
					default:
						str = string.Join(';', strValues);
						break;
				}

				// remember
				int knownIndex = GetKnownRequestHeaderIndex(name);
				if (knownIndex >= 0)
				{
					knownRequestHeaders[knownIndex] = str;
				}
				else
				{
					headers.Add(name);
					headers.Add(str);
				}
			}

			// append AspFilterSessionId
            var path = Context.Request.Path.Value ?? string.Empty;
            // Optimize for the common case where there is no cookie
            if (path.IndexOf('(') != -1)
            {
                int endPos = path.LastIndexOf(")/", StringComparison.Ordinal);
                int startPos = (endPos > 2 ? path.LastIndexOf("/(", endPos - 1, endPos, StringComparison.Ordinal) : -1);
                if (startPos < 0) // pattern not found: common case, exit immediately
                    return;

                if (IsValidHeader(path, startPos + 2, endPos))
                {
                    var sessionHeader = path.Substring(startPos + 2, endPos - startPos - 2);
                    headers.Add("AspFilterSessionId");
                    headers.Add(sessionHeader);
                }
            }

            // copy to array unknown headers

			int n = headers.Count / 2;
			unknownRequestHeaders = new string[n][];
			int j = 0;

			for (int i = 0; i < n; i++)
			{
				unknownRequestHeaders[i] = new string[2];
				unknownRequestHeaders[i][0] = headers[j++];
				unknownRequestHeaders[i][1] = headers[j++];
			}
		}

        // Make sure sub-string if of the pattern: A(XXXX)N(XXXXX)P(XXXXX) and so on.
        static private bool IsValidHeader(string path, int startPos, int endPos)
        {
            if (endPos - startPos < 3) // Minimum len is "X()"
                return false;

            while (startPos <= endPos - 3) { // Each iteration deals with one "A(XXXX)" pattern

                if (path[startPos] < 'A' || path[startPos] > 'Z') // Make sure pattern starts with a capital letter
                    return false;

                if (path[startPos + 1] != '(') // Make sure next char is '('
                    return false;

                startPos += 2;
                bool found = false;
                for (; startPos < endPos; startPos++) { // Find the ending ')'

                    if (path[startPos] == ')') { // found it!
                        startPos++; // Set position for the next pattern
                        found = true;
                        break; // Break out of this for-loop.
                    }

                    if (path[startPos] == '/') { // Can't contain path separaters
                        return false;
                    }
                }
                if (!found)  {
                    return false; // Ending ')' not found!
                }
            }

            if (startPos < endPos) // All chars consumed?
                return false;

            return true;
        }

		private void ParsePostedContent()
		{
			contentLength = 0;
			bodyLength = 0;

			string contentLengthValue = knownRequestHeaders[HeaderContentLength];
			if (contentLengthValue != null)
			{
				try
				{
					contentLength = Int32.Parse(contentLengthValue, CultureInfo.InvariantCulture);
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch
				// ReSharper restore EmptyGeneralCatchClause
				{
				}
			}

			/*
			if (headerBytes.Length > endHeadersOffset)
			{
				bodyLength = headerBytes.Length - endHeadersOffset;

				if (bodyLength > contentLength)
				{
					bodyLength = contentLength; // don't read more than the content-length
				}

				if (bodyLength > 0)
				{
					body = new byte[bodyLength];
					Buffer.BlockCopy(headerBytes, endHeadersOffset, body, 0, bodyLength);
					//connection.LogRequestBody(body);
				}
			} */
		}

		bool requestParsed = false;
		private void ParseRequest()
		{
			if (!requestParsed)
			{
				path = $"{Context.Request.PathBase}{Context.Request.Path}";

				int lastDot = path.LastIndexOf('.');
				int lastSlh = path.LastIndexOf('/');

				if (lastDot >= 0 && lastSlh >= 0 && lastDot < lastSlh)
				{
					int ipi = path.LastIndexOf('/', lastDot);
					filePath = path.Substring(0, ipi);
					pathInfo = path.Substring(ipi);
				}
				else
				{
					filePath = path;
					pathInfo = String.Empty;
				}

				pathTranslated = MapPath(filePath);

				requestParsed = true;
			}
		}

		private void PrepareResponse()
		{
			Context.Response.Headers.Clear();
			//Context.Response.Cookies.Clear();
		}

		private void Reset()
		{
			headerBytes = null;
			startHeadersOffset = 0;
			endHeadersOffset = 0;
			headerByteStrings = null;

			isClientScriptPath = false;

			verb = null;
			url = null;
			protocol = null;

			path = null;
			filePath = null;
			pathInfo = null;
			pathTranslated = null;
			queryString = null;
			queryStringBytes = null;

			contentLength = 0;
			bodyLength = 0;
			body = null;

			allRawHeaders = null;
			unknownRequestHeaders = null;
			knownRequestHeaders = null;
			specialCaseStaticFileHeaders = false;
		}

		private void SendResponseFromFileStream(Stream f, long offset, long length)
		{
			long fileSize = f.Length;

			if (length == -1)
			{
				length = fileSize - offset;
			}

			if (length == 0 || offset < 0 || length > fileSize - offset)
			{
				return;
			}

			if (offset > 0)
			{
				f.Seek(offset, SeekOrigin.Begin);
			}

			using (var noSyncContext = new SafeAsync())
			{
				if (length <= MaxChunkLength)
				{
					var fileBytes = ArrayPool<byte>.Shared.Rent((int)length);
					try
					{
						int bytesRead = f.Read(fileBytes, 0, (int)length);
						//SendResponseFromMemory(fileBytes, bytesRead);
						Context.Response.Body.Write(fileBytes, 0, bytesRead);
					}
					finally
					{
						ArrayPool<byte>.Shared.Return(fileBytes);
					}
				}
				else
				{
					var chunk = ArrayPool<byte>.Shared.Rent(MaxChunkLength);
					try
					{
						var bytesRemaining = (int)length;

						while (bytesRemaining > 0)
						{
							int bytesToRead = (bytesRemaining < MaxChunkLength) ? bytesRemaining : MaxChunkLength;
							int bytesRead = f.Read(chunk, 0, bytesToRead);

							//SendResponseFromMemory(chunk, bytesRead);
							Context.Response.Body.Write(chunk, 0, bytesRead);
							bytesRemaining -= bytesRead;

							// flush to release keep memory
							if ((bytesRemaining > 0) && (bytesRead > 0))
							{
								//FlushResponse(false);
								Context.Response.Body.Flush();
							}
						}
					}
					finally
					{
						ArrayPool<byte>.Shared.Return(chunk);
					}
				}
			}
		}

		private void SkipAllPostedContent()
		{
			if ((contentLength > 0) && (bodyLength < contentLength))
			{
				byte[] buffer = new byte[1024];
				for (int i = contentLength - bodyLength; i > 0; i -= buffer.Length)
				{
					var nread = Context.Request.Body.Read(buffer, 0, Math.Min(i, buffer.Length));
					if ((buffer == null) || (nread == 0))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// TODO: defer response until request is written
		/// </summary>
		/// <returns></returns>
		private bool TryParseRequest()
		{
			Reset();

			ParseRequest();

			// Check for bad path
			if (IsBadPath())
			{
				Context.Response.StatusCode = 400;
				Context.Response.CompleteAsync();
				return false;
			}

			// Check if the path is not well formed or is not for the current app
			if (!Host.IsVirtualPathInApp(path, out isClientScriptPath))
			{
				Context.Response.StatusCode = 404;
				Context.Response.CompleteAsync();
				return false;
			}

			ParseHeaders();

			ParsePostedContent();

			return true;
		}

		/*private static string UrlEncodeRedirect(string path)
		{
			// this method mimics the logic in HttpResponse.Redirect (which relies on internal methods)

			// count non-ascii characters
			byte[] bytes = Encoding.UTF8.GetBytes(path);
			int count = bytes.Length;
			int countNonAscii = 0;
			for (int i = 0; i < count; i++)
			{
				if ((bytes[i] & 0x80) != 0)
				{
					countNonAscii++;
				}
			}

			// encode all non-ascii characters using UTF-8 %XX
			if (countNonAscii > 0)
			{
				// expand not 'safe' characters into %XX, spaces to +s
				var expandedBytes = new byte[count + countNonAscii * 2];
				int pos = 0;
				for (int i = 0; i < count; i++)
				{
					byte b = bytes[i];

					if ((b & 0x80) == 0)
					{
						expandedBytes[pos++] = b;
					}
					else
					{
						expandedBytes[pos++] = (byte)'%';
						expandedBytes[pos++] = (byte)IntToHex[(b >> 4) & 0xf];
						expandedBytes[pos++] = (byte)IntToHex[b & 0xf];
					}
				}

				path = Encoding.ASCII.GetString(expandedBytes);
			}

			// encode spaces into %20
			if (path.IndexOf(' ') >= 0)
			{
				path = path.Replace(" ", "%20");
			}

			return path;
		}*/

		#region Nested type: ByteParser

		internal class ByteParser
		{
			private readonly byte[] bytes;

			private int pos;

			public ByteParser(byte[] bytes)
			{
				bytes = bytes;
				pos = 0;
			}

			public int CurrentOffset
			{
				get { return pos; }
			}

			public ByteString ReadLine()
			{
				ByteString line = null;

				for (int i = pos; i < bytes.Length; i++)
				{
					if (bytes[i] == (byte)'\n')
					{
						int len = i - pos;
						if (len > 0 && bytes[i - 1] == (byte)'\r')
						{
							len--;
						}

						line = new ByteString(bytes, pos, len);
						pos = i + 1;
						return line;
					}
				}

				if (pos < bytes.Length)
				{
					line = new ByteString(bytes, pos, bytes.Length - pos);
				}

				pos = bytes.Length;
				return line;
			}
		}

		#endregion

		#region Nested type: ByteString

		internal class ByteString
		{
			private readonly byte[] bytes;

			private readonly int length;

			private readonly int offset;

			public ByteString(byte[] bytes, int offset, int length)
			{
				bytes = bytes;
				offset = offset;
				length = length;
			}

			public byte[] Bytes
			{
				get { return bytes; }
			}

			public bool IsEmpty
			{
				get { return (bytes == null || length == 0); }
			}

			public byte this[int index]
			{
				get { return bytes[offset + index]; }
			}

			public int Length
			{
				get { return length; }
			}

			public int Offset
			{
				get { return offset; }
			}

			public byte[] GetBytes()
			{
				var bytes = new byte[length];
				if (length > 0) Buffer.BlockCopy(bytes, offset, bytes, 0, length);
				return bytes;
			}

			public string GetString(Encoding enc)
			{
				if (IsEmpty) return string.Empty;
				return enc.GetString(bytes, offset, length);
			}

			public string GetString()
			{
				return GetString(Encoding.UTF8);
			}

			public int IndexOf(char ch)
			{
				return IndexOf(ch, 0);
			}

			public int IndexOf(char ch, int offset)
			{
				for (int i = offset; i < length; i++)
				{
					if (this[i] == (byte)ch) return i;
				}
				return -1;
			}

			public ByteString[] Split(char sep)
			{
				var list = new List<ByteString>();

				int pos = 0;
				while (pos < length)
				{
					int i = IndexOf(sep, pos);
					if (i < 0)
					{
						break;
					}

					list.Add(Substring(pos, i - pos));
					pos = i + 1;

					while (this[pos] == (byte)sep && pos < length)
					{
						pos++;
					}
				}

				if (pos < length)
					list.Add(Substring(pos));

				return list.ToArray();
			}

			public ByteString Substring(int offset, int len)
			{
				return new ByteString(bytes, offset + offset, len);
			}

			public ByteString Substring(int offset)
			{
				return Substring(offset, length - offset);
			}
		}

		#endregion
	}
}
#endif