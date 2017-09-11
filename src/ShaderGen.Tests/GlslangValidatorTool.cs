﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShaderGen.Tests
{
    public static class GlsLangValidatorTool
    {
        private static readonly string s_fxcLocation = FindExe();

        public static void AssertCompilesCode(string code, string type)
        {
            using (TempFile tmpFile = new TempFile())
            {
                File.WriteAllText(tmpFile, code);
                AssertCompilesFile(tmpFile, type);
            }
        }

        public static void AssertCompilesFile(string file, string type, string output = null)
        {
            ToolResult result = Compile(file, type, output);
            if (result.ExitCode != 0)
            {
                string message = result.StdOut;
                throw new InvalidOperationException("HLSL compilation failed: " + message);
            }
        }

        public static ToolResult Compile(string file, string type, string output = null)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = s_fxcLocation,
                Arguments = FormatArgs(file, type, output),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process p = Process.Start(psi);
            p.WaitForExit();

            string stdOut = p.StandardOutput.ReadToEnd();
            string stdError = p.StandardError.ReadToEnd();
            return new ToolResult(p.ExitCode, stdOut, stdError);
        }

        private static string FormatArgs(string file, string type, string output = null)
        {
            StringBuilder args = new StringBuilder();
            args.Append("-S "); args.Append(type);
            if (output != null)
            {
                args.Append(" -o "); args.Append(output);
            }
            args.Append(" "); args.Append(file);
            return args.ToString();
        }

        private static string FindExe()
        {
            const string VulkanSdkEnvVar = "VULKAN_SDK";
            string vulkanSdkPath = Environment.GetEnvironmentVariable(VulkanSdkEnvVar);
            if (vulkanSdkPath != null)
            {
                string exePath = Path.Combine(vulkanSdkPath, "bin", "glslangvalidator.exe");
                if (File.Exists(exePath))
                {
                    return exePath;
                }
            }

            throw new InvalidOperationException("Couldn't locate fxc.exe.");
        }
    }
}