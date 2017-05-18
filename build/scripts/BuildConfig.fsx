﻿#I "../../packages/build/FAKE/tools"
#I "../../packages/build/Fsharp.Data/lib/net40"
#I "../../packages/build/Fsharp.Configuration/lib/net46"

#r "FakeLib.dll"
#r "Fsharp.Data.dll"
#r "System.Xml.Linq.dll"
#r "FSharp.Configuration.dll"
#load "Products.fsx"

namespace Scripts

module BuildConfig = 
    open System
    open Fake
    open FSharp.Configuration
    open Fake.StringHelper
    open Products.Products
    
    type TypedConfig = YamlConfig<"config.yaml">
    let private sourceYaml = __SOURCE_DIRECTORY__  </> "config.yaml"

    //ugly :)
    let private writeProductGuids = fun(config: TypedConfig) ->
        let file = __SOURCE_DIRECTORY__ </> "../../src/Elastic.Installer.Domain" </> "ProductGuids.cs"
        WriteStringToFile false file """using System;
using System.Collections.Generic;

//THIS IS GENERATED BY OUR BUILD SCRIPT
namespace Elastic.Installer.Domain
{
	public static class ProductGuids
	{
"""
        WriteStringToFile true file (sprintf "		public static Guid ElasticsearchUpgradeCode => new Guid(\"%A\");\r\n" config.elasticsearch.upgrade_code)
        WriteStringToFile true file (sprintf "		public static Guid KibanaUpgradeCode => new Guid(\"%A\");" config.kibana.upgrade_code)
        WriteStringToFile true file  """
		public static Dictionary<string, Guid> ElasticsearchProductCodes => new Dictionary<string, Guid>
		{
"""
        let dictValues = config.elasticsearch.known_versions |> Seq.map (fun v -> sprintf "			{ \"%s\", new Guid(\"%A\") }" v.version v.guid)
        let guids = dictValues |> String.concat ",\r\n"
        WriteStringToFile true file  guids
        WriteStringToFile true file """
		};
"""

        WriteStringToFile true file  """
		public static Dictionary<string, Guid> KibanaProductCodes => new Dictionary<string, Guid>
		{
"""
        let dictValues = config.kibana.known_versions |> Seq.map (fun v -> sprintf "			{ \"%s\", new Guid(\"%A\") }" v.version v.guid)
        let guids = dictValues |> String.concat ",\r\n"
        WriteStringToFile true file  guids
        WriteStringToFile true file """
		};
"""
        WriteStringToFile true file """
	}
}
"""

    let versionGuid (productVersions: ProductVersion list) =
        let config = TypedConfig()
        config.Load sourceYaml
        tracefn "found %i elasticsearch known versions" config.elasticsearch.known_versions.Count
        tracefn "found %i kibana known versions" config.kibana.known_versions.Count

        productVersions
        |> List.iter(fun p ->
            let version = p.Version.FullVersion
            match p.Product with
            | Elasticsearch ->
                match config.elasticsearch.known_versions |> Seq.tryFind (fun v -> v.version = version) with
                | Some _ -> ()
                | None ->
                    let newGuid = Guid.NewGuid()
                    let newVersion = new TypedConfig.elasticsearch_Type.known_versions_Item_Type(version=version, guid=newGuid)
                    config.elasticsearch.known_versions.Add newVersion
            | Kibana ->
                match config.kibana.known_versions |> Seq.tryFind (fun v -> v.version = version) with
                | Some _ -> ()
                | None ->
                    let newGuid = Guid.NewGuid()
                    let newVersion = new TypedConfig.kibana_Type.known_versions_Item_Type(version=version, guid=newGuid)
                    config.kibana.known_versions.Add newVersion
        )

        config.Save sourceYaml
        writeProductGuids config
