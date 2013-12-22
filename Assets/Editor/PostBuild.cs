using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class PostBuild
{

	/// Wird nach dem Build ausgeführt
	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		DirectoryInfo targetDir = Directory.GetParent(path);
		DirectoryInfo projectDir = Directory.GetParent(Application.dataPath);
		//string buildName = Path.GetFileNameWithoutExtension(path);

		int fileCount = 0;
		int dirCount = 0;
		Logger.Log("Führe Post-Build aus...");

		if (targetDir.FullName.TrimEnd('\\').Equals(
				projectDir.FullName.TrimEnd('\\'),
				StringComparison.InvariantCultureIgnoreCase))
			Logger.LogWarning(
				"Achtung: Baue direkt in das Projektverzeichnis. Da Unity " +
				"die DLL-Dateien (WiiController.dll etc.)in diesem nicht " +
				"released, nachdem der interne Player beendet wurde, könnte " +
				"das Fehler beim Kopieren geben. Das ist solange kein " +
				"Problem, wie die DLLs aktuell sind. Trotzdem bitte in " +
				"Unterverzeichnis bauen!");

		FileHelper.CopyFile(
			new FileInfo("..\\CaveApi\\Release\\WiiClient.dll"),
			new DirectoryInfo(targetDir.FullName),
			ref fileCount);
		FileHelper.CopyFile(
			new FileInfo("..\\CaveApi\\Release\\KinectClient.dll"),
			new DirectoryInfo(targetDir.FullName),
			ref fileCount);
		FileHelper.CopyFile(
			new FileInfo("..\\CaveApi\\Release\\NavigationInterface.dll"),
			new DirectoryInfo(targetDir.FullName),
			ref fileCount);
		FileHelper.CopyFile(
			new FileInfo("ts3client_win32.dll"),
			new DirectoryInfo(targetDir.FullName),
			ref fileCount);
		FileHelper.CopyFile(
			new FileInfo("geraetekoordinaten_dummy.txt"),
			new DirectoryInfo(targetDir.FullName),
			ref fileCount);
		FileHelper.CopyAll(
			new DirectoryInfo("soundbackends"),
			new DirectoryInfo(Path.Combine(targetDir.FullName, "soundbackends")),
			ref dirCount,
			ref fileCount);

		Logger.Log("Kopiert: " + fileCount + 
			" Datei" + ((fileCount != 1) ? "en" : "") + ", " +
			dirCount + " Ordner");
	}
}
