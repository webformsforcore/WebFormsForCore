namespace System.Xml.Serialization;
internal interface INameScope
{
	object this[string name, string ns] { get; set; }
}
