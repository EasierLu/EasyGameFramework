using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ProtobufAutoCompiler : AssetPostprocessor
{
    private const string PROTO_EXTENSION = ".proto";
    private const string OUT_PUT_PATH = "Scripts/Hotfix/Common/Protocol";
    private const string PROTOC_FILE_PATH = "ThirdParty/Protobuf/Editor/protoc.exe";
    private const string PROTO_FILE_DIR = "Assets/Scripts/Hotfix/Common/Protocol";
    private const string PROTO_AIPHELPER_TEMPLATE_PATH = "Assets/ThirdParty/Protobuf/Editor/ProtoAPIHelper_Template.txt";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool anyChange = false;
        foreach (string path in importedAssets)
        {
            if (path.StartsWith(PROTO_FILE_DIR) && path.EndsWith(PROTO_EXTENSION))
            {
                anyChange = true;
                break;
            }
        }

        if (anyChange)
        {
            string protoDir = Path.Combine(Application.dataPath, PROTO_FILE_DIR.Substring("Assets/".Length));
            var protoFiles = Directory.GetFiles(protoDir, "*.proto", SearchOption.AllDirectories);
            bool success = CompileProtobuf(protoDir, protoFiles);
            if (success)
            { 
                GenCSharpCode(protoFiles);
            }
            AssetDatabase.Refresh();
        }
    }

    private static bool CompileProtobuf(string protoDir,string[] protoFiles)
    {
        string outputPath = Path.Combine(Application.dataPath, OUT_PUT_PATH);
        string command = $" --csharp_out=\"{outputPath}\"";
        command += $" --proto_path=\"{protoDir}\"";
        foreach (var filePath in protoFiles)
        {
            command += $" \"{filePath}\"";
        }
        //UnityEngine.Debug.Log(command);

        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = Path.Combine(Application.dataPath,PROTOC_FILE_PATH), Arguments = command };

        Process proc = new Process() { StartInfo = startInfo };
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.Start();

        string output = proc.StandardOutput.ReadToEnd();
        string error = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        //UnityEngine.Debug.Log(output);

        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError(error);
            return false;
        }
        else
        {
            UnityEngine.Debug.Log("proto generate success");
            return true;
        }
    }

    private static HashSet<string> GetProtoApisDataScruct(string[] protoFiles)
    {
        HashSet<string> allApiDataScruct = new HashSet<string>(1024);
        Regex rg = new Regex("(?<=(\\nmessage))[.\\s\\S]*?(?=({))", RegexOptions.Multiline | RegexOptions.Singleline);
        foreach (var filePath in protoFiles)
        {
            try
            {
                string proto = File.ReadAllText(filePath);
                List<string> protoStructNames = RegexMatchs(rg, proto, " ", "\r\n");
                foreach (var structName in protoStructNames)
                {
                    if (IsProtoApi(structName))
                    {
                        allApiDataScruct.Add(structName);
                    }   
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        return allApiDataScruct;
    }

    private static List<string> GetProtoServiceNames(string[] protoFiles)
    {
        List<string> allApi = new List<string>(1024);
        Regex rg = new Regex("(?<=(ServiceApi))[.\\s\\S]*?(?=(}))", RegexOptions.Multiline | RegexOptions.Singleline);
        foreach (var filePath in protoFiles)
        {
            try
            {
                string proto = File.ReadAllText(filePath);
                string apiEnumCode = RegexMatchs(rg, proto, "{", "}", " ")[0];
                //UnityEngine.Debug.Log(apiEnumCode);
                foreach (var item in apiEnumCode.Split("\r\n"))
                {
                    if (string.IsNullOrEmpty(item) || item.StartsWith("/"))
                    {
                        continue;
                    }

                    string apiName = CamelCase(item.Split('=')[0]);
                    //UnityEngine.Debug.Log(apiName);
                    allApi.Add(apiName);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        return allApi;
    }
    
    private static bool IsProtoApi(string protoStructName)
    {
        foreach (var item in Enum.GetNames(typeof(ProtoApiType)))
        {
            if (protoStructName.EndsWith(item))
            {
                return true;
            }
        }

        return false;
    }

    private static void GenCSharpCode(string[] protoFiles)
    {
        HashSet<string> allApiDataScruct = GetProtoApisDataScruct(protoFiles);
        List<string> allApi = GetProtoServiceNames(protoFiles);
        string template = AssetDatabase.LoadAssetAtPath<TextAsset>(PROTO_AIPHELPER_TEMPLATE_PATH).text;

        StringBuilder dataTypeBuilder = new StringBuilder();
        //            { ServiceApi.Alive, new ProtoApiDataType(typeof(AliveRequest), typeof(AliveResponse)) },
        StringBuilder dataParsersBuilder = new StringBuilder();
        //            {typeof(Request).TypeHandle,Request.Parser },
        foreach (var item in allApi)
        {
            string requset = item + "Request";
            string requsetType = allApiDataScruct.Contains(requset) ? $"typeof({requset})" : "null";

            string response = item + "Response";
            string responseType = allApiDataScruct.Contains(response) ? $"typeof({response})" : "null";

            if (responseType == "null")
            {
                response = item + "Notification";
                responseType = allApiDataScruct.Contains(response) ? $"typeof({response})" : "null";
            }

            if (requsetType != "null" || responseType != "null")
            {
                dataTypeBuilder.AppendLine($"\t\t\t{{ ServiceApi.{item}, new ProtoApiDataType({requsetType}, {responseType}) }},");
                if (requsetType != "null")
                {
                    dataParsersBuilder.AppendLine($"\t\t\t{{typeof({requset}).TypeHandle,{requset}.Parser}},");
                }
                if (responseType != "null")
                {
                    dataParsersBuilder.AppendLine($"\t\t\t{{typeof({response}).TypeHandle,{response}.Parser}},");
                }
            }
            else
            {
                UnityEngine.Debug.Log($"<color=yellow>[WARNING]</color>ServiceApi.{item} 没有发送或接收时解析用数据结构");
            }
        }

        string desc = $"/*\r\n\tAuto generate by {typeof(ProtobufAutoCompiler)} do not modfiy!!!\r\n\tTemplate:{PROTO_AIPHELPER_TEMPLATE_PATH}\r\n*/\r\n";
        template = desc + template;

        template = template.Replace("$$DATA_TYPES$$", dataTypeBuilder.ToString());
        template = template.Replace("$$DATA_PARSERS$$", dataParsersBuilder.ToString());

        SaveCode(Path.Combine(Application.dataPath,PROTO_FILE_DIR.Substring("Assets/".Length), "ProtoAPIHelper.cs"), template);
    }
    private static void SaveCode(string filePath, string code)
    {
        string path = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(filePath, code, Encoding.UTF8);
    }

    private static List<string> RegexMatchs(Regex r, string source, params string[] remove)
    {
        MatchCollection match = r.Matches(source);
        List<string> matchResult = new List<string>();
        foreach (Match m in match)
        {
            string result = m.Value;
            if (remove != null)
            {
                foreach (var s in remove)
                {
                    result = result.Replace(s, "");
                }
            }
            matchResult.Add(result);
        }

        return matchResult;
    }

    private static string CamelCase(string s)
    {
        string result = "";
        foreach (var item in s.Split('_'))
        {
            var x = item;
            if (x.Length == 0) return "null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                (m) =>
                {
                    return m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value.ToLower();
                });
            result += x;
        }
        
        return result;
    }

    enum ProtoApiType
    {
        Request,
        Response,
        Notification
    }
}
