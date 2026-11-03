using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace LearnCSharp.Concurrency;

/// <summary>
/// A bounded producer/consumer pipeline exposed as an async stream. Stage 4 explains
/// it in depth; the starter uses it to stream the roadmap from /api/roadmap.
/// </summary>
public static class ChannelPipeline
{
    public static async IAsyncEnumerable<T> StreamAsync<T>(
        IEnumerable<T> source,
        int capacity = 4,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Channel<T> channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = true,
        });

        // The linked source lets the consumer stop the producer when it leaves early.
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task producer = Task.Run(() => ProduceAsync(source, channel.Writer, stop.Token), CancellationToken.None);

        try
        {
            await foreach (T item in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return item;
            }
        }
        finally
        {
            await stop.CancelAsync();
            await producer;
        }
    }

    private static async Task ProduceAsync<T>(IEnumerable<T> source, ChannelWriter<T> writer, CancellationToken token)
    {
        try
        {
            foreach (T item in source)
            {
                // Waits (without blocking a thread) while the channel is full: backpressure.
                await writer.WriteAsync(item, token);
            }

            writer.TryComplete();
        }
        catch (Exception ex)
        {
            // Completing with the exception makes the reader rethrow it.
            writer.TryComplete(ex);
        }
    }
}
