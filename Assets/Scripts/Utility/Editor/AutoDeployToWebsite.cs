using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using System.IO;

/// <summary>
/// copied from https://blog.walterlv.com/post/run-commands-using-csharp.html
/// </summary>
public class CommandRunner
{
    public string ExecutablePath { get; }
    public string WorkingDirectory { get; }

    public CommandRunner(string executablePath, string workingDirectory = null)
    {
        ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
        WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(executablePath);
    }

    public string Run(string arguments)
    {
        var info = new ProcessStartInfo(ExecutablePath, arguments)
        {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = WorkingDirectory,
        };
        var process = new Process
        {
            StartInfo = info,
        };
        process.Start();
        return process.StandardOutput.ReadToEnd();
    }
}

public static class AutoDeployToWebsite
{
    [MenuItem("策划/部署到网页端")]
    public static void Deploy()
    {
        string path = Application.dataPath;
        path = path.Substring(0, path.IndexOf(PlayerSettings.productName + "/Assets"));
        path += "SantaRushBuildTest";
        Debug.Log(path);
        var git = new CommandRunner("git", path);

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log(git.Run("clone git@github.com:dw218192/SantaRushBuildTest.git"));
        }

        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.locationPathName = path;
        opts.target = BuildTarget.WebGL;

        var report = BuildPipeline.BuildPlayer(opts);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError($"Webgl build failed with msg: {report.summary.ToString()}");
            return;
        }

        Debug.Log(git.Run("add ."));
        Debug.Log(git.Run("commit -m \"auto commit\""));
        Debug.Log(git.Run("push"));
    }
}