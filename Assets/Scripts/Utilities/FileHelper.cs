using System.IO;

public static class FileHelper
{
	/// <summary>
	/// Datei source ins Verzeichnis target kopieren. Überschreibt vorhandene
	/// Dateien.
	/// </summary>
	/// <param name="source">QuellDATEI</param>
	/// <param name="target">ZielVERZEICHNIS</param>
	/// <param name="fileCount">wird um 1 erhöht, wenn erfolgreich</param>
	public static void CopyFile(
		FileInfo source,
		DirectoryInfo target,
		ref int fileCount)
	{
		if (source.Exists)
		{
			fileCount++;
			source.CopyTo(Path.Combine(target.ToString(), source.Name), true);
			Logger.Log("Datei kopiert: " + source.FullName);
		}
		else
		{
			Logger.LogError("Datei nicht gefunden: " + source.FullName);
		}
	}

	/// <summary>
	/// Verzeichnis rekursiv kopieren. Vorhandene Dateien werden überschreiben
	/// </summary>
	/// <param name="source">QuellVERZEICHNIS</param>
	/// <param name="target">ZielVERZEICHNIS</param>
	/// <param name="dirCount">für jedes erzeugte Verzeichnis +=1</param>
	/// <param name="fileCount">für jede erzeugte Datei +=1</param>
	public static void CopyAll(
		DirectoryInfo source,
		DirectoryInfo target,
		ref int dirCount,
		ref int fileCount)
	{
		// Check if the target directory exists, if not, create it.
		if (!Directory.Exists(target.FullName))
		{
			dirCount++;
			Directory.CreateDirectory(target.FullName);
			Logger.Log("Verzeichnis erstellt: " + target.FullName);
		}

		// Copy each file into its new directory.
		foreach (FileInfo fi in source.GetFiles())
		{
			fileCount++;
			fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
			Logger.Log("Datei kopiert: " + fi.FullName);
		}

		// Copy each subdirectory using recursion.
		foreach (DirectoryInfo diSrcSubDir in source.GetDirectories())
		{
			dirCount++;
			DirectoryInfo diNext = target.CreateSubdirectory(diSrcSubDir.Name);
			CopyAll(diSrcSubDir, diNext, ref dirCount, ref fileCount);
		}
	}
}
