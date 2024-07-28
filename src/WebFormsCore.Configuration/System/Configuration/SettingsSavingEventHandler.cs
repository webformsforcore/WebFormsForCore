
using System.ComponentModel;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsSaving" /> event.</summary>
  /// <param name="sender">The source of the event, typically a data container or data-bound collection.</param>
  /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
  public delegate void SettingsSavingEventHandler(object sender, CancelEventArgs e);
}
