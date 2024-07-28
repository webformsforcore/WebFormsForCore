
using System.ComponentModel;
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Represents the <see cref="T:System.Net.HttpListener" /> timeouts element in the configuration file. This class cannot be inherited.</summary>
  public sealed class HttpListenerTimeoutsElement : ConfigurationElement
  {
    private static ConfigurationPropertyCollection properties;
    private static readonly ConfigurationProperty entityBody = HttpListenerTimeoutsElement.CreateTimeSpanProperty(nameof (entityBody));
    private static readonly ConfigurationProperty drainEntityBody = HttpListenerTimeoutsElement.CreateTimeSpanProperty(nameof (drainEntityBody));
    private static readonly ConfigurationProperty requestQueue = HttpListenerTimeoutsElement.CreateTimeSpanProperty(nameof (requestQueue));
    private static readonly ConfigurationProperty idleConnection = HttpListenerTimeoutsElement.CreateTimeSpanProperty(nameof (idleConnection));
    private static readonly ConfigurationProperty headerWait = HttpListenerTimeoutsElement.CreateTimeSpanProperty(nameof (headerWait));
    private static readonly ConfigurationProperty minSendBytesPerSecond = new ConfigurationProperty(nameof (minSendBytesPerSecond), typeof (long), (object) 0L, (TypeConverter) null, (ConfigurationValidatorBase) new HttpListenerTimeoutsElement.LongValidator(), ConfigurationPropertyOptions.None);

    static HttpListenerTimeoutsElement()
    {
      HttpListenerTimeoutsElement.properties = new ConfigurationPropertyCollection();
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.entityBody);
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.drainEntityBody);
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.requestQueue);
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.idleConnection);
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.headerWait);
      HttpListenerTimeoutsElement.properties.Add(HttpListenerTimeoutsElement.minSendBytesPerSecond);
    }

    private static ConfigurationProperty CreateTimeSpanProperty(string name)
    {
      return new ConfigurationProperty(name, typeof (TimeSpan), (object) TimeSpan.Zero, (TypeConverter) null, (ConfigurationValidatorBase) new HttpListenerTimeoutsElement.TimeSpanValidator(), ConfigurationPropertyOptions.None);
    }

    /// <summary>Gets the time, in seconds, allowed for the request entity body to arrive.</summary>
    /// <returns>The time, in seconds, allowed for the request entity body to arrive.</returns>
    [ConfigurationProperty("entityBody", DefaultValue = 0, IsRequired = false)]
    public TimeSpan EntityBody => (TimeSpan) this[HttpListenerTimeoutsElement.entityBody];

    /// <summary>Gets the time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to drain the entity body on a Keep-Alive connection.</summary>
    /// <returns>The time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to drain the entity body on a Keep-Alive connection.</returns>
    [ConfigurationProperty("drainEntityBody", DefaultValue = 0, IsRequired = false)]
    public TimeSpan DrainEntityBody => (TimeSpan) this[HttpListenerTimeoutsElement.drainEntityBody];

    /// <summary>Gets the time, in seconds, allowed for the request to remain in the request queue before the <see cref="T:System.Net.HttpListener" /> picks it up.</summary>
    /// <returns>The time, in seconds, allowed for the request to remain in the request queue before the <see cref="T:System.Net.HttpListener" /> picks it up.</returns>
    [ConfigurationProperty("requestQueue", DefaultValue = 0, IsRequired = false)]
    public TimeSpan RequestQueue => (TimeSpan) this[HttpListenerTimeoutsElement.requestQueue];

    /// <summary>Gets the time, in seconds, allowed for an idle connection.</summary>
    /// <returns>The time, in seconds, allowed for an idle connection.</returns>
    [ConfigurationProperty("idleConnection", DefaultValue = 0, IsRequired = false)]
    public TimeSpan IdleConnection => (TimeSpan) this[HttpListenerTimeoutsElement.idleConnection];

    /// <summary>Gets the time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to parse the request header.</summary>
    /// <returns>The time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to parse the request header.</returns>
    [ConfigurationProperty("headerWait", DefaultValue = 0, IsRequired = false)]
    public TimeSpan HeaderWait => (TimeSpan) this[HttpListenerTimeoutsElement.headerWait];

    /// <summary>Gets the minimum send rate, in bytes-per-second, for the response.</summary>
    /// <returns>The minimum send rate, in bytes-per-second, for the response.</returns>
    [ConfigurationProperty("minSendBytesPerSecond", DefaultValue = 0, IsRequired = false)]
    public long MinSendBytesPerSecond
    {
      get => (long) this[HttpListenerTimeoutsElement.minSendBytesPerSecond];
    }

		protected internal override ConfigurationPropertyCollection Properties => properties;

		internal long[] GetTimeouts()
    {
      return new long[6]
      {
        Convert.ToInt64(this.EntityBody.TotalSeconds),
        Convert.ToInt64(this.DrainEntityBody.TotalSeconds),
        Convert.ToInt64(this.RequestQueue.TotalSeconds),
        Convert.ToInt64(this.IdleConnection.TotalSeconds),
        Convert.ToInt64(this.HeaderWait.TotalSeconds),
        this.MinSendBytesPerSecond
      };
    }

    private class TimeSpanValidator : ConfigurationValidatorBase
    {
      public override bool CanValidate(Type type) => type == typeof (TimeSpan);

      public override void Validate(object value)
      {
        TimeSpan actualValue = (TimeSpan) value;
        long int64 = Convert.ToInt64(actualValue.TotalSeconds);
        if (int64 < 0L || int64 > (long) ushort.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (value), (object) actualValue, SR.GetString("ArgumentOutOfRange_Bounds_Lower_Upper", (object) "0:0:0", (object) "18:12:15"));
      }
    }

    private class LongValidator : ConfigurationValidatorBase
    {
      public override bool CanValidate(Type type) => type == typeof (long);

      public override void Validate(object value)
      {
        long actualValue = (long) value;
        if (actualValue < 0L || actualValue > (long) uint.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (value), (object) actualValue, SR.GetString("ArgumentOutOfRange_Bounds_Lower_Upper", (object) 0, (object) uint.MaxValue));
      }
    }
  }
}
