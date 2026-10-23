using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Storage;

internal sealed class S3ObjectReadStream : Stream
{
	private readonly GetObjectResponse _response;

	public S3ObjectReadStream(GetObjectResponse response)
	{
		_response = response;
	}

	private Stream InnerStream => _response.ResponseStream;

	public override bool CanRead => InnerStream.CanRead;
	public override bool CanSeek => InnerStream.CanSeek;
	public override bool CanWrite => false;

	public override long Length => InnerStream.Length;

	public override long Position
	{
		get => InnerStream.Position;
		set => InnerStream.Position = value;
	}

	public override void Flush() => InnerStream.Flush();

	public override int Read(
		byte[] buffer,
		int offset,
		int count)
		=> InnerStream.Read(buffer, offset, count);

	public override ValueTask<int> ReadAsync(
		Memory<byte> buffer,
		CancellationToken cancellationToken = default)
		=> InnerStream.ReadAsync(buffer, cancellationToken);

	public override Task<int> ReadAsync(
		byte[] buffer,
		int offset,
		int count,
		CancellationToken cancellationToken)
		=> InnerStream.ReadAsync(
			buffer,
			offset,
			count,
			cancellationToken);

	public override long Seek(long offset, SeekOrigin origin)
		=> InnerStream.Seek(offset, origin);

	public override void SetLength(long value)
		=> throw new NotSupportedException();

	public override void Write(
		byte[] buffer,
		int offset,
		int count)
		=> throw new NotSupportedException();

	protected override void Dispose(bool disposing)
	{
		if (disposing)
			_response.Dispose();

		base.Dispose(disposing);
	}

	public override ValueTask DisposeAsync()
	{
		_response.Dispose();
		GC.SuppressFinalize(this);
		return ValueTask.CompletedTask;
	}
}