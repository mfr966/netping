using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netping {
    class Program {

        static void Run(Options o) {
            Config config = Config.load_JSON(o.ConfigFilename).WaitResult();

            if (o.Add) {
                // Add a hostname to the group of hostnames. Always add to "all".

                List<string> groups = o.Groups.Select(g => g.ToLower()).ToList();
                if (!groups.Contains("all")) {
                    groups.Add("all");
                }

                foreach (var groupName in groups) {
                    foreach (var host in o.Hostnames) {
                        if (!config.Groups.ContainsKey(groupName)) {
                            config.Groups.Add(groupName, new SortedSet<string>());
                        }
                        if (config.Groups[groupName].Add(host) && o.Verbose) {
                            Console.Out.WriteLine("Adding host {0} to group {1}", host, groupName);
                        }
                    }
                }

                Options.CreateConfigFile(o.ConfigFilename);
                Config.save_JSON(o.ConfigFilename, config).Wait();

            } else if (o.Delete) {
                // Remove a hostname from a group of hostnames. Always remove from "all".

                var groups = new SortedSet<string>(o.Groups.Select(g => g.ToLower()));
                if (groups.Contains("all")) {
                    foreach (var gr in config.Groups.Keys) {
                        if (!groups.Contains(gr)) {
                            groups.Add(gr);
                        }
                    }
                }

                foreach (var groupName in groups) {
                    foreach (var host in o.Hostnames) {
                        if (config.Groups.ContainsKey(groupName)) {
                            if (config.Groups[groupName].Remove(host) && o.Verbose) {
                                Console.Out.WriteLine("Removing host {0} from group {1}", host, groupName);
                            }
                        } else {
                            Console.Out.WriteLine("Group {0} does not exist.", groupName);
                        }
                    }
                }

                Options.CreateConfigFile(o.ConfigFilename);
                Config.save_JSON(o.ConfigFilename, config).Wait();

            } else if (o.DeleteGroup) {

                List<string> groups = o.Groups.Select(g => g.ToLower()).ToList();
                if (groups.Contains("all")) {
                    if (groups.Count == 1) {
                        // default "all" group only
                        Console.Out.WriteLine("Specify groups to remove.");
                    } else {
                        // all cannot be deleted
                        Console.Out.WriteLine("Group \"all\" cannot be deleted.");
                    }
                    groups.Remove("all");
                }

                foreach (var groupName in groups) {
                    if (config.Groups.ContainsKey(groupName)) {
                        if (config.Groups.Remove(groupName) && o.Verbose) {
                            Console.Out.WriteLine("Deleting group \"{0}\".", groupName);
                        }
                    } else {
                        Console.Out.WriteLine("Group \"{0}\" does not exist.", groupName);
                    }
                }

                Options.CreateConfigFile(o.ConfigFilename);
                Config.save_JSON(o.ConfigFilename, config).Wait();

            }

            if (o.Show) {
                var groups = new SortedSet<string>(o.Groups.Select(g => g.ToLower()));

                if (groups.Contains("*")) {
                    groups.UnionWith(config.Groups.Keys);
                }


                foreach (var gr in groups) {
                    if (config.Groups.ContainsKey(gr)) {
                        Console.Out.WriteLine("Group {0}:", gr);
                        foreach (var host in config.Groups[gr]) {
                            Console.Out.WriteLine("    {0}", host);
                        }
                    } else {
                        Console.Out.WriteLine("Group {0} does not exist.", gr);
                    }
                }
            }

            // if there is no other option specified, execute the ping for the selected groups
            if (!(new bool [] { o.Add, o.Delete, o.DeleteGroup, o.Show }).Where(b => b).Any()) {

                var pings = new List<Tuple<string, Task<int>>>();
                var hosts = new SortedSet<string>(o.Hostnames);

                if (hosts.Count == 0) { 
                    foreach (var gr in o.Groups) {
                        if (config.Groups.ContainsKey(gr)) {
                            hosts.UnionWith(config.Groups[gr]);
                        } else {
                            Console.Out.WriteLine("Group \"{0}\" does not exist.", gr);
                        }
                    }
                }

                foreach (var h in hosts) {
                    pings.Add(Tuple.Create(h, NetPing.SimplePingAsync(h, o.TimeOut, o.Verbose)));
                }

                Task.WaitAll(pings.Select(t => t.Item2).ToArray());

                foreach (var t in pings) {
                    if (o.Verbose) {
                        if (t.Item2.Result >= 0) {
                            Console.Out.WriteLine("{0}\t{1} ms", t.Item1, t.Item2.Result);
                        } else {
                            Console.Out.WriteLine("{0}\ttimeout", t.Item1);
                        }
                    } else {
                        if (t.Item2.Result < 0) {
                            Console.Out.WriteLine("{0}", t.Item1);
                        }
                    }
                }
            }


        }


        static void Main(string[] args) {

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed((o) => Run(o));

        }

    }
}
