using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace netping {
    public class Options {

        [Option('a', "add", SetName="add", HelpText = "Add hostnames to list")]
        public bool Add { get; set; }

        [Option('d', "delete", SetName="delete", HelpText = "Delete hostnames from list")]
        public bool Delete { get; set; }

        [Option('s', "show", HelpText = "Show hostnames in list")]
        public bool Show { get; set; }

        [Option('D', "delete-group", SetName="delete-group", HelpText = "Delete a group and it's host list")]
        public bool DeleteGroup { get; set; }

        [Option('g', "groups", HelpText = "Hostname Groups. Show supports \"*\" for all groups.", Default = new string[] { "all" })]
        public IEnumerable<string> Groups { get; set; }

        [Option('v', "verbose", HelpText ="Show all ping results")]
        public bool Verbose { get; set; }

        [Option('c', "config", HelpText ="Configuration file")]
        public string ConfigFile { get; set; }

        [Option('t', "timeout", HelpText = "Timeout in ms", Default = 500)]
        public int TimeOut { get; set; }

        [Value(0, HelpText = "Hostnames to ping or add")]
        public IEnumerable<string> Hostnames { get; set; }

        /// <summary>
        /// return the  configuration file from the options, environment, or default.
        /// </summary>
        public string ConfigFilename { get {
                if (!string.IsNullOrWhiteSpace(ConfigFile)) {
                    return ConfigFile;
                }

                // Check for netping variable first
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NETPING"))) {
                    return Environment.GetEnvironmentVariable("NETPING");
                }

                // Check for HOME-variable and path first
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NETPING"))) {
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".netping", "config.json");
                }

                // return user profile
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".netping", "config.json");
            } 
        }

        /// <summary>
        /// Create the config file directory if it does not exist.
        /// </summary>
        /// <param name="filename"></param>
        public static void CreateConfigFile(string filename) {
            if (File.Exists(filename)) {
                return;
            }
            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

    }
}
