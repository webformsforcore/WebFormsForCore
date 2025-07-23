using System.IO;

namespace Microsoft.Reporting.WebForms;

internal sealed class MemoryThenTempStorageStream : Stream
{
	private const int m_threshold = 65536;

	private ITemporaryStorage m_tempStorage;

	private Stream m_storageStream = new MemoryStream();

	private bool m_thresholdReached;

	private bool m_isClosed;

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override long Length => m_storageStream.Length;

	public override long Position
	{
		get
		{
			return m_storageStream.Position;
		}
		set
		{
			m_storageStream.Position = value;
		}
	}

	public MemoryThenTempStorageStream(ITemporaryStorage storage)
	{
		m_tempStorage = storage;
	}

	public override void Flush()
	{
		m_storageStream.Flush();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing && !m_isClosed)
		{
			m_storageStream.Close();
			m_isClosed = true;
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return m_storageStream.Read(buffer, offset, count);
	}

	public override int ReadByte()
	{
		return m_storageStream.ReadByte();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return m_storageStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		if (!m_thresholdReached && value > 65536)
		{
			ThresholdReached();
		}
		if (m_storageStream != null)
		{
			m_storageStream.SetLength(value);
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (!m_thresholdReached && Position + count > 65536)
		{
			ThresholdReached();
		}
		if (m_storageStream != null)
		{
			m_storageStream.Write(buffer, offset, count);
		}
	}

	public override void WriteByte(byte value)
	{
		if (!m_thresholdReached && Position >= 65536)
		{
			ThresholdReached();
		}
		if (m_storageStream != null)
		{
			m_storageStream.WriteByte(value);
		}
	}

	private void ThresholdReached()
	{
		Stream stream = m_tempStorage.CreateTemporaryStream();
		if (stream != null)
		{
			if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
			{
				throw new InvalidTemporaryStorageStreamException();
			}
			m_storageStream.Position = 0L;
			byte[] array = new byte[m_storageStream.Length];
			m_storageStream.Read(array, 0, array.Length);
			stream.Write(array, 0, array.Length);
			m_storageStream.Close();
			m_storageStream = stream;
			m_thresholdReached = true;
		}
	}
}
