using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace netping {
    public class Config {

        public Dictionary<string, SortedSet<string>> Groups { get; set; } = new Dictionary<string, SortedSet<string>>();

        public static async Task save_JSON(string filename, Config config) {
            using (FileStream fStream = File.Create(filename)) {
                await JsonSerializer.SerializeAsync(fStream, config);
            }
        }

        public static async ValueTask<Config> load_JSON(string filename) {
            if (!File.Exists(filename)) {
                return new Config();
            }
            using (FileStream fStream = File.OpenRead(filename)) {
                return await JsonSerializer.DeserializeAsync<Config>(fStream);
            }
        }

    }
}
