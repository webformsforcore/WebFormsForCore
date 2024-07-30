
using System.Configuration;
using System.Net.Sockets;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents information used to configure <see cref="T:System.Net.Sockets.Socket" /> objects. This class cannot be inherited.</summary>
	public sealed class SocketElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty alwaysUseCompletionPortsForConnect = new ConfigurationProperty(nameof(alwaysUseCompletionPortsForConnect), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty alwaysUseCompletionPortsForAccept = new ConfigurationProperty(nameof(alwaysUseCompletionPortsForAccept), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty ipProtectionLevel = new ConfigurationProperty(nameof(ipProtectionLevel), typeof(IPProtectionLevel), (object)IPProtectionLevel.Unspecified, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.SocketElement" /> class.</summary>
		public SocketElement()
		{
			this.properties.Add(this.alwaysUseCompletionPortsForAccept);
			this.properties.Add(this.alwaysUseCompletionPortsForConnect);
			this.properties.Add(this.ipProtectionLevel);
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			try
			{
				ExceptionHelper.UnrestrictedSocketPermission.Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(SR.GetString("net_config_element_permission", (object)"socket"), ex);
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether completion ports are used when accepting connections.</summary>
		/// <returns>
		/// <see langword="true" /> to use completion ports; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("alwaysUseCompletionPortsForAccept", DefaultValue = false)]
		public bool AlwaysUseCompletionPortsForAccept
		{
			get => (bool)this[this.alwaysUseCompletionPortsForAccept];
			set => this[this.alwaysUseCompletionPortsForAccept] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that specifies whether completion ports are used when making connections.</summary>
		/// <returns>
		/// <see langword="true" /> to use completion ports; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("alwaysUseCompletionPortsForConnect", DefaultValue = false)]
		public bool AlwaysUseCompletionPortsForConnect
		{
			get => (bool)this[this.alwaysUseCompletionPortsForConnect];
			set => this[this.alwaysUseCompletionPortsForConnect] = (object)value;
		}

		/// <summary>Gets or sets a value that specifies the default <see cref="T:System.Net.Sockets.IPProtectionLevel" /> to use for a socket.</summary>
		/// <returns>The value of the <see cref="T:System.Net.Sockets.IPProtectionLevel" /> for the current instance.</returns>
		[ConfigurationProperty("ipProtectionLevel", DefaultValue = IPProtectionLevel.Unspecified)]
		public IPProtectionLevel IPProtectionLevel
		{
			get => (IPProtectionLevel)this[this.ipProtectionLevel];
			set => this[this.ipProtectionLevel] = (object)value;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	 }
}
