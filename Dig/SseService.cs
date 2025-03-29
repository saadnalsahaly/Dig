using System.Collections.Concurrent;

namespace Dig;

public class SseService<T>
{
    private readonly ConcurrentQueue<T> _updates = new();
    private readonly List<TaskCompletionSource<T>> _listeners = new();
    private bool _isUpdateBeingSent = false;

    public void AddUpdate(T update)
    {
        // Add the new update to the queue
        _updates.Enqueue(update);

        // Notify listeners only if no update is already being sent
        if (!_isUpdateBeingSent)
        {
            ProcessUpdates();
        }
    }

    private async void ProcessUpdates()
    {
        // Set flag indicating that an update is being sent
        _isUpdateBeingSent = true;

        // Process all updates in the queue
        while (_updates.TryDequeue(out var update))
        {
            // Notify each listener
            lock (_listeners)
            {
                foreach (var listener in _listeners)
                {
                    listener.TrySetResult(update);
                }
                _listeners.Clear();
            }
        }

        // Reset flag once all updates have been sent
        _isUpdateBeingSent = false;
    }

    public async Task<T> WaitForUpdateAsync(CancellationToken cancellationToken)
    {
        // First, check if there are any updates in the queue
        if (_updates.TryDequeue(out var update))
        {
            // If there is an update available, return it immediately
            return update;
        }

        // If no updates are available, wait for a new update
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Add this listener to the listeners list
        lock (_listeners)
        {
            _listeners.Add(tcs);
        }

        // Use cancellation token to handle task cancellation if needed
        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            // Wait until the listener is notified
            return await tcs.Task;
        }
    }
}