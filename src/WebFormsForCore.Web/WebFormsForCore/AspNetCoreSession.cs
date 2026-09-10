using System.IO;

namespace System.Web.SessionState;

/// <summary>
/// Bridges classic <see cref="SessionStateStoreProviderBase"/> session state to the
/// ASP.NET Core session (<see cref="Microsoft.AspNetCore.Http.ISession"/>) of the current request,
/// so WebForms session state is backed by whatever session store the host app configured
/// (in-memory, distributed cache, etc.) via services.AddSession()/app.UseSession().
/// </summary>
public class AspNetCoreSessionProvider : SessionStateStoreProviderBase
{
    private const string SessionStateKeyPrefix = "WebFormsForCore.SessionState.";

    // Keyed by the WebForms session id (rather than a single fixed key) so that a WebForms-side
    // id rotation (e.g. session-fixation mitigation, RegenerateExpiredSessionId) starts from an
    // empty item instead of silently inheriting whatever was stored under the previous id.
    private static string GetSessionStateKey(string id) => SessionStateKeyPrefix + id;

    public override void Dispose()
    {
    }

    public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
    {
        // ASP.NET Core session has no expiry notification mechanism to hook Session_End into.
        return false;
    }

    public override void InitializeRequest(HttpContext context)
    {
    }

    public override void EndRequest(HttpContext context)
    {
    }

    public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        return DoGet(context, id, out locked, out lockAge, out lockId, out actions);
    }

    public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        return DoGet(context, id, out locked, out lockAge, out lockId, out actions);
    }

    private static SessionStateStoreData DoGet(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        locked = false;
        lockAge = TimeSpan.Zero;
        lockId = null;
        actions = SessionStateActions.None;

        if (!context.CoreContext.Session.TryGetValue(GetSessionStateKey(id), out byte[] buffer))
        {
            return null;
        }

        using var stream = new MemoryStream(buffer);
        return SessionStateUtility.DeserializeStoreData(context, stream, false);
    }

    public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
    {
    }

    public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
    {
        SessionStateUtility.SerializeStoreData(item, 4096, out byte[] buffer, out int length, false);

        byte[] bytes = buffer;
        if (length != buffer.Length)
        {
            bytes = new byte[length];
            Array.Copy(buffer, bytes, length);
        }

        context.CoreContext.Session.Set(GetSessionStateKey(id), bytes);
    }

    public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
    {
        context.CoreContext.Session.Remove(GetSessionStateKey(id));
    }

    public override void ResetItemTimeout(HttpContext context, string id)
    {
        // ASP.NET Core session expiry is governed globally by SessionOptions.IdleTimeout;
        // there is no per-item timeout to reset.
    }

    public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
    {
        return SessionStateUtility.CreateLegitStoreData(context, null, null, timeout);
    }

    public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
    {
        SessionStateStoreData item = CreateNewStoreData(context, timeout);
        SetAndReleaseItemExclusive(context, id, item, null, true);
    }
}
