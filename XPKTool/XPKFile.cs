namespace XPKTool;

/// <summary>
/// Represents a file entry in an XPK archive.
/// </summary>
public class XPKFile
{
	/// <summary>
	/// The name of the file, including its path relative to the archive.
	/// </summary>
	public string Name;

	/// <summary>
	/// The offset in the archive where the file's name starts.
	/// </summary>
	public uint NameOffset;

	/// <summary>
	/// The size of the file in bytes.
	/// </summary>
	public uint Size;

	/// <summary>
	/// The creation date of the file, represented as a Unix timestamp (seconds since 1970-01-01).
	/// </summary>
	public uint CreationDate;

	/// <summary>
	/// The offset in the archive where the file's data starts.
	/// </summary>
	public uint DataOffset;
}
