using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class DeviceConfig
    {
        public string WifiSsidContext { get; set; }

        [Required]
        public List<DeviceConfigItem> Items { get; set; }

        public static DeviceConfig Default {
            get
            {
                return new DeviceConfig() { Items = new List<DeviceConfigItem>() };
            }
        }

        public string ToTextPlain()
        {
            StringBuilder result = new StringBuilder();

            this.Items.ForEach(item =>
            {
                result.AppendLine(string.Format("{0}={1}", item.Key, item.Value));
            });

            return result.ToString();
        }
    }
}
